using WheelOfFortune.Data;

namespace WheelOfFortune.Utilities
{
    public static class ZoneHelper
    {
        public static ZoneType GetZoneType(int spinCount, int safeInterval, int superInterval)
        {
            if (spinCount % superInterval == 0) return ZoneType.Super;
            if (spinCount % safeInterval == 0) return ZoneType.Safe;
            return ZoneType.Normal;
        }
    }
}
