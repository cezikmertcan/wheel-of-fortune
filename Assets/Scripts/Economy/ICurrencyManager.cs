namespace WheelOfFortune.Economy
{
    public interface ICurrencyManager
    {
        int Balance { get; }
        int ReviveCost { get; }
        bool TrySpend(int cost);
        void Add(int amount);
    }
}
