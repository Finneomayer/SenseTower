namespace Assets.Mechanics.MetaAvatars.Scripts
{
    public class IncreasingValue
    {
        public float MinValue { get; }
        public float MaxValue { get; }
        public float IncreaseMultiplier { get; }

        public float CurrentValue { get; private set; }
        public IncreasingValue(float minValue, float maxValue, float increaseMultiplier)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            IncreaseMultiplier = increaseMultiplier;

            CurrentValue = minValue;
        }

        public void Increase()
        {
            CurrentValue *= IncreaseMultiplier;
            if (CurrentValue > MaxValue)
            {
                CurrentValue = MaxValue;
            }
        }

        public void Reset()
        {
            CurrentValue = MinValue;
        }
    }
}