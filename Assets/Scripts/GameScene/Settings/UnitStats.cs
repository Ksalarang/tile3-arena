using System;

namespace GameScene.Settings {
//todo: make abstract
public class UnitStats {
    public float health;
    public float damage;
    public float attackRate;
    public float viewRadius;
    public float attackRadius;
    public float speed;
    public float[] levelModifiers;
}

[Serializable]
public class WarriorStats : UnitStats {}

[Serializable]
public class RangerStats : UnitStats {}

[Serializable]
public class ArtilleryStats : UnitStats {}
}