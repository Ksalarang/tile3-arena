using System;
using System.Collections.Generic;
using System.Linq;
using GameScene.Controllers;
using ModestTree;
using Utils;
using Log = Utils.Log;

namespace GameScene.Models {
public class Army {
    readonly Unit[] artilleryRow;
    readonly Unit[] rangerRow;
    readonly Unit[] warriorRow;
    readonly List<int> indicesTemp = new();
    readonly bool isLeft;

    public Army(int unitsPerRow, bool isLeft) {
        artilleryRow = new Unit[unitsPerRow];
        rangerRow = new Unit[unitsPerRow];
        warriorRow = new Unit[unitsPerRow];
        this.isLeft = isLeft;
    }

    public int addAtRandomPosition(Unit unit) {
        indicesTemp.Clear();
        var row = getRow(unit.unitClass);
        for (var i = 0; i < row.Length; i++) {
            var u = row[i];
            if (u is null) {
                indicesTemp.Add(i);
            }
        }
        if (indicesTemp.Count > 0) {
            var index = RandomUtils.nextItem(indicesTemp);
            row[index] = unit;
            return index;
        }
        return -1;
    }

    public void addAtPosition(Unit unit, int unitIndex) {
        var row = getRow(unit.unitClass);
        if (row[unitIndex] is null) {
            row[unitIndex] = unit;
        } else {
            var tag = isLeft ? "LeftArmy" : "RightArmy";
            Log.error(tag, $"{unit.unitClass} row at index {unitIndex} is taken");
        }
    }

    public int addAtFreePosition(Unit unit) {
        var row = getRow(unit.unitClass);
        for (var i = 0; i < row.Length; i++) {
            if (row[i] is null) {
                row[i] = unit;
                return i;
            }
        }
        var tag = isLeft ? "LeftArmy" : "RightArmy";
        Log.warn(tag, $"all positions are taken for {unit}");
        return -1;
    }

    public void remove(Unit unit) {
        var row = getRow(unit.unitClass);
        var index = row.IndexOf(unit);
        if (index >= 0) row[index] = null;
    }

    public void clear() {
        for (var i = 0; i < artilleryRow.Length; i++) {
            artilleryRow[i] = null;
            rangerRow[i] = null;
            warriorRow[i] = null;
        }
    }

    public int count(UnitClass unitClass) {
        return count(getRow(unitClass));
    }

    public int count(Unit[] row) {
        return row.Count(unit => unit != null);
    }

    public Unit[] getRow(UnitClass unitClass) {
        return unitClass switch {
            UnitClass.Warrior => warriorRow,
            UnitClass.Ranger => rangerRow,
            UnitClass.Artillery => artilleryRow,
            _ => throw new ArgumentOutOfRangeException(nameof(unitClass), unitClass, null)
        };
    }
}
}