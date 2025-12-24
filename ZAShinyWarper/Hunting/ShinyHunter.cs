using System.Diagnostics;
using System.Text;
using PKHeX.Core;

namespace ZAShinyWarper.Hunting
{
    public class ShinyHunter<T> : IDisposable where T : PKM, new()
    {
        private const int STASHED_SHINIES_MAX = 10;
        private const int PA9_SIZE = 0x148;
        private const int STRUCT_SIZE = 0x28;
        private const string STASH_FOLDER = "StashedShinies";

        // Monitor intervals
        private const int WEATHER_CHECK_INTERVAL_MS = 60000;  // 1 minute
        private const int TIME_CHECK_INTERVAL_MS = 300000;    // 5 minutes
        private const int ERROR_RETRY_DELAY_MS = 1000;        // 1 Second

        private Weather? _lockedWeather = null;
        private CancellationTokenSource? _weatherLockCts = null;
        private TimeOfDay? _lockedTime = null;
        private CancellationTokenSource? _timeLockCts = null;
        private ConnectionWrapper connectionWrapper = default!;
        private bool _disposed = false;

        public IList<StashedShiny<T>> PreviousStashedShinies { get; private set; } = [];
        public IList<StashedShiny<T>> StashedShinies { get; private set; } = [];
        public IList<StashedShiny<T>> DifferentShinies { get; private set; } = [];

        // Event for errors instead of MessageBox
        public event EventHandler<string>? Error;

        public void Initialize(ConnectionWrapper wrapper)
        {
            connectionWrapper = wrapper;
        }

        /// <summary>
        /// Gets the count of stashed shinies by reading the array boundaries.
        /// </summary>
        private async Task<(int, ulong)> GetStashedShinyCount(CancellationToken token)
        {
            var structArrayStart = await connectionWrapper.GetArrayStartOffset(token);
            var invalidStartAddress = await connectionWrapper.GetInvalidStartOffset(token);

            if (invalidStartAddress <= structArrayStart)
                return (0, structArrayStart);

            return (Math.Min((int)((invalidStartAddress - structArrayStart) / STRUCT_SIZE), STASHED_SHINIES_MAX), structArrayStart);
        }

        /// <summary>
        /// Follows a chain of pointers starting from a base address.
        /// </summary>
        /// <returns>The final pointer address, or null if any pointer in the chain is 0</returns>
        private async Task<ulong?> FollowPointerChain(ulong startAddress, CancellationToken token, params ulong[] offsets)
        {
            ulong current = startAddress;
            foreach (var offset in offsets)
            {
                var data = await connectionWrapper.ReadBytesAbsolute(current + offset, 8, token);
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

        public async Task SetWeather(Weather weather, CancellationToken token, bool forced = true)
        {
            try
            {
                if (weather == Weather.None && forced)
                {
                    UnlockWeather();
                    return;
                }

                if (weather != Weather.None)
                {
                    var weatherBytes = BitConverter.GetBytes((uint)weather);
                    await connectionWrapper.SetCurrentWeather(weatherBytes, token).ConfigureAwait(false);

                    if (forced)
                        LockWeather(weather);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting weather: {ex.Message}");
            }
        }

        public async Task SetTime(TimeOfDay time, CancellationToken token, bool forced = true)
        {
            try
            {
                if (time == TimeOfDay.None && forced)
                {
                    UnlockTime();
                    return;
                }

                if (time != TimeOfDay.None)
                {
                    var timeBytes = BitConverter.GetBytes((float)time);
                    await connectionWrapper.SetCurrentTime(timeBytes, token).ConfigureAwait(false);

                    if (forced)
                        LockTime(time);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting time: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads the stashed shinies from RAM, compares them to previous and saves
        /// </summary>
        /// <returns>whether or not a new one has entered since previous</returns>
        public async Task<bool> LoadStashedShinies(CancellationToken token)
        {
            PreviousStashedShinies = StashedShinies;
            StashedShinies = [];
            var previousECs = new HashSet<uint>(PreviousStashedShinies.Select(x => x.EncryptionConstant));

            if (!Directory.Exists(STASH_FOLDER))
                Directory.CreateDirectory(STASH_FOLDER);

            // Get the actual count and start address of stashed shinies
            (int stashCount, ulong structArrayStart) = await GetStashedShinyCount(token);

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
                    var structData = await connectionWrapper.ReadBytesAbsolute(structOffset, STRUCT_SIZE, token);

                    var hash = BitConverter.ToUInt64(structData, 0);
                    var address = BitConverter.ToUInt64(structData, 8);

                    // Skip if address is 0 (despawned/invalid)
                    if (address == 0)
                        continue;

                    // Follow pointer chain: [[[address]+50]+30]+0]
                    var pkmAddress = await FollowPointerChain(address, token, 0x50, 0x30).ConfigureAwait(false);
                    if (!pkmAddress.HasValue)
                        continue;

                    // Read the PKM data (0x148 bytes)
                    var pkmData = await connectionWrapper.ReadBytesAbsolute(pkmAddress.Value, PA9_SIZE, token);
                    var pk = CreatePKM(pkmData);

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
        /// <param name="index">Index of the shiny to despawn (0-based)</param>
        public async Task ClearSingleFromCache(int index, CancellationToken token)
        {
            try
            {
                (int stashCount, ulong structArrayStart) = await GetStashedShinyCount(token);

                if (index < 0 || index >= stashCount)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index out of range for current stash count.");

                // Read the structure to despawn
                var structOffset = structArrayStart + (ulong)(index * STRUCT_SIZE);
                var structData = await connectionWrapper.ReadBytesAbsolute(structOffset, STRUCT_SIZE, token);

                // Set the address to 0 (offset 8 in the structure)
                Array.Clear(structData, 8, 8);

                // Shift all structures after this one down by one position
                for (int i = index; i < stashCount - 1; i++)
                {
                    var nextData = await connectionWrapper.ReadBytesAbsolute(structArrayStart + (ulong)((i + 1) * STRUCT_SIZE), STRUCT_SIZE, token);
                    await connectionWrapper.WriteBytesAbsolute(nextData, structArrayStart + (ulong)(i * STRUCT_SIZE), token);
                }

                // Write the despawned structure at the end
                var lastStructOffset = structArrayStart + (ulong)((stashCount - 1) * STRUCT_SIZE);
                await connectionWrapper.WriteBytesAbsolute(structData, lastStructOffset, token);

                // Update the invalid start address (decrease by one structure size)
                var invalidStartAddress = await connectionWrapper.GetInvalidStartOffset(token);
                var newInvalidStartAddress = invalidStartAddress - STRUCT_SIZE;

                // Write the new invalid start address back to metabase
                var metadataBase = await connectionWrapper.GetMetaBaseOffset(token);
                await connectionWrapper.WriteBytesAbsolute(BitConverter.GetBytes(newInvalidStartAddress), metadataBase + 0x380, token);
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, $"Failed to remove the shiny from stash. Please check your connection to the Switch.\n\nError: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Clears all shinies from the cache by resetting the invalid start pointer to match the array start.
        /// This is much faster than iterating through each slot.
        /// </summary>
        public async Task ClearAllFromCache(CancellationToken token)
        {
            try
            {
                // Get the array start address
                var structArrayStart = await connectionWrapper.GetArrayStartOffset(token);

                // Set the invalid start address equal to the array start (making count = 0)
                var metadataBase = await connectionWrapper.GetMetaBaseOffset(token);
                await connectionWrapper.WriteBytesAbsolute(BitConverter.GetBytes(structArrayStart), metadataBase + 0x380, token);

                // Clear local cache
                StashedShinies = [];
                DifferentShinies = [];
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, $"Failed to clear stash. Please check your connection to the Switch.\n\nError: {ex.Message}");
                throw;
            }
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

        public void LockWeather(Weather weather)
        {
            // Dispose old token source if it exists
            _weatherLockCts?.Cancel();
            _weatherLockCts?.Dispose();

            _lockedWeather = weather;
            _weatherLockCts = new CancellationTokenSource();

            // Start background task to monitor and maintain weather
            _ = Task.Run(() => MonitorWeather(_weatherLockCts.Token));
        }

        public void UnlockWeather()
        {
            _lockedWeather = null;
            _weatherLockCts?.Cancel();
            _weatherLockCts?.Dispose();
            _weatherLockCts = null;
        }

        public void LockTime(TimeOfDay time)
        {
            // Dispose old token source if it exists
            _timeLockCts?.Cancel();
            _timeLockCts?.Dispose();

            _lockedTime = time;
            _timeLockCts = new CancellationTokenSource();

            // Start background task to monitor and maintain time
            _ = Task.Run(() => MonitorTime(_timeLockCts.Token));
        }

        public void UnlockTime()
        {
            _lockedTime = null;
            _timeLockCts?.Cancel();
            _timeLockCts?.Dispose();
            _timeLockCts = null;
        }

        private async Task MonitorWeather(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _lockedWeather.HasValue)
            {
                try
                {
                    var targetWeather = _lockedWeather.Value;
                    var currentWeatherBytes = await connectionWrapper.GetCurrentWeather(token).ConfigureAwait(false);
                    var currentWeather = (Weather)BitConverter.ToUInt32(currentWeatherBytes, 0);

                    if (currentWeather != targetWeather)
                    {
                        var weatherBytes = BitConverter.GetBytes((uint)targetWeather);
                        await connectionWrapper.SetCurrentWeather(weatherBytes, token).ConfigureAwait(false);
                        Debug.WriteLine($"Weather changed from {currentWeather} to {targetWeather}, correcting...");
                    }

                    await Task.Delay(WEATHER_CHECK_INTERVAL_MS, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error monitoring weather: {ex.Message}");
                    await Task.Delay(ERROR_RETRY_DELAY_MS, token);
                }
            }
        }

        private async Task MonitorTime(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _lockedTime.HasValue)
            {
                try
                {
                    var targetTime = (float)_lockedTime.Value;
                    var currentTimeBytes = await connectionWrapper.GetCurrentTime(token).ConfigureAwait(false);
                    var currentTime = BitConverter.ToSingle(currentTimeBytes, 0);

                    if (currentTime != targetTime)
                    {
                        var timeBytes = BitConverter.GetBytes(targetTime);
                        await connectionWrapper.SetCurrentTime(timeBytes, token).ConfigureAwait(false);
                        Debug.WriteLine($"Time drifted from {targetTime} to {currentTime}, correcting...");
                    }

                    await Task.Delay(TIME_CHECK_INTERVAL_MS, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error monitoring time: {ex.Message}");
                    await Task.Delay(ERROR_RETRY_DELAY_MS, token);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    UnlockWeather();
                    UnlockTime();
                }
                _disposed = true;
            }
        }
    }
}