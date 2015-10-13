namespace DifficultyTuningMod.DifficultyOptions
{
    public class MaintenanceCostMultiplier : DifficultyParameterBase
    {
        public MaintenanceCostMultiplier() : base() { }

        protected override void InitValues()
        {
            CustomValue = 100;
            customValues = new int[50];

            int i;
            for (i = 0; i <= 30; i++) customValues[i] = 5 * i;                // 0, 5, .. 150
            for (i = 31; i <= 35; i++) customValues[i] = 150 + 10 * (i - 30);  // 160, 170, .. 200
            for (i = 36; i <= 40; i++) customValues[i] = 200 + 20 * (i - 35);  // 220, 240, .. 300
            for (i = 41; i <= 44; i++) customValues[i] = 300 + 50 * (i - 40);  // 350, 400, 450, 500
            for (i = 45; i <= 49; i++) customValues[i] = 500 + 100 * (i - 44); // 600, 700, .. 1000
        }

        public override int GetValue(Difficulties difficultyLevel)
        {
            switch (difficultyLevel)
            {
                case Difficulties.Free:
                    return 50;
                case Difficulties.Easy:
                    return 75;
                case Difficulties.Normal:
                    return 100;
                case Difficulties.Advanced:
                    return 120;
                case Difficulties.Hard:
                    return 140;
                case Difficulties.Expert:
                    return 160;
                case Difficulties.Challenge:
                    return 180;
                case Difficulties.Impossible:
                    return 200;
                case Difficulties.Custom:
                    return CustomValue;
            }

            return 100;
        }
        
        protected override string valueToStr(int value)
        {
            return value.ToString() + "%";
        }
    }
}