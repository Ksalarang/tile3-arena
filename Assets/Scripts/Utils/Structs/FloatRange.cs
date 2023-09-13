using System;

namespace utils.structs {
public readonly struct FloatRange {
    public readonly float min;
    public readonly float max;

    public FloatRange(float min, float max) {
        if (max <= min) throw new ArgumentException($"max ({max}) must be greater than min ({min})");
        this.min = min;
        this.max = max;
    }
    
    public bool includes(int value) => min <= value && value <= max;

    public override string ToString() => $"[{min}, {max}]";
}
}