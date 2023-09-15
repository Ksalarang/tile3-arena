using System;
using System.Collections.Generic;
using GameScene.Contracts;
using GameScene.Models;
using GameScene.Settings;
using GameScreen;
using UnityEngine;
using Zenject;

namespace GameScene.Controllers {
public class UnitFactory : MonoBehaviour {
    [Inject(Id = PrefabId.Warrior)] GameObject warriorPrefab;
    [Inject(Id = PrefabId.Ranger)] GameObject rangerPrefab;
    [Inject(Id = PrefabId.Artillery)] GameObject artilleryPrefab;
    
    [Inject] DiContainer diContainer;
    [Inject] UnitStatsSettings unitStats;

    #region unit creation
    // ReSharper disable Unity.PerformanceAnalysis
    public Unit create(UnitClass unitClass, bool isPlayer = true) {
        var unit = diContainer.InstantiatePrefabForComponent<Unit>(getPrefabForUnitClass(unitClass));
        unit.setStats(getUnitStatsForClass(unitClass));
        unit.setSide(isPlayer);
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