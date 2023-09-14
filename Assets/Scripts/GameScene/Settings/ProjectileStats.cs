using System;

namespace GameScene.Settings {
public abstract class ProjectileStats {
    // [HideInInspector] public float damage;
    public float speed;
    public float collateralDamageRadius;
    public float collateralDamageFactor;

    public abstract ProjectileStats clone();
}

[Serializable]
public class BulletStats : ProjectileStats {
    public override ProjectileStats clone() {
        return new BulletStats {
            speed = speed,
            collateralDamageRadius = collateralDamageRadius,
            collateralDamageFactor = collateralDamageFactor,
        };
    }
}

[Serializable]
public class ShellStats : ProjectileStats {
    public float maxAltitude;

    public override ProjectileStats clone() {
        return new ShellStats {
            speed = speed,
            collateralDamageRadius = collateralDamageRadius,
            collateralDamageFactor = collateralDamageFactor,
            maxAltitude = maxAltitude,
        };
    }
}
}