using PKHeX.Core;

namespace ZAShinyWarper.Hunting
{
    public class ShinyFilter<T> where T : PKM, new()
    {
        public IVType[] IVs { get; set; } =
        [
            IVType.Any,
                IVType.Any,
                IVType.Any,
                IVType.Any,
                IVType.Any,
                IVType.Any
        ];
        public List<ushort>? SpeciesList { get; set; } = null;
        public byte SizeMinimum { get; set; } = 0;
        public byte SizeMaximum { get; set; } = 0;

        public bool IsAlpha { get; set; }

        public bool MatchesFilter(T pk)
        {
            if (!MatchesFilterSpeciesOrAlpha(pk))
                return false;
            // IVs
            for (int i = 0; i < 6; i++)
            {
                var iv = pk.GetIV(i);
                switch (IVs[i])
                {
                    case IVType.Perfect:
                        if (iv != 31)
                            return false;
                        break;
                    case IVType.Zero:
                        if (iv != 0)
                            return false;
                        break;
                }
            }
            return true;
        }

        // This is required otherwise the spawn may hog that encounter until it is defeated/captured.
        public bool MatchesFilterSpeciesOrAlpha(T pk)
        {
            // Species - check if species matches any in the list
            if (SpeciesList != null && SpeciesList.Count > 0)
            {
                if (!SpeciesList.Contains(pk.Species))
                    return false;
            }
            // Size
            if (pk is IScaledSize3 pks)
            {
                if (pks.Scale < SizeMinimum)
                    return false;
                if (pks.Scale > SizeMaximum)
                    return false;
            }
            // Alpha
            if (pk is IAlpha pka) // Even if scale is 255, Still needs alpha flag
            {
                if (IsAlpha && !pka.IsAlpha)
                    return false;
            }
            return true;
        }
    }
}
