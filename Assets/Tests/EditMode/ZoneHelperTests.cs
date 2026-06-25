using NUnit.Framework;
using WheelOfFortune.Data;
using WheelOfFortune.Utilities;

namespace WheelOfFortune.Tests
{
    public class ZoneHelperTests
    {
        private const int SafeInterval = 5;
        private const int SuperInterval = 30;

        [Test]
        public void NormalSpin_ReturnsNormal()
        {
            Assert.AreEqual(ZoneType.Normal, ZoneHelper.GetZoneType(1, SafeInterval, SuperInterval));
            Assert.AreEqual(ZoneType.Normal, ZoneHelper.GetZoneType(2, SafeInterval, SuperInterval));
            Assert.AreEqual(ZoneType.Normal, ZoneHelper.GetZoneType(4, SafeInterval, SuperInterval));
        }

        [Test]
        public void FifthSpin_ReturnsSafe()
        {
            Assert.AreEqual(ZoneType.Safe, ZoneHelper.GetZoneType(5, SafeInterval, SuperInterval));
            Assert.AreEqual(ZoneType.Safe, ZoneHelper.GetZoneType(10, SafeInterval, SuperInterval));
            Assert.AreEqual(ZoneType.Safe, ZoneHelper.GetZoneType(25, SafeInterval, SuperInterval));
        }

        [Test]
        public void ThirtiethSpin_ReturnsSuper()
        {
            Assert.AreEqual(ZoneType.Super, ZoneHelper.GetZoneType(30, SafeInterval, SuperInterval));
            Assert.AreEqual(ZoneType.Super, ZoneHelper.GetZoneType(60, SafeInterval, SuperInterval));
        }

        [Test]
        public void SuperTakesPriorityOverSafe()
        {
            // 30 is divisible by both 5 and 30 — Super wins
            Assert.AreEqual(ZoneType.Super, ZoneHelper.GetZoneType(30, SafeInterval, SuperInterval));
        }
    }
}
