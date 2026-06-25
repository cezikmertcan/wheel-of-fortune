using NUnit.Framework;
using WheelOfFortune.Utilities;

namespace WheelOfFortune.Tests
{
    public class RewardFormatterTests
    {
        [Test]
        public void ComputeValue_MultipliesAndRounds()
        {
            Assert.AreEqual(100, RewardFormatter.ComputeValue(100, 1f));
            Assert.AreEqual(200, RewardFormatter.ComputeValue(100, 2f));
            Assert.AreEqual(250, RewardFormatter.ComputeValue(100, 2.5f));
        }

        [Test]
        public void ComputeValue_RoundsBankersStyle()
        {
            // Mathf.RoundToInt uses banker's rounding: 4.5 → 4 (nearest even)
            Assert.AreEqual(4, RewardFormatter.ComputeValue(3, 1.5f));
            // 5.5 → 6
            Assert.AreEqual(6, RewardFormatter.ComputeValue(11, 0.5f));
        }

        [Test]
        public void FormatMultiplier_WholeNumber_OmitsDecimal()
        {
            Assert.AreEqual("x2", RewardFormatter.FormatMultiplier(2f));
            Assert.AreEqual("x10", RewardFormatter.FormatMultiplier(10f));
        }

        [Test]
        public void FormatMultiplier_Fractional_ShowsOneDecimal()
        {
            Assert.AreEqual("x1.5", RewardFormatter.FormatMultiplier(1.5f));
            Assert.AreEqual("x2.3", RewardFormatter.FormatMultiplier(2.3f));
        }

        [Test]
        public void FormatMultiplier_CustomPrefix()
        {
            Assert.AreEqual("X3", RewardFormatter.FormatMultiplier(3f, "X"));
        }
    }
}
