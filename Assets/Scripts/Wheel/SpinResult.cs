using WheelOfFortune.Data;

namespace WheelOfFortune.Wheel
{
    public class SpinResult
    {
        public SliceData Slice { get; }
        public int SliceIndex { get; }
        public float Multiplier { get; }
        public bool IsBomb => Slice == null;

        public SpinResult(SliceData slice, int sliceIndex, float multiplier)
        {
            Slice = slice;
            SliceIndex = sliceIndex;
            Multiplier = multiplier;
        }
    }
}
