using System;
using UnityEngine;

namespace GameScene.Settings {
public class GameSettings : MonoBehaviour {
    public UnitAnimationSettings unitAnimation;
    public PrepActionSettings prepActions;
    public ArenaSettings arena;
    public MergeLineSettings mergeLine;
    public UnitSpawnSettings unitSpawn;
    public UnitStatsSettings unitStats;
    public ProjectileSettings projectiles;
}

[Serializable]
public struct UnitAnimationSettings {
    public float moveDuration;
    public float deathFadeDelay;
    public float damageIndicationDuration;
    [Range(0f, 1f)] public float damageShadeIntensity;
    [Range(0f, 1f)] public float projectileShadeIntensity;
    [Header("Skeletal animations")]
    public float attackDuration;
    [Range(0f, 1f)] public float attackTime;
    [Header("Misc")]
    public bool showHealth;
}

[Serializable]
public struct PrepActionSettings {
    public int actionPointsPerRound;
    [Header("Cost for actions")]
    public int placeUnitOnMergeLine;
    public int placeUnitOnArena;
    public int respawnUnitsInSpawnArea;
}

[Serializable]
public struct ArenaSettings {
    public int unitsPerRow;
    public int unitWidthReductionFactor;
    public Vector2 distanceBetweenUnits;
    public float distanceToWall;
}

[Serializable]
public struct MergeLineSettings {
    [Range(0.1f, 1f)] public float unitSizeModifier;
}

[Serializable]
public struct UnitSpawnSettings {
    [Range(0.1f, 1f)] public float unitSizeModifier;
    [Header("Unit proportions")]
    [SerializeField] public float warrior;
    [SerializeField] public float ranger;
    [SerializeField] public float artillery;
}

[Serializable]
public struct UnitStatsSettings {
    [SerializeField] public WarriorStats warriorStats;
    [SerializeField] public RangerStats rangerStats;
    [SerializeField] public ArtilleryStats artilleryStats;
}

[Serializable]
public struct ProjectileSettings {
    public BulletStats bulletStats;
    public ShellStats shellStats;
}
}