using System;

namespace utils.structs {
public readonly struct IntRange {
    public readonly int min;
    public readonly int max;

    public IntRange(int min, int max) {
        if (max <= min) throw new ArgumentException($"max ({max}) must be greater than min ({min})");
        this.min = min;
        this.max = max;
    }

    public bool includes(int value) => min <= value && value <= max;
    
    public override string ToString() => $"[{min}, {max}]";
}
}