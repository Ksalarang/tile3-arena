using System;
using UnityEngine;

namespace Utils.enums {
public enum Direction {
    Up, Right, Down, Left
}

public static class DirectionExtensions {
    public static Direction opposite(this Direction direction) {
        return direction switch {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
    
    public static Vector2Int toVector(this Direction direction) {
        var vector = new Vector2Int();
        switch (direction) {
            case Direction.Up:
                vector.y = 1;
                break;
            case Direction.Down:
                vector.y = -1;
                break;
            case Direction.Left:
                vector.x = -1;
                break;
            case Direction.Right:
                vector.x = 1;
                break;
        }
        return vector;
    }

    public static bool isVertical(this Direction direction) => direction is Direction.Up or Direction.Down;

    public static bool isHorizontal(this Direction direction) => direction is Direction.Left or Direction.Right;
}
}