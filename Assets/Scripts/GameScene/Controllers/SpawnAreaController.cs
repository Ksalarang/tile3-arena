using System.Collections.Generic;
using GameScene.Settings;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;
using Zenject;

namespace GameScene.Controllers {
public class SpawnAreaController : MonoBehaviour {
    [Inject] GameLogConfig logConfig;
    [Inject(Id = ViewId.SpawnArea)] Image spawnArea;
    [Inject] CanvasController canvasController;
    Log log;
    List<GameObject> tiles;
    List<Rect> tileRects;
    Vector2 bottomLeftPosition;
    float horizontalStep;
    float verticalStep;

    void Awake() {
        log = new(GetType(), logConfig.spawnAreaController);
        tiles = new();
        tileRects = new();
        var spawnAreaTransform = spawnArea.transform;
        var canvasScale = canvasController.getCanvasMinScale();
        for (var i = 0; ; i++) {
            var tile = spawnAreaTransform.Find($"tile_{i}");
            if (tile == null) break;
            tiles.Add(tile.gameObject);
            tileRects.Add(tile.GetComponent<RectTransform>().getWorldRect(canvasScale));
        }
    }

    public int getTileCount() => tiles.Count;
    
    public Rect getTileRect(int index) {
        return tileRects[index];
    }
}
}