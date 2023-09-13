using UnityEngine;

namespace Utils {
public static class Vector3Utils {
    public static Vector3 create(float value) => new(value, value, value);
}
}