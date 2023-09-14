using System;
using System.Collections.Generic;
using GameScene.Contracts;
using GameScene.Models;
using GameScene.Settings;
using GameScreen;
using UnityEngine;
using Zenject;

namespace GameScene.Controllers {
public class UnitFactory : MonoBehaviour, UnitAnimatorFactory {
    // unit prefabs
    [Inject(Id = PrefabId.Warrior)] GameObject warriorPrefab;
    [Inject(Id = PrefabId.Ranger)] GameObject rangerPrefab;
    [Inject(Id = PrefabId.Artillery)] GameObject artilleryPrefab;
    // zombie skeleton prefabs
    // level 1
    [Inject(Id = UnitAnimatorId.ZombieWarrior1)] GameObject zombieWarrior1;
    [Inject(Id = UnitAnimatorId.ZombieRanger1)] GameObject zombieRanger1;
    [Inject(Id = UnitAnimatorId.ZombieArtillery1)] GameObject zombieArtillery1;
    // level 2
    [Inject(Id = UnitAnimatorId.ZombieWarrior2)] GameObject zombieWarrior2;
    [Inject(Id = UnitAnimatorId.ZombieRanger2)] GameObject zombieRanger2;
    [Inject(Id = UnitAnimatorId.ZombieArtillery2)] GameObject zombieArtillery2;
    // level 3
    [Inject(Id = UnitAnimatorId.ZombieWarrior3)] GameObject zombieWarrior3;
    [Inject(Id = UnitAnimatorId.ZombieRanger3)] GameObject zombieRanger3;
    [Inject(Id = UnitAnimatorId.ZombieArtillery3)] GameObject zombieArtillery3;
    // alien skeleton prefabs
    // level 1
    [Inject(Id = UnitAnimatorId.AlienWarrior1)] GameObject alienWarrior1;
    [Inject(Id = UnitAnimatorId.AlienRanger1)] GameObject alienRanger1;
    [Inject(Id = UnitAnimatorId.AlienArtillery1)] GameObject alienArtillery1;
    // level 2
    [Inject(Id = UnitAnimatorId.AlienWarrior2)] GameObject alienWarrior2;
    [Inject(Id = UnitAnimatorId.AlienRanger2)] GameObject alienRanger2;
    [Inject(Id = UnitAnimatorId.AlienArtillery2)] GameObject alienArtillery2;
    // level 3
    [Inject(Id = UnitAnimatorId.AlienWarrior3)] GameObject alienWarrior3;
    [Inject(Id = UnitAnimatorId.AlienRanger3)] GameObject alienRanger3;
    [Inject(Id = UnitAnimatorId.AlienArtillery3)] GameObject alienArtillery3;
    
    [Inject] DiContainer diContainer;
    [Inject] UnitStatsSettings unitStats;

    List<GameObject> zombieSkeletons;
    List<GameObject> alienSkeletons;

    void Awake() {
        initZombieSkeletonList();
        initAlienSkeletonList();
    }

    void initZombieSkeletonList() {
        zombieSkeletons = new() {
            zombieWarrior1,
            zombieWarrior2,
            zombieWarrior3,
            zombieRanger1,
            zombieRanger2,
            zombieRanger3,
            zombieArtillery1,
            zombieArtillery2,
            zombieArtillery3
        };
    }

    void initAlienSkeletonList() {
        alienSkeletons = new() {
            alienWarrior1,
            alienWarrior2,
            alienWarrior3,
            alienRanger1,
            alienRanger2,
            alienRanger3,
            alienArtillery1,
            alienArtillery2,
            alienArtillery3
        };
    }

    #region unit creation
    // ReSharper disable Unity.PerformanceAnalysis
    public Unit create(UnitClass unitClass, bool isPlayer = true) {
        var unit = diContainer.InstantiatePrefabForComponent<Unit>(getPrefabForUnitClass(unitClass));
        unit.setStats(getUnitStatsForClass(unitClass));
        unit.setSide(isPlayer);
        var animationPrefab = getAnimationPrefab(isPlayer, unitClass);
        var animator = diContainer.InstantiatePrefabForComponent<Animator>(animationPrefab);
        unit.setAnimator(animator);
        return unit;
    }

    UnitStats getUnitStatsForClass(UnitClass unitClass) {
        return unitClass switch {
            UnitClass.Warrior => unitStats.warriorStats,
            UnitClass.Ranger => unitStats.rangerStats,
            UnitClass.Artillery => unitStats.artilleryStats,
            _ => throw new ArgumentOutOfRangeException(nameof(unitClass), unitClass, null)
        };
    }

    GameObject getPrefabForUnitClass(UnitClass unitClass) {
        return unitClass switch {
            UnitClass.Warrior => warriorPrefab,
            UnitClass.Ranger => rangerPrefab,
            UnitClass.Artillery => artilleryPrefab,
            _ => null,
        };
    }

    GameObject getAnimationPrefab(bool isPlayer, UnitClass unitClass) {
        if (isPlayer) {
            return unitClass switch {
                UnitClass.Warrior => zombieWarrior1,
                UnitClass.Ranger => zombieRanger1,
                UnitClass.Artillery => zombieArtillery1,
                _ => throw new ArgumentOutOfRangeException($"there is no animator prefab for unit class {unitClass}")
            };
        } else {
            return unitClass switch {
                UnitClass.Warrior => alienWarrior1,
                UnitClass.Ranger => alienRanger1,
                UnitClass.Artillery => alienArtillery1,
                _ => throw new ArgumentOutOfRangeException($"there is no animator prefab for unit class {unitClass}")
            };
        }
    }
    
    UnitClass inferUnitClassFromType(Type type) {
        UnitClass unitClass;
        if (type.IsAssignableFrom(typeof(Warrior))) {
            unitClass = UnitClass.Warrior;
        } else if (type.IsAssignableFrom(typeof(Ranger))) {
            unitClass = UnitClass.Ranger;
        } else if (type.IsAssignableFrom(typeof(Artillery))) {
            unitClass = UnitClass.Artillery;
        } else {
            throw new ArgumentException($"type {type.Name} is not a known child of the Unit class");
        }
        return unitClass;
    }
    #endregion

    #region unit animator creation
    // ReSharper disable Unity.PerformanceAnalysis
    public Animator create(ArmyType armyType, UnitClass unitClass, int level) {
        var prefab = getUnitSkeletonPrefab(armyType, unitClass, level);
        var animator = diContainer.InstantiatePrefabForComponent<Animator>(prefab);
        return animator;
    }

    GameObject getUnitSkeletonPrefab(ArmyType armyType, UnitClass unitClass, int level) {
        var index = level - 1;
        index += getIndexOffset(unitClass);
        var list = getUnitSkeletonList(armyType);
        return list[index];
    }

    List<GameObject> getUnitSkeletonList(ArmyType armyType) {
        return armyType == ArmyType.Zombie ? zombieSkeletons : alienSkeletons;
    }

    int getIndexOffset(UnitClass unitClass) {
        return unitClass switch {
            UnitClass.Warrior => 0,
            UnitClass.Ranger => 3,
            UnitClass.Artillery => 6,
            _ => throw new ArgumentOutOfRangeException(nameof(unitClass), unitClass, null)
        };
    }
    #endregion
}

public enum ArmyType {
    Zombie,
    Alien,
}

public enum UnitClass {
    Warrior,
    Ranger,
    Artillery,
}
}