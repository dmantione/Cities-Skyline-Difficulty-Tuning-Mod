using ICities;
using ColossalFramework;

namespace DifficultyTuningMod.DifficultyOptions
{
    public class ResidentialTargetLandValue : DifficultyParameterMultiple
    {
        public ResidentialTargetLandValue() : base() { }

        protected override void InitValues()
        {
            nMin = -4;
            nMax = 10;
        }

        protected override int getValue(int n, Level level)
        {
            switch (level)
            {
                case Level.Level2:
                    return -2 + 4 * n;
                case Level.Level3:
                    return 7 + 7 * n;
                case Level.Level4:
                    return 21 + 10 * n;
                case Level.Level5:
                    return 37 + 12 * n;
            }

            return InvalidValue;
        }

        protected override int getTooLowValue(int n, Level level)
        {
            switch (level)
            {
                case Level.Level2:
                    return -3 + 2 * n;
                case Level.Level3:
                    return 1 + 5 * n;
                case Level.Level4:
                    return 15 + 8 * n;
                case Level.Level5:
                    return 31 + 10 * n;
            }

            return InvalidValue;
        }
    }
}