using System.Collections.Generic;
using GameScene.Controllers;
using GameScreen;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;
using Zenject;

namespace GameScene.Views {
public class MergeLineView : MonoBehaviour {
    [field: SerializeField] public int cellCount { get; private set; }
    [SerializeField] float distanceBetweenCells;
    
    [Inject(Id = PrefabId.UnitCell)] GameObject unitCellPrefab;
    [Inject(Id = ViewId.SpawnArea)] Image spawnArea;
    [Inject(Id = ViewId.MergeArea)] Image mergeArea;
    [Inject] new Camera camera;
    [Inject] CanvasController canvasController;
    
    List<GameObject> unitCells;
    List<Rect> unitCellRects;
    GameObject cellContainer;
    Vector3 firstCellPosition;
    float step;

    #region init
    void Awake() {
        unitCells = new();
        unitCellRects = new();
        cellContainer = new GameObject("UnitCells");

        var canvasScale = canvasController.getCanvasMinScale();
        var mergeAreaTransform = mergeArea.transform;
        for (var i = 0; ; i++) {
            var cell = mergeAreaTransform.Find($"bg_merge_{i}");
            if (cell == null) break;
            unitCells.Add(cell.gameObject);
            unitCellRects.Add(cell.GetComponent<RectTransform>().getWorldRect(canvasScale));
        }
        step = unitCellRects[1].position.x - unitCellRects[0].position.x;
    }

    void createCells() {
        for (var i = 0; i < cellCount; i++) {
            var unitCell = Instantiate(unitCellPrefab, cellContainer.transform, true);
            unitCells.Add(unitCell);
        }
    }

    void initCells() {
        var screenLeftX = camera.getBottomLeft().x;
        var screenRightX = camera.getBottomRight().x;
        var screenWidth = screenRightX - screenLeftX;
        var totalDistanceBetweenCells = cellCount * distanceBetweenCells + distanceBetweenCells;
        var cellSize = (screenWidth - totalDistanceBetweenCells) / cellCount;
        firstCellPosition = new Vector3(
            screenLeftX + distanceBetweenCells + cellSize / 2,
            spawnArea.transform.getTopY() + distanceBetweenCells + cellSize / 2
        );
        step = cellSize + distanceBetweenCells;
        for (var i = 0; i < unitCells.Count; i++) {
            var cell = unitCells[i].transform;
            cell.position = new Vector3(
                firstCellPosition.x + i * step,
                firstCellPosition.y
            );
            cell.localScale = new Vector3(cellSize, cellSize);
        }
    }
    #endregion

    public Vector3 getCellPositionForIndex(int index) {
        return unitCellRects[index].center;
    }

    public Vector3 getCellSizeForIndex(int index) {
        return unitCellRects[index].size;
    }

    public float getStep() => step;
}
}