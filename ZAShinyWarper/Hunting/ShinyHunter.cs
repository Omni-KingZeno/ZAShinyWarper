using System.Diagnostics;
using System.Text;
using PKHeX.Core;
using ZAShinyWarper.Injection;

namespace ZAShinyWarper.Hunting
{
    public enum ShinyFoundAction
    {
        StopOnFound,
        StopAtFullCache,
        CacheAndContinue,        
        ClearCacheAndContinue
    }

    public enum IVType
    {
        Any,
        Perfect, // 31
        Zero // 0
    }

    public class ShinyHunter<T> where T : PKM, new()
    {
        private const int STASHED_SHINIES_MAX = 10;
        private const int PA9_SIZE = 0x148;
        private const int STRUCT_SIZE = 0x28;
        private const string STASH_FOLDER = "StashedShinies";

        // Pointer to the array start: [[main+4200D20]+350]
        private readonly long[] arrayStartPointer = [0x4200D20, 0x350];
        // Pointer to invalid start: [[main+4200D20]+358]
        private readonly long[] invalidStartPointer = [0x4200D20, 0x358];

        public IList<StashedShiny<T>> PreviousStashedShinies { get; private set; } = [];
        public IList<StashedShiny<T>> StashedShinies { get; private set; } = [];
        public IList<StashedShiny<T>> DifferentShinies { get; private set; } = [];

        /// <summary>
        /// Gets the count of stashed shinies by reading the array boundaries.
        /// </summary>
        private int GetStashedShinyCount(IRAMReadWriter bot, out ulong structArrayStart)
        {
            structArrayStart = bot.FollowMainPointer(arrayStartPointer);
            var invalidStartAddress = bot.FollowMainPointer(invalidStartPointer);

            if (invalidStartAddress <= structArrayStart)
                return 0;

            return Math.Min((int)((invalidStartAddress - structArrayStart) / STRUCT_SIZE), STASHED_SHINIES_MAX);
        }

        /// <summary>
        /// Follows a chain of pointers starting from a base address.
        /// </summary>
        /// <returns>The final pointer address, or null if any pointer in the chain is 0</returns>
        private static ulong? FollowPointerChain(IRAMReadWriter bot, ulong startAddress, params ulong[] offsets)
        {
            ulong current = startAddress;
            foreach (var offset in offsets)
            {
                var data = bot.ReadBytes(current + offset, 8, RWMethod.Absolute);
                current = BitConverter.ToUInt64(data, 0);
                if (current == 0)
                    return null;
            }
            return current;
        }

        /// <summary>
        /// Creates a PKM instance from byte data.
        /// </summary>
        private static T CreatePKM(byte[] data)
        {
            return (T)Activator.CreateInstance(typeof(T), new Memory<byte>(data))!;
        }

        /// <summary>
        /// Loads the stashed shinies from RAM, compares them to previous and saves
        /// </summary>
        /// <param name="bot"></param>
        /// <returns>whether or not a new one has entered since previous</returns>
        public bool LoadStashedShinies(IRAMReadWriter bot)
        {
            PreviousStashedShinies = StashedShinies;
            StashedShinies = [];
            var previousECs = new HashSet<uint>(PreviousStashedShinies.Select(x => x.EncryptionConstant));

            if (!Directory.Exists(STASH_FOLDER))
                Directory.CreateDirectory(STASH_FOLDER);

            // Get the actual count and start address of stashed shinies
            int stashCount = GetStashedShinyCount(bot, out ulong structArrayStart);

            if (stashCount == 0)
            {
                DifferentShinies = [];
                return false;
            }

            for (int i = 0; i < stashCount; i++)
            {
                try
                {
                    // Calculate offset to structure i in the array
                    var structOffset = structArrayStart + (ulong)(i * STRUCT_SIZE);

                    // Read the 0x28 structure: u64 hash, u64 address, 3x u64 unknown
                    var structData = bot.ReadBytes(structOffset, STRUCT_SIZE, RWMethod.Absolute);

                    var hash = BitConverter.ToUInt64(structData, 0);
                    var address = BitConverter.ToUInt64(structData, 8);

                    // Skip if address is 0 (despawned/invalid)
                    if (address == 0)
                        continue;

                    // Follow pointer chain: [[[address]+50]+30]+0]
                    var pkmAddress = ShinyHunter<T>.FollowPointerChain(bot, address, 0x50, 0x30);
                    if (!pkmAddress.HasValue)
                        continue;

                    // Read the PKM data (0x148 bytes)
                    var pkmData = bot.ReadBytes(pkmAddress.Value, PA9_SIZE, RWMethod.Absolute);
                    var pk = ShinyHunter<T>.CreatePKM(pkmData);

                    if (pk.Species != 0 && !StashedShinies.Any(x => x.EncryptionConstant == pk.EncryptionConstant))
                    {
                        var stashed = new StashedShiny<T>(pk, hash);
                        StashedShinies.Add(stashed);
                        var fileName = Path.Combine(STASH_FOLDER, pk.FileName);
                        File.WriteAllBytes(fileName, pk.DecryptedPartyData);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error reading shiny at index {i}: {ex.Message}");
                    continue;
                }
            }

            DifferentShinies = StashedShinies.Where(pk => !previousECs.Contains(pk.EncryptionConstant)).ToList();
            return DifferentShinies.Count > 0;
        }

        /// <summary>
        /// Despawns a shiny by setting its address to 0 and moving it to the end of the array.
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="index">Index of the shiny to despawn (0-based)</param>
        public void RemoveShinyFromCache(IRAMReadWriter bot, int index)
        {
            int stashCount = GetStashedShinyCount(bot, out ulong structArrayStart);

            if (index < 0 || index >= stashCount)
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range for current stash count.");

            // Read the structure to despawn
            var structOffset = structArrayStart + (ulong)(index * STRUCT_SIZE);
            var structData = bot.ReadBytes(structOffset, STRUCT_SIZE, RWMethod.Absolute);

            // Set the address to 0 (offset 8 in the structure)
            Array.Clear(structData, 8, 8);

            // Shift all structures after this one down by one position
            for (int i = index; i < stashCount - 1; i++)
            {
                var nextData = bot.ReadBytes(structArrayStart + (ulong)((i + 1) * STRUCT_SIZE), STRUCT_SIZE, RWMethod.Absolute);
                bot.WriteBytes(nextData, structArrayStart + (ulong)(i * STRUCT_SIZE), RWMethod.Absolute);
            }

            // Write the despawned structure at the end
            var lastStructOffset = structArrayStart + (ulong)((stashCount - 1) * STRUCT_SIZE);
            bot.WriteBytes(structData, lastStructOffset, RWMethod.Absolute);

            // Update the invalid start address (decrease by one structure size)
            var invalidStartAddress = bot.FollowMainPointer(invalidStartPointer);
            var newInvalidStartAddress = invalidStartAddress - STRUCT_SIZE;

            // Write the new invalid start address back to [[main+4200D20]+358]
            var metadataBase = bot.FollowMainPointer([0x4200D20]);
            bot.WriteBytes(BitConverter.GetBytes(newInvalidStartAddress), metadataBase + 0x358, RWMethod.Absolute);
        }

        /// <summary>
        /// Clears all shinies from the cache by resetting the invalid start pointer to match the array start.
        /// This is much faster than iterating through each slot.
        /// </summary>
        public void ClearAllFromStash(IRAMReadWriter bot)
        {
            // Get the array start address
            var structArrayStart = bot.FollowMainPointer(arrayStartPointer);

            // Set the invalid start address equal to the array start (making count = 0)
            var metadataBase = bot.FollowMainPointer([0x4200D20]);
            bot.WriteBytes(BitConverter.GetBytes(structArrayStart), metadataBase + 0x358, RWMethod.Absolute);

            // Clear local cache
            StashedShinies = [];
            DifferentShinies = [];
        }

        public string GetShinyStashInfo(IList<StashedShiny<T>> stash)
        {
            if (stash.Count == 0)
                return "No stashed shinies found.";

            var info = new StringBuilder("The following shinies are stashed on your save currently:\r\n\r\n");
            foreach (var pk in stash)
            {
                info.AppendLine(pk.ToString());
            }
            return info.ToString();
        }
    }
}