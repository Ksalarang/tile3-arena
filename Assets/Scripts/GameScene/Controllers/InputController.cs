using System.Collections.Generic;
using GameScene.Models;
using GameScene.Settings;
using UnityEngine;
using Zenject;

namespace GameScene.Controllers {
public class InputController : MonoBehaviour {
    [Inject] GameControllerSettings gameControllerSettings;
    [Inject] UnitSpawner unitSpawner;
    [Inject] MergeLineController mergeLineController;
    [Inject] new Camera camera;

    bool isMobile;
    IEnumerable<Unit> unitsInSpawnArea;
    IEnumerable<Unit> unitsInMergeLine;
    IEnumerable<Unit> testPlayerUnitsInSpawnArea;
    IEnumerable<Unit> testEnemyUnitsInSpawnArea;

    void Awake() {
        isMobile = Application.isMobilePlatform;
    }

    void Start() {
        unitsInSpawnArea = unitSpawner.getPlayerUnits();
        unitsInMergeLine = mergeLineController.getUnits();
        if (gameControllerSettings.testMode) {
            testPlayerUnitsInSpawnArea = unitSpawner.getTestUnits(true);
            testEnemyUnitsInSpawnArea = unitSpawner.getTestUnits(false);
        }
    }

    void Update() {
        Vector3 position;
        if (isMobile) {
            if (Input.touchCount == 0) return;
            position = camera.ScreenToWorldPoint(Input.GetTouch(0).position);
        } else {
            if (!Input.GetMouseButtonDown(0)) return;
            position = camera.ScreenToWorldPoint(Input.mousePosition);
        }
        var overlappedCollider = Physics2D.OverlapPoint(position);
        if (overlappedCollider is null) return;
        foreach (var unit in unitsInSpawnArea) {
            if (unit.collider == overlappedCollider) {
                unit.onClick?.Invoke(unit);
                return;
            }
        }
        foreach (var unit in unitsInMergeLine) {
            if (unit.collider == overlappedCollider) {
                unit.onClick?.Invoke(unit);
                return;
            }
        }
        if (!gameControllerSettings.testMode) return;
        foreach (var unit in testPlayerUnitsInSpawnArea) {
            if (unit.collider == overlappedCollider) {
                unit.onClick?.Invoke(unit);
                return;
            }
        }
        foreach (var unit in testEnemyUnitsInSpawnArea) {
            if (unit.collider == overlappedCollider) {
                unit.onClick?.Invoke(unit);
                return;
            }
        }
    }
}
}