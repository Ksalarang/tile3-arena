using Events;
using GameScene.Controllers;
using GameScene.Events;
using GameScene.Settings;
using GameScene.Views;
using GameScene.Windows;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace GameScene {
public class GameController : MonoBehaviour, VictoryWindow.Manager, DefeatWindow.Manager {
    [field: SerializeField] public GameControllerSettings settings { get; private set; }
    [field: SerializeField] public GameLogConfig logConfig { get; private set; }

    [Inject] GameModel model;
    [Inject] UnitAnimationSettings unitAnimationSettings;
    [Inject] UnitSpawner unitSpawner;
    [Inject] MergeLineController mergeLineController;
    [Inject] ArenaController arenaController;
    [Inject] ActionPointsController actionPointsController;
    [Inject] BattleController battleController;
    [Inject] VictoryWindow victoryWindow;
    [Inject] DefeatWindow defeatWindow;
    [Inject] UpHudView upHudView;

    Log log;
    
    public readonly EventBus<GameEvent> eventBus = new();

    void Awake() {
        log = new(GetType(), logConfig.gameController);
        eventBus.subscribe<ActionPointsDepletedEvent>(e => onActionPointsDepleted());
        eventBus.subscribe<ArenaEvent.FinishPlacingUnits>(e => onFinishPlacingUnits());
        eventBus.subscribe<BattleEvent.BattleEnded>(e => onBattleEnd(e as BattleEvent.BattleEnded));
        Application.targetFrameRate = 60;
    }

    void Start() {
        if (!settings.testMode) {
            onStartLevel();
        }
    }
    
    #region event handlers
    void onActionPointsDepleted() {
        log.log($"onActionPointsDepleted");
        placeRemainingUnits();
    }

    void placeRemainingUnits() {
        if (mergeLineController.placingUnit || arenaController.placingUnit) {
            var delay = unitAnimationSettings.moveDuration + 0.05f;
            StartCoroutine(Coroutines.delayAction(delay, placeRemainingUnits));
            return;
        }
        arenaController.placeRemainingUnits(mergeLineController.getMergeListCopy());
    }

    void onFinishPlacingUnits() {
        log.log("onFinishPlacingUnits");
        battleController.beginBattle();
    }

    void onBattleEnd(BattleEvent.BattleEnded e) {
        log.log($"battle ended: {e.result}");
        if (e.result == BattleResult.Win) {
            victoryWindow.animateShow();
        } else {
            defeatWindow.animateShow();
        }
    }
    #endregion
    

    #region window management
    #region victory window
    public void onClickContinue() {
        var hasNext = model.nextLevel();
        if (!hasNext) model.reset();
        onStartLevel();
    }

    public void onClickExit() {
        log.log("onClickExit");
        Application.Quit();
    }
    #endregion

    #region defeat window
    public void onClickRetry() {
        onStartLevel();
    }
    #endregion
    #endregion

    void onStartLevel() {
        log.log($"start level {model.currentLevel.number}");
        arenaController.reset();
        unitSpawner.respawnPlayerUnits();
        unitSpawner.spawnEnemyUnits(model.currentLevel.enemyFormation);
        mergeLineController.reset();
        battleController.reset();
        actionPointsController.actionPoints = model.currentLevel.actionPoints;
        upHudView.setRoundLabelText($"Round {model.currentLevel.number}");
    }
}
}