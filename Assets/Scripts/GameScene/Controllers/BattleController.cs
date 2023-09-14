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
public class BattleController : MonoBehaviour, ProjectileManager {
    [Inject] GameControllerSettings gameControllerSettings;
    [Inject] GameLogConfig logConfig;
    [Inject] ArenaController arenaController;
    [Inject] ArenaView arenaView;
    [Inject] UnitHealthBars healthBars;
    [Inject] EventBus<GameEvent> eventBus;
    Log log;
    List<Unit> leftArmy;
    List<Unit> rightArmy;
    bool battleBegan;
    float playerTotalHealth;
    float enemyTotalHealth;
    int projectileCount;

    void Awake() {
        log = new(GetType(), logConfig.battleController);
        leftArmy = new();
        rightArmy = new();
        eventBus.subscribe<BattleEvent.UnitDied>(e => onUnitDeath(e as BattleEvent.UnitDied));
        eventBus.subscribe<BattleEvent.DamageTaken>(e => onUnitTakeDamage(e as BattleEvent.DamageTaken));
    }

    public void beginBattle() {
        log.log($"begin battle");
        leftArmy = arenaController.getUnits(true);
        rightArmy = arenaController.getUnits(false);
        var totalHealth = 0f;
        foreach (var unit in leftArmy) {
            unit.enableBattleMode(rightArmy);
            totalHealth += unit.stats.health;
        }
        playerTotalHealth = totalHealth;
        totalHealth = 0f;
        foreach (var unit in rightArmy) {
            unit.enableBattleMode(leftArmy);
            totalHealth += unit.stats.health;
        }
        enemyTotalHealth = totalHealth;
        healthBars.setActive(true);
        healthBars.playerHealth = playerTotalHealth == 0f ? 0f : 1f;
        healthBars.enemyHealth = enemyTotalHealth == 0f ? 0f : 1f;
        battleBegan = true;
    }

    public void reset() {
        foreach (var unit in leftArmy) Destroy(unit.gameObject);
        foreach (var unit in rightArmy) Destroy(unit.gameObject);
        healthBars.setActive(false);
    }

    #region event handlers
    void onUnitTakeDamage(BattleEvent.DamageTaken e) {
        if (e.isPlayer) {
            var currentHealth = healthBars.playerHealth * playerTotalHealth;
            currentHealth -= e.damage;
            healthBars.playerHealth = currentHealth / playerTotalHealth;
        } else {
            var currentHealth = healthBars.enemyHealth * enemyTotalHealth;
            currentHealth -= e.damage;
            healthBars.enemyHealth = currentHealth / enemyTotalHealth;
        }
    }
    
    void onUnitDeath(BattleEvent.UnitDied e) {
        log.log($"onUnitDeath: {e.unit}");
        // if (e.unit.isPlayer) {
            // leftArmy.Remove(e.unit);
        // } else {
            // rightArmy.Remove(e.unit);
        // }
    }
    #endregion

    #region ProjectileManager
    public void onCreate(Projectile projectile) {
        projectileCount++;
    }

    public void onDestroy(Projectile projectile) {
        projectileCount--;
    }
    #endregion

    void Update() {
        if (!battleBegan) return;
        if ((healthBars.playerHealth == 0 || healthBars.enemyHealth == 0) && projectileCount == 0) {
            onBattleEnd();
        }
        keepUnitsInsideArena(leftArmy);
        keepUnitsInsideArena(rightArmy);
    }

    void onBattleEnd() {
        log.log(nameof(onBattleEnd));
        BattleResult result;
        if (healthBars.playerHealth > 0) {
            result = BattleResult.Win;
        } else if (healthBars.enemyHealth > 0) {
            result = BattleResult.Loss;
        } else {
            result = BattleResult.Draw;
        }
        var winningArmy = result == BattleResult.Win ? leftArmy : rightArmy;
        foreach (var unit in winningArmy.Where(unit => unit.isAlive)) {
            unit.disableBattleMode();
        }
        battleBegan = false;
        GameEvent gameEvent = gameControllerSettings.testMode
            ? new BattleEvent.TestBattleEnded(result)
            : new BattleEvent.BattleEnded(result);
        eventBus.sendEvent(gameEvent);
    }

    void keepUnitsInsideArena(List<Unit> units) {
        foreach (var unit in units) {
            if (!unit.isAlive) continue;
            var unitTransform = unit.transform;
            var position = unitTransform.position;
            var scale = unitTransform.localScale;
            var halfWidth = scale.x / 2;
            var halfHeight = scale.y / 2;
            if (position.x - halfWidth < arenaView.bottomLeftPosition.x) { // to the left of the left wall
                unitTransform.setX(arenaView.bottomLeftPosition.x + halfWidth);
                unit.setMoveForward(false);
            } else if (position.x + halfWidth > arenaView.topRightPosition.x) { // to the right of the right wall
                unitTransform.setX(arenaView.topRightPosition.x - halfWidth);
                unit.setMoveForward(false);
            }
            if (position.y - halfHeight < arenaView.bottomLeftPosition.y) { // below the bottom wall
                unitTransform.setY(arenaView.bottomLeftPosition.y + halfHeight);
                unit.setMoveForward(false);
            } else if (position.y + halfHeight > arenaView.topRightPosition.y) { // above the top wall
                unitTransform.setY(arenaView.topRightPosition.y - halfHeight);
                unit.setMoveForward(false);
            }
        }
    }
}

public enum BattleResult {
    Win,
    Loss,
    Draw,
}
}