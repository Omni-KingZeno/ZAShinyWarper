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
        CacheAndContinue
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
        private const int PA9_SIZE = 0x158;
        private const int PA9_BUFFER = 0x1F0;
        private const string STASH_FOLDER = "StashedShinies";

        private readonly long[] jumpsPos = [0x5F0B250, 0x120, 0x168]; // [[[main+5F0B250]+120]+168]

        public IList<StashedShiny<T>> PreviousStashedShinies { get; private set; } = [];
        public IList<StashedShiny<T>> StashedShinies { get; private set; } = [];
        public IList<StashedShiny<T>> DifferentShinies { get; private set; } = [];

        private ulong GetShinyStashOffset(IRAMReadWriter bot)
        {
            return bot.FollowMainPointer(jumpsPos);
        }

        /// <summary>
        /// Loads the stashed shinies from RAM, compares them to previous and saves them to disk.
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="path"></param>
        /// <returns>whether or not a new one has entered since previous</returns>
        public bool LoadStashedShinies(IRAMReadWriter bot)
        {
            var offs = GetShinyStashOffset(bot);
            PreviousStashedShinies = StashedShinies;
            StashedShinies = [];

            if (!Directory.Exists(STASH_FOLDER))
                Directory.CreateDirectory(STASH_FOLDER);

            for (int i = 0; i < STASHED_SHINIES_MAX; i++)
            {
                var data = bot.ReadBytes(offs + (ulong)(i * PA9_BUFFER), PA9_SIZE + 8, RWMethod.Absolute);
                var construct = typeof(T).GetConstructor([typeof(Memory<byte>)]);
                Debug.Assert(construct != null, "PKM type must have a Memory<byte> constructor");

                var pk = (T)construct.Invoke([new Memory<byte>(data[8..])]);
                var location = BitConverter.ToUInt64(data, 0);
                if (pk.Species != 0)
                {
                    var stashed = new StashedShiny<T>(pk, location);
                    if (!StashedShinies.Any(x => x.EncryptionConstant == pk.EncryptionConstant))
                    {
                        StashedShinies.Add(stashed);

                        var fileName = Path.Combine(STASH_FOLDER, pk.FileName);
                        File.WriteAllBytes(fileName, pk.DecryptedPartyData);
                    }
                }
            }

            DifferentShinies = StashedShinies.Where(pk => !PreviousStashedShinies.Any(x => x.EncryptionConstant == pk.EncryptionConstant)).ToList();
            return DifferentShinies.Any();
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
