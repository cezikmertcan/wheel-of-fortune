using NUnit.Framework;
using WheelOfFortune.Economy;

namespace WheelOfFortune.Tests
{
    public class CurrencyManagerTests
    {
        private class FakeCurrencyManager : ICurrencyManager
        {
            private int _balance;
            public int Balance => _balance;
            public int ReviveCost { get; }

            public FakeCurrencyManager(int startBalance, int reviveCost)
            {
                _balance = startBalance;
                ReviveCost = reviveCost;
            }

            public bool TrySpend(int cost)
            {
                if (_balance < cost) return false;
                _balance -= cost;
                return true;
            }

            public void Add(int amount) => _balance += amount;
        }

        [Test]
        public void TrySpend_SufficientBalance_DeductsAndReturnsTrue()
        {
            var mgr = new FakeCurrencyManager(50, 25);
            bool result = mgr.TrySpend(25);
            Assert.IsTrue(result);
            Assert.AreEqual(25, mgr.Balance);
        }

        [Test]
        public void TrySpend_InsufficientBalance_ReturnsFalseAndKeepsBalance()
        {
            var mgr = new FakeCurrencyManager(10, 25);
            bool result = mgr.TrySpend(25);
            Assert.IsFalse(result);
            Assert.AreEqual(10, mgr.Balance);
        }

        [Test]
        public void Add_IncreasesBalance()
        {
            var mgr = new FakeCurrencyManager(0, 25);
            mgr.Add(100);
            Assert.AreEqual(100, mgr.Balance);
        }

        [Test]
        public void TrySpend_ExactBalance_Succeeds()
        {
            var mgr = new FakeCurrencyManager(25, 25);
            Assert.IsTrue(mgr.TrySpend(25));
            Assert.AreEqual(0, mgr.Balance);
        }
    }
}
