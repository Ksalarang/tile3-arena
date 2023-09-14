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
using Utils.Extensions;
using Zenject;

namespace GameScene.Controllers {
public class MergeLineController : MonoBehaviour {
    const int MergeNumber = 3;
    [Inject] GameLogConfig logConfig;
    [Inject] UnitAnimationSettings unitAnimationSettings;
    [Inject] MergeLineSettings settings;
    [Inject] MergeLineView view;
    [Inject] ArenaController arenaController;
    [Inject] ActionPointsController actionPointsController;
    [Inject] EventBus<GameEvent> eventBus;
    Log log;
    List<Unit> mergeList;
    public bool placingUnit { get; private set; }

    void Awake() {
        log = new(GetType(), logConfig.mergeLineController);
        mergeList = new();
        eventBus.subscribe<UnitAutoPlacementEvent>(e => onUnitAutoPlaced(e as UnitAutoPlacementEvent));
    }

    void onUnitAutoPlaced(UnitAutoPlacementEvent e) {
        log.log($"removing {e.unit} from mergeList");
        mergeList.Remove(e.unit);
    }

    public void placeUnit(Unit unit) {
        placeUnit(unit, -1);
    }

    public List<Unit> getMergeListCopy() => new(mergeList);

    public IEnumerable<Unit> getUnits() => mergeList;

    public void reset() {
        foreach (var unit in mergeList) {
            Destroy(unit.gameObject);
        }
        mergeList.Clear();
    }

    void placeUnit(Unit unit, int index) {
        if (placingUnit) {
            log.log("can only place one unit at a time");
            return;
        }
        if (mergeList.Count == view.cellCount) {
            log.log($"merge list is full");
            return;
        }
        var firstTime = index == -1;
        if (firstTime && !actionPointsController.isEnoughPointsForAction(PrepAction.PlaceUnitOnMergeLine)) {
            log.log("not enough action points");
            return;
        }
        onPlacingUnit(unit, firstTime);
        index = firstTime ? getIndexForUnit(unit) : index;
        log.log($"place {unit} with index {index}");
        var endPosition = view.getCellPositionForIndex(index);
        var duration = firstTime ? unitAnimationSettings.moveDuration : 0f;
        StartCoroutine(Coroutines.moveTo(unit.transform, endPosition, duration, Interpolation.Linear, () => {
            if (firstTime) mergeList.Insert(index, unit);
            if (canMerge(unit)) {
                mergeUnits(unit, index, mergedUnit => {
                    onUnitPlaced(mergedUnit);
                    placeUnit(mergedUnit, index - 2);
                });
                shiftUnits(index + 1, false, 2);
            } else {
                onUnitPlaced(unit);
            }
        }));
        if (firstTime) {
            shiftUnits(index, true, 1);
            var endScale = view.getCellSizeForIndex(index).getMin() * settings.unitSizeModifier;
            StartCoroutine(Coroutines.scaleTo(unit.transform, endScale, duration));
        }
    }

    void onPlacingUnit(Unit unit, bool firstTime) {
        placingUnit = true;
        unit.onClick = null;
        if (firstTime) {
            eventBus.sendEvent(new UnitOnLinePlacementEvent(unit));
            actionPointsController.spendPoints(PrepAction.PlaceUnitOnMergeLine);
        }
    }

    void onUnitPlaced(Unit placedUnit) {
        placingUnit = false;
        placedUnit.onClick = unit => {
            var canPlaceUnit = arenaController.placeUnit(unit);
            if (canPlaceUnit) {
                var index = mergeList.IndexOf(unit);
                mergeList.Remove(unit);
                shiftUnits(index, false, 1);
            }
        };
    }

    int getIndexForUnit(Unit unit) {
        var biggerLevelUnitIndex = -1;
        var sameLevelUnitIndex = -1;
        for (var i = 0; i < mergeList.Count; i++) {
            var otherUnit = mergeList[i];
            if (unit.unitClass == otherUnit.unitClass) {
                if (unit.level < otherUnit.level) {
                    biggerLevelUnitIndex = i;
                } else if (unit.level > otherUnit.level) {
                    return i;
                } else {
                    sameLevelUnitIndex = i;
                }
            }
        }
        if (sameLevelUnitIndex > -1) return sameLevelUnitIndex + 1;
        return biggerLevelUnitIndex > -1 ? biggerLevelUnitIndex + 1 : mergeList.Count;
    }

    void shiftUnits(int fromIndex, bool right, int shiftAmount) {
        var deltaX = shiftAmount * view.getStep();
        if (!right) deltaX = -deltaX;
        for (var i = fromIndex; i < mergeList.Count; i++) {
            var unit = mergeList[i];
            var unitTransform = unit.transform;
            var startPosition = unitTransform.position;
            var endPosition = new Vector3(startPosition.x + deltaX, startPosition.y, startPosition.z);
            StartCoroutine(Coroutines.moveTo(unitTransform, endPosition, unitAnimationSettings.moveDuration));
        }
    }

    bool canMerge(Unit unit) {
        return unit.level < Unit.MaxLevel && countSameUnits(unit) == MergeNumber;
    }

    int countSameUnits(Unit unit) => mergeList.Count(u => u.unitClass == unit.unitClass && u.level == unit.level);

    void mergeUnits(Unit lastUnit, int lastUnitIndex, Action<Unit> action) {
        var firstUnit = mergeList[lastUnitIndex - 2];
        var secondUnit = mergeList[lastUnitIndex - 1];
        var endPosition = firstUnit.transform.position;
        var duration = unitAnimationSettings.moveDuration;
        StartCoroutine(Coroutines.moveTo(secondUnit.transform, endPosition, duration));
        StartCoroutine(Coroutines.moveTo(lastUnit.transform, endPosition, duration));
        StartCoroutine(Coroutines.delayAction(duration, () => {
            firstUnit.levelUp();
            removeUnit(secondUnit);
            removeUnit(lastUnit);
            action.Invoke(firstUnit);
        }));
    }

    void removeUnit(Unit unit) {
        mergeList.Remove(unit);
        Destroy(unit.gameObject);
    }
}
}