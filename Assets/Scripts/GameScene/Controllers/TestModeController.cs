using System;
using Events;
using GameScene.Events;
using GameScene.Settings;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace GameScene.Controllers {
public class TestModeController : MonoBehaviour {
    [Inject] GameControllerSettings settings;
    [Inject] UnitSpawner unitSpawner;
    [Inject] ArenaController arenaController;
    [Inject] BattleController battleController;
    [Inject] EventBus<GameEvent> eventBus;
    [Inject(Id = ViewId.NextButtonForTest)] Button nextButton;
    [Inject(Id = ViewId.RespawnButton)] Button respawnButton;

    Log log;

    void Awake() {
        log = new(GetType(), settings.testMode);
        eventBus.subscribe<BattleEvent.TestBattleEnded>(e => onBattleEnded(e as BattleEvent.TestBattleEnded));
    }

    void Start() {
        if (settings.testMode) {
            log.log($"start test mode");
            prepareTestMode();
            respawnButton.onClick.RemoveAllListeners();
        }
    }

    void prepareTestMode() {
        log.log($"spawn player units");
        unitSpawner.createUnitsForTest(true);
        nextButton.gameObject.SetActive(true);
        nextButton.onClick.AddListener(() => {
            log.log("spawn enemy units");
            unitSpawner.removeUnitsForTest(true);
            unitSpawner.createUnitsForTest(false);
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => {
                log.log("begin battle in test mode");
                unitSpawner.removeUnitsForTest(false);
                arenaController.alignArmy(true);
                arenaController.alignArmy(false);
                battleController.beginBattle();
                nextButton.onClick.RemoveAllListeners();
            });
        });
    }

    void onBattleEnded(BattleEvent.TestBattleEnded e) {
        log.log($"battle ended, result: {e.result}");
        arenaController.reset();
        battleController.reset();
        prepareTestMode();
    }
}
}