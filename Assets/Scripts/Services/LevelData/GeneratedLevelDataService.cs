using System;
using System.Collections.Generic;
using System.Linq;
using GameScene.Controllers;
using UnityEngine;
using Utils;

namespace Services.LevelData {
public class GeneratedLevelDataService : LevelDataService {
    const int UnitsInRow = 4;
    readonly int[] actionPointsArray = { 5, 5, 7, 7, 10, 10, 12, 12, 15, 15, };
    readonly Log log = new(typeof(GeneratedLevelDataService), false);
    readonly List<LevelData> levelDataList = new();

    public GeneratedLevelDataService() {
        generateLevelData();
    }

    void generateLevelData() {
        for (var i = 0; i < actionPointsArray.Length; i++) {
            log.log($"level {i + 1}");
            var actionPoints = actionPointsArray[i];
            var unitDataList = createAndMergeUnits(actionPoints);
            var levelData = new LevelData {
                levelNumber = i + 1,
                actionPoints = actionPoints,
                enemyFormation = new List<UnitData>(),
            };
            var currentUnitClass = UnitClass.Warrior;
            var unitsInRow = 0;
            foreach (var unitData in unitDataList) {
                if (currentUnitClass != unitData.unitClass) {
                    currentUnitClass = unitData.unitClass;
                    unitsInRow = 0;
                }
                if (unitsInRow < UnitsInRow) {
                    levelData.enemyFormation.Add(unitData);
                    log.log(unitData);
                    unitsInRow++;
                } else {
                    log.error($"cannot add more {unitData.unitClass} units to a row than the specified limit: {UnitsInRow}");
                }
            }
            levelDataList.Add(levelData);
        }
    }

    List<UnitData> createAndMergeUnits(int unitCount) {
        var unitClasses = RandomUtils.generateEnums<UnitClass>(unitCount, 0.33f, 0.33f, 0.33f);
        var warriorFirstMergeData = mergeUnitClasses(unitClasses, UnitClass.Warrior);
        var rangerFirstMergeData = mergeUnitClasses(unitClasses, UnitClass.Ranger);
        var artilleryFirstMergeData = mergeUnitClasses(unitClasses, UnitClass.Artillery);
        var warriorSecondMergeData = mergeUnitClasses(unitClasses, UnitClass.Warrior, warriorFirstMergeData.x);
        var rangerSecondMergeData = mergeUnitClasses(unitClasses, UnitClass.Ranger, rangerFirstMergeData.x);
        var artillerySecondMergeData = mergeUnitClasses(unitClasses, UnitClass.Artillery, artilleryFirstMergeData.x);
        var unitDataList = unitClasses
            .Select(unitClass => new UnitData { unitClass = unitClass, level = 1, rowIndex = getUnitRowIndex(unitClass), })
            .ToList();
        levelUpUnits(unitDataList, UnitClass.Warrior, warriorSecondMergeData);
        levelUpUnits(unitDataList, UnitClass.Ranger, rangerSecondMergeData);
        levelUpUnits(unitDataList, UnitClass.Artillery, artillerySecondMergeData);
        unitDataList.Sort((unitData1, unitData2) => unitData1.rowIndex - unitData2.rowIndex);
        return unitDataList;
    }

    Vector2Int mergeUnitClasses(List<UnitClass> list, UnitClass unitClass, int count = -1) {
        if (count == -1) count = list.Count(uc => uc == unitClass);
        var mergeCount = count / 3;
        var remainder = count % 3;
        for (var i = 0; i < 2 * mergeCount; i++) {
            list.Remove(unitClass);
        }
        return new Vector2Int(mergeCount, remainder);
    }
    
    void levelUpUnits(List<UnitData> list, UnitClass unitClass, Vector2Int mergeData) {
        var secondLevelUnitCount = mergeData.y;
        var thirdLevelUnitCount = mergeData.x;
        foreach (var unitData in list.Where(ud => ud.unitClass == unitClass)) {
            if (secondLevelUnitCount > 0) {
                unitData.level = 2;
                secondLevelUnitCount--;
            } else if (thirdLevelUnitCount > 0) {
                unitData.level = 3;
                thirdLevelUnitCount--;
            } else break;
        }
    }
    
    int getUnitRowIndex(UnitClass unitClass) {
        return unitClass switch {
            UnitClass.Warrior => 0,
            UnitClass.Ranger => 1,
            UnitClass.Artillery => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(unitClass), unitClass, null)
        };
    }

    public IList<LevelData> getLevelDataList() => levelDataList;

    public bool hasActionPointsData() => true;
}
}