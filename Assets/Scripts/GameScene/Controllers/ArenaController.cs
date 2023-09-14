using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using GameScene.Events;
using GameScene.Models;
using GameScene.Settings;
using GameScene.Views;
using UnityEngine;
using Utils;
using Zenject;

namespace GameScene.Controllers {
public class ArenaController : MonoBehaviour {
    const int MergeNumber = 3;
    [Inject] GameLogConfig logConfig;
    [Inject] ArenaSettings settings;
    [Inject] UnitAnimationSettings unitAnimationSettings;
    [Inject] ArenaView view;
    [Inject] ActionPointsController actionPointsController;
    [Inject] EventBus<GameEvent> eventBus;
    Log log;
    Army leftArmy;
    Army rightArmy;
    Army currentArmy;
    public bool placingUnit { get; private set; }

    void Awake() {
        log = new(GetType(), logConfig.arenaController);
        leftArmy = new(settings.unitsPerRow, true);
        rightArmy = new(settings.unitsPerRow, false);
    }

    #region unit placement
    public bool placeUnit(Unit unit, bool isLeftArmy = true, Action<bool> action = null) {
        if (placingUnit) {
            log.log("can only place one unit at a time");
            return false;
        }
        var army = getArmy(isLeftArmy);
        if (army.count(unit.unitClass) == settings.unitsPerRow) {
            log.log($"{unit.unitClass} row is full");
            action?.Invoke(false);
            return false;
        }
        if (action == null && !actionPointsController.isEnoughPointsForAction(PrepAction.PlaceUnitOnArena)) {
            log.log("not enough action points");
            return false;
        }
        placeUnitInternal(unit, isLeftArmy, action);
        return true;
    }

    public void placeRemainingUnits(List<Unit> units) {
        log.log($"placeRemainingUnits: {units.Count} left");
        if (placingUnit) {
            var delay = unitAnimationSettings.moveDuration + 0.05f;
            StartCoroutine(Coroutines.delayAction(delay, () => placeRemainingUnits(units)));
            return;
        }
        var first = units.FirstOrDefault();
        if (first != null) {
            placeUnit(first, true, canPlaceAnother => {
                if (canPlaceAnother) {
                    units.Remove(first);
                    eventBus.sendEvent(new UnitAutoPlacementEvent(first));
                } else {
                    units.RemoveAll(u => u.unitClass == first.unitClass);
                }
                placeRemainingUnits(units);
            });
        } else {
            eventBus.sendEvent(new ArenaEvent.FinishPlacingUnits());
        }
    }

    public void placeUnitRandomly(Unit unit, bool isLeftArmy) {
        var army = getArmy(isLeftArmy);
        var unitIndex = army.addAtRandomPosition(unit);
        var x = getArmyRowIndex(unit, isLeftArmy);
        var y = unitIndex;
        log.log($"place {unit} at position ({x},{y})");
        unit.transform.position = view.getPositionForUnit(x, y, isLeftArmy);
    }

    public void placeUnitAtPosition(Unit unit, bool isLeftArmy, int unitIndex) {
        var army = getArmy(isLeftArmy);
        army.addAtPosition(unit, unitIndex);
        var x = getArmyRowIndex(unit, isLeftArmy);
        var y = unitIndex;
        log.log($"place {unit} at position ({x},{y})");
        unit.transform.position = view.getPositionForUnit(x, y, isLeftArmy);
    }

    public bool placeUnitAtFreePosition(Unit unit, bool isLeftArmy) {
        var army = getArmy(isLeftArmy);
        if (army.count(unit.unitClass) == settings.unitsPerRow) {
            log.log($"{unit.unitClass} row is full");
            return false;
        }
        unit.onClick = null;
        var unitIndex = army.addAtFreePosition(unit);
        var x = getArmyRowIndex(unit, isLeftArmy);
        var y = unitIndex;
        log.log($"place {unit} at position ({x},{y})");
        unit.transform.position = view.getPositionForUnit(x, y, isLeftArmy);
        return true;
    }

    public void alignArmy(bool isLeftArmy) {
        var army = getArmy(isLeftArmy);
        alignRow(army, UnitClass.Warrior);
        alignRow(army, UnitClass.Ranger);
        alignRow(army, UnitClass.Artillery);
    }

    void alignRow(Army army, UnitClass unitClass) {
        var row = army.getRow(unitClass);
        var unitCount = army.count(row);
        if (unitCount < settings.unitsPerRow) {
            var verticalOffset = ((settings.unitsPerRow - unitCount) * view.verticalStep) / 2; 
            foreach (var unit in row.Where(u => u is not null)) {
                unit.transform.Translate(0, verticalOffset, 0);
            }
        }
    }

    void placeUnitInternal(Unit unit, bool isLeftArmy, Action<bool> action = null) {
        currentArmy = getArmy(isLeftArmy);
        onPlacingUnit(unit, action == null);
        var unitIndex = currentArmy.addAtRandomPosition(unit);
        var x = getArmyRowIndex(unit, isLeftArmy);
        var y = unitIndex;
        log.log($"place {unit} at position ({x},{y})");
        var position = view.getPositionForUnit(x, y, isLeftArmy);
        var duration = unitAnimationSettings.moveDuration;
        StartCoroutine(Coroutines.moveTo(unit.transform, position, duration, Interpolation.Linear, () => {
            var row = currentArmy.getRow(unit.unitClass);
            if (canMerge(unit, row)) {
                mergeUnits(unit, row, isLeftArmy, () => {
                    unit.levelUp();
                    onUnitPlaced(action);
                });
            } else {
                onUnitPlaced(action);
            }
        }));
        StartCoroutine(Coroutines.scaleTo(unit.transform, view.unitScale, duration));
    }
    
    void onPlacingUnit(Unit unit, bool spendPoints) {
        placingUnit = true;
        unit.onClick = null;
        if (spendPoints) actionPointsController.spendPoints(PrepAction.PlaceUnitOnArena);
    }

    void onUnitPlaced(Action<bool> action) {
        placingUnit = false;
        action?.Invoke(true);
    }

    bool canMerge(Unit unit, Unit[] row) {
        return unit.level < Unit.MaxLevel && row.Count(u => u != null && isUnitsSame(u, unit)) == MergeNumber;
    }
    
    readonly List<Unit> unitsTemp = new();
    readonly List<int> indicesTemp = new();

    void mergeUnits(Unit unit, Unit[] row, bool isLeftArmy, Action action) {
        unitsTemp.Clear();
        indicesTemp.Clear();
        for (var i = 0; i < row.Length; i++) {
            var rowUnit = row[i];
            if (rowUnit != null && isUnitsSame(rowUnit, unit)) {
                unitsTemp.Add(rowUnit);
                indicesTemp.Add(i);
            }
        }
        var index = RandomUtils.nextItem(indicesTemp);
        var position = view.getPositionForUnit(getArmyRowIndex(unit, isLeftArmy), index, isLeftArmy);
        var duration = unitAnimationSettings.moveDuration;
        foreach (var u in unitsTemp) {
            StartCoroutine(Coroutines.moveTo(u.transform, position, duration));
        }
        StartCoroutine(Coroutines.delayAction(duration, () => {
            foreach (var u in unitsTemp) {
                currentArmy.remove(u);
                if (u != unit) Destroy(u.gameObject);
            }
            row[index] = unit;
            action.Invoke();
        }));
    }
    
    int getArmyRowIndex(Unit unit, bool isLeftArmy) {
        if (isLeftArmy) {
                return unit.unitClass switch {
                UnitClass.Warrior => 2,
                UnitClass.Ranger => 1,
                UnitClass.Artillery => 0,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        return unit.unitClass switch {
            UnitClass.Warrior => 0,
            UnitClass.Ranger => 1,
            UnitClass.Artillery => 2,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    bool isUnitsSame(Unit unit1, Unit unit2) => unit1.unitClass == unit2.unitClass && unit1.level == unit2.level;

    Army getArmy(bool isLeft) => isLeft ? leftArmy : rightArmy;
    #endregion
    
    public List<Unit> getUnits(bool isLeftArmy) {
        var army = getArmy(isLeftArmy);
        var list = new List<Unit>();
        var warriors = army.getRow(UnitClass.Warrior);
        var rangers = army.getRow(UnitClass.Ranger);
        var artilleryRow = army.getRow(UnitClass.Artillery);
        foreach (var warrior in warriors.Where(u => u != null)) { 
            list.Add(warrior);
        }
        foreach (var ranger in rangers.Where(u => u != null)) { 
            list.Add(ranger); 
        }
        foreach (var artillery in artilleryRow.Where(u => u != null)) { 
            list.Add(artillery); 
        }
        return list;
    }

    public void reset() {
        leftArmy.clear();
        rightArmy.clear();
    }
}
}