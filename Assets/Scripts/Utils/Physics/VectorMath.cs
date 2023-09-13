using UnityEngine;

namespace Utils.physics {
public static class VectorMath {
    
    public static Vector2 calculateForce(float angleDegrees, float value) {
        var vector = new Vector2();
        var angleRadians = angleDegrees * Mathf.Deg2Rad;
        vector.x = value * Mathf.Cos(angleRadians);
        vector.y = value * Mathf.Sin(angleRadians);
        return vector;
    }
    
    /// <summary>
    /// The angle starts from rightmost position and is rotated counter-clockwise.
    /// </summary>
    public static Vector3 getPositionAroundCenter(Vector3 center, float radius, float angleDegrees, float z = 0f) {
        var angle = angleDegrees * Mathf.Deg2Rad;
        var x = center.x + radius * Mathf.Cos(angle);
        var y = center.y + radius * Mathf.Sin(angle);
        return new Vector3(x, y, z);
    }
}
}