using PKHeX.Core;

namespace ZAShinyWarper.Hunting
{
    public class StashedShiny<T> where T : PKM, new()
    {
        public T PKM { get; private set; }
        public ulong LocationHash { get; private set; } = 0;
        public uint EncryptionConstant => PKM.EncryptionConstant;
        public int? Rolls { get; }
        public ulong? Seed { get; }

        public StashedShiny(T pk, ulong locHash)
        {
            PKM = pk;
            LocationHash = locHash;
            if (PKM is PA9)
                (Rolls, Seed) = ShinyRollChecker<T>.CheckValidDirtyZARNG(PKM);
        }

        public string GetRollsInfo()
        {
            if (Rolls.HasValue)
                return (Rolls.Value < 4 ? $"Rolls: {Rolls.Value} (Legit?)" : $"Rolls: {Rolls.Value} (Mad haxx)") + $" Seed: {Seed:X16}";
            else
                return "Rolls: N/A";
        }

        public override string ToString() => $"Location hash: {LocationHash:X16}\r\n{GetRollsInfo()}\r\n" + ShowdownParsing.GetShowdownText(PKM) + "\r\n";
        public string ToShowdownString() => ShowdownParsing.GetShowdownText(PKM);
    }
}
