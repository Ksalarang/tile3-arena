using GameScene.Controllers;
using GameScene.Settings;
using GameScene.Views;
using GameScene.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

// ReSharper disable All

namespace GameScene {
public class GameInstaller : MonoInstaller {
    [Header("Controllers")]
    [SerializeField] GameController gameController;
    [SerializeField] UnitFactory unitFactory;
    [SerializeField] SpawnAreaController spawnAreaController;
    [SerializeField] UnitSpawner unitSpawner;
    [SerializeField] MergeLineController mergeLineController;
    [SerializeField] ActionPointsController actionPointsController;
    [SerializeField] ArenaController arenaController;
    [SerializeField] BattleController battleController;
    [SerializeField] ProjectileFactory projectileFactory;
    [SerializeField] CanvasController canvasController;
    [Header("Views")] 
    [SerializeField] MergeLineView mergeLineView;
    [SerializeField] Image spawnArea;
    [SerializeField] Button respawnButton;
    [SerializeField] Image mergeArea;
    [SerializeField] TMP_Text actionPointsLabel;
    [SerializeField] RectTransform arena;
    [SerializeField] ArenaView arenaView;
    [SerializeField] Slider playerHealthBar;
    [SerializeField] Slider enemyHealthBar;
    [SerializeField] UnitHealthBars healthBars;
    // [SerializeField] Button nextButtonForTest;
    // [SerializeField] Button cheatButton;
    [SerializeField] UpHudView upHudView;
    [Header("Windows")]
    [SerializeField] VictoryWindow victoryWindow;
    [SerializeField] DefeatWindow defeatWindow;
    [Header("Misc")]
    [SerializeField] new Camera camera;
    [SerializeField] CanvasScaler canvasScaler;
    [SerializeField] GameSettings gameSettings;

    public override void InstallBindings() {
        // game controller
        bindWithInterfaces(gameController);
        bind(gameController.eventBus);
        bind(gameController.settings);
        bind(gameController.logConfig);
        Container.BindInterfacesAndSelfTo<GameModel>().AsSingle();
        // controllers
        bindWithInterfaces(unitFactory);
        bind(unitSpawner);
        bind(spawnAreaController);
        bind(mergeLineController);
        bind(actionPointsController);
        bind(arenaController);
        bindWithInterfaces(battleController);
        bind(projectileFactory);
        bind(canvasController);
        // windows
        bindWithInterfaces(victoryWindow);
        bindWithInterfaces(defeatWindow);
        // views
        bind(mergeLineView);
        bind(spawnArea, ViewId.SpawnArea);
        bind(respawnButton, ViewId.RespawnButton);
        bind(mergeArea, ViewId.MergeArea);
        bind(actionPointsLabel, ViewId.ActionPointsLabel);
        bind(arena, ViewId.Arena);
        bindWithInterfaces(arenaView);
        bind(playerHealthBar, ViewId.PlayerHealthBar);
        bind(enemyHealthBar, ViewId.EnemyHealthBar);
        bind(healthBars);
        // bind(nextButtonForTest, ViewId.NextButtonForTest);
        // bind(cheatButton, ViewId.CheatButton);
        bind(upHudView);
        // settings
        bind(gameSettings);
        bind(gameSettings.unitAnimation);
        bind(gameSettings.prepActions);
        bind(gameSettings.arena);
        bind(gameSettings.mergeLine);
        bind(gameSettings.unitSpawn);
        bind(gameSettings.unitStats);
        bind(gameSettings.projectiles);
        // misc
        bind(camera);
        bind(canvasScaler);
        bind(new Log("Unit", gameController.logConfig.unit));
    }

    void bind<T>(T instance) {
        Container.BindInstance(instance);
    }

    void bind<T>(T instance, object id) {
        Container.Bind<T>().WithId(id).FromInstance(instance);
    }
    
    void bindWithInterfaces<T>(T instance) {
        Container.BindInterfacesAndSelfTo<T>().FromInstance(instance);
    }
}

public enum ViewId {
    SpawnArea,
    RespawnButton,
    MergeArea,
    ActionPointsLabel,
    Arena,
    PlayerHealthBar,
    EnemyHealthBar,
    NextButtonForTest,
    CheatButton,
}
}