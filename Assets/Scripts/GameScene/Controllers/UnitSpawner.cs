using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using GameScene.Events;
using GameScene.Settings;
using GameScene.Views;
using Services.LevelData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;
using Zenject;
using Unit = GameScene.Models.Unit;

namespace GameScene.Controllers {
public class UnitSpawner : MonoBehaviour {
    [Inject] GameLogConfig logConfig;
    [Inject] UnitSpawnSettings settings;
    [Inject] PrepActionSettings prepActionSettings;
    [Inject] UnitFactory unitFactory;
    [Inject] SpawnAreaController spawnAreaController;
    [Inject] MergeLineController mergeLineController;
    [Inject] ActionPointsController actionPointsController;
    [Inject] ArenaController arenaController;
    [Inject] ArenaView arenaView;
    [Inject(Id = ViewId.RespawnButton)] Button respawnButton;
    [Inject] EventBus<GameEvent> eventBus;
    
    Log log;
    List<Unit> playerUnits;
    List<Unit> enemyUnits;
    GameObject playerUnitContainer;
    GameObject enemyUnitContainer;
    // test
    List<Unit> playerUnitsForTest;
    List<Unit> enemyUnitsForTest;

    void Awake() {
        log = new(GetType(), logConfig.unitSpawner);
        playerUnits = new();
        enemyUnits = new();
        playerUnitsForTest = new();
        enemyUnitsForTest = new();
        playerUnitContainer = new("PlayerUnits");
        enemyUnitContainer = new("EnemyUnits");
        respawnButton.onClick.AddListener(onClickRespawnButton);
        eventBus.subscribe<UnitOnLinePlacementEvent>(e => onUnitPlacedOnLine(e as UnitOnLinePlacementEvent));
    }

    void onUnitPlacedOnLine(UnitOnLinePlacementEvent e) {
        playerUnits.Remove(e.unit);
    }

    public void spawnPlayerUnits() {
        var unitClasses = RandomUtils.generateEnums<UnitClass>(
            spawnAreaController.getTileCount(), 
            settings.warrior, settings.ranger, settings.artillery
        );
        unitClasses.shuffle();
        for (var i = 0; i < spawnAreaController.getTileCount(); i++) {
            var unitClass = unitClasses[i];
            var unit = unitFactory.create(unitClass);
            var tileRect = spawnAreaController.getTileRect(i);
            var unitTransform = unit.transform;
            unitTransform.position = tileRect.center;
            unitTransform.localScale = tileRect.size * settings.unitSizeModifier;
            unitTransform.SetParent(playerUnitContainer.transform);
            unit.onClick = u => mergeLineController.placeUnit(u);
            playerUnits.Add(unit);
        }
    }

    public void respawnPlayerUnits() {
        removeSpawnedPlayerUnits();
        spawnPlayerUnits();
    }

    public void spawnEnemyUnits(IList<UnitData> formation) {
        enemyUnits.Clear();
        var index = 0;
        var prevUnitClass = UnitClass.Warrior;
        foreach (var unitData in formation) {
            if (prevUnitClass != unitData.unitClass) {
                index = 0;
                prevUnitClass = unitData.unitClass;
            }
            var unit = createUnit(unitData.unitClass, false);
            for (var i = 0; i < unitData.level - 1; i++) {
                unit.levelUp();
            }
            arenaController.placeUnitAtPosition(unit, false, index);
            enemyUnits.Add(unit);
            index++;
        }
        arenaController.alignArmy(false);
    }

    public IEnumerable<Unit> getPlayerUnits() => playerUnits;

    public IEnumerable<Unit> getTestUnits(bool isPlayer) => isPlayer ? playerUnitsForTest : enemyUnitsForTest;

    #region test mode
    public void createUnitsForTest(bool isPlayer) {
        var unitClasses = new[] { UnitClass.Warrior, UnitClass.Ranger, UnitClass.Artillery };
        var index = 0;
        for (var i = 0; i < 3; i++) {
            foreach (var unitClass in unitClasses) {
                if ((index + 1) % 4 == 0) index++;
                createUnitForTest(unitClass, isPlayer, index, i);
                index++;
            }
        }
    }

    public void removeUnitsForTest(bool isPlayer) {
        var units = getUnitsForTest(isPlayer);
        foreach (var unit in units) Destroy(unit.gameObject);
        units.Clear();
    }

    Unit createUnitForTest(UnitClass unitClass, bool isPlayer, int index, int levelIndex) {
        var unit = unitFactory.create(unitClass, isPlayer);
        getUnitsForTest(isPlayer).Add(unit);
        for (var i = 0; i < levelIndex; i++) unit.levelUp();
        var tileRect = spawnAreaController.getTileRect(index);
        var unitTransform = unit.transform;
        unitTransform.position = tileRect.center;
        unitTransform.localScale = tileRect.size * settings.unitSizeModifier;
        unit.onClick = u => {
            var placed = arenaController.placeUnitAtFreePosition(u, isPlayer);
            if (placed) {
                u.transform.localScale = arenaView.unitScale;
                getUnitsForTest(isPlayer).Remove(u);
                createUnitForTest(unitClass, isPlayer, index, levelIndex);
            }
        };
        return unit;
    }

    List<Unit> getUnitsForTest(bool isPlayer) => isPlayer ? playerUnitsForTest : enemyUnitsForTest;
    #endregion

    #region unit generation
    [Obsolete]
    public void spawnUnitsInArena(bool isPlayer) {
        var units = isPlayer ? playerUnits : enemyUnits;
        units.Clear();
        var actionPoints = prepActionSettings.actionPointsPerRound;
        // var unitsTotal = Mathf.RoundToInt(actionPoints * settings.enemyUnitStrengthFactor);
        var unitsTotal = Mathf.RoundToInt(actionPoints * 0.8f);
        if (actionPoints > 0 && unitsTotal == 0) unitsTotal = 1;
        var unitClasses = RandomUtils.generateEnums<UnitClass>(unitsTotal, 0.33f, 0.33f, 0.33f);
        var warriorFirstMergeData = mergeUnitClasses(unitClasses, UnitClass.Warrior);
        var rangerFirstMergeData = mergeUnitClasses(unitClasses, UnitClass.Ranger);
        var artilleryFirstMergeData = mergeUnitClasses(unitClasses, UnitClass.Artillery);
        var warriorSecondMergeData = mergeUnitClasses(unitClasses, UnitClass.Warrior, warriorFirstMergeData.x);
        var rangerSecondMergeData = mergeUnitClasses(unitClasses, UnitClass.Ranger, rangerFirstMergeData.x);
        var artillerySecondMergeData = mergeUnitClasses(unitClasses, UnitClass.Artillery, artilleryFirstMergeData.x);
        foreach (var unitClass in unitClasses) {
            var unit = createUnit(unitClass, isPlayer);
            units.Add(unit);
        }
        levelUpUnits(units, UnitClass.Warrior, warriorSecondMergeData);
        levelUpUnits(units, UnitClass.Ranger, rangerSecondMergeData);
        levelUpUnits(units, UnitClass.Artillery, artillerySecondMergeData);
        foreach (var unit in units) {
            arenaController.placeUnitRandomly(unit, isPlayer);
        }
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

    void levelUpUnits(List<Unit> list, UnitClass unitClass, Vector2Int mergeData) {
        var secondLevelWarriorCount = mergeData.y;
        var thirdLevelWarriorCount = mergeData.x;
        foreach (var warrior in list.Where(u => u.unitClass == unitClass)) {
            if (secondLevelWarriorCount > 0) {
                warrior.levelUp();
                secondLevelWarriorCount--;
            } else if (thirdLevelWarriorCount > 0) {
                warrior.levelUp();
                warrior.levelUp();
                thirdLevelWarriorCount--;
            } else break;
        }
    }
    #endregion

    void onClickRespawnButton() {
        if (!actionPointsController.isEnoughPointsForAction(PrepAction.RespawnUnitsInSpawnArea)) {
            log.log("respawnUnits: not enough action points");
            return;
        }
        respawnPlayerUnits();
        actionPointsController.spendPoints(PrepAction.RespawnUnitsInSpawnArea);
    }

    void removeSpawnedPlayerUnits() {
        foreach (var unit in playerUnits.Where(unit => !unit.IsDestroyed())) {
            Destroy(unit.gameObject);
        }
        playerUnits.Clear();
    }

    Unit createUnit(UnitClass unitClass, bool isPlayer) {
        var unit = unitFactory.create(unitClass, false);
        var unitTransform = unit.transform;
        var container = isPlayer ? playerUnitContainer : enemyUnitContainer;
        unitTransform.SetParent(container.transform);
        unitTransform.localScale = arenaView.unitScale;
        return unit;
    }
}
}