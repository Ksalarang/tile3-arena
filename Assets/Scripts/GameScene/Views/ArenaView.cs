using GameScene.Contracts;
using GameScene.Controllers;
using GameScene.Settings;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Zenject;

namespace GameScene.Views {
public class ArenaView : MonoBehaviour, UnitMeasureProvider, ArenaBordersProvider {
    const float UnitHeightToWidthRatio = 1.8f;
    [Inject] ArenaSettings settings;
    [Inject(Id = ViewId.Arena)] RectTransform arenaView;
    [Inject] CanvasController canvasController;
    
    public Vector2 unitScale { get; private set; }
    Log log;
    public Vector2 bottomLeftPosition { get; private set; }
    public Vector2 topRightPosition { get; private set; }
    Vector2 leftHalfBottomLeftPosition;
    Vector2 rightHalfBottomLeftPosition;
    float horizontalStep;
    public float verticalStep { get; private set;  }

    void Awake() {
        log = new(GetType());
        bottomLeftPosition = getArenaBottomLeft();
        topRightPosition = getArenaTopRight();
        var arenaSize = new Vector2(
            topRightPosition.x - bottomLeftPosition.x,
            topRightPosition.y - bottomLeftPosition.y
        );
        var arenaTopCenter = new Vector2(bottomLeftPosition.x + arenaSize.x / 2, topRightPosition.y);
        var arenaBottomCenter = new Vector2(bottomLeftPosition.x + arenaSize.x / 2, bottomLeftPosition.y);
        var unitWidth = arenaSize.x / settings.unitWidthReductionFactor;
        unitScale = new Vector2(unitWidth, unitWidth);
        horizontalStep = unitScale.x + settings.distanceBetweenUnits.x;
        verticalStep = unitScale.y * UnitHeightToWidthRatio + settings.distanceBetweenUnits.y;
        var leftHalfCenter = bottomLeftPosition.getMidPoint(arenaTopCenter);
        leftHalfBottomLeftPosition = getBottomLeftPositionForLeftArmy(bottomLeftPosition, leftHalfCenter);
        var rightHalfCenter = arenaBottomCenter.getMidPoint(topRightPosition);
        rightHalfBottomLeftPosition = getBottomLeftPositionForRightArmy(topRightPosition, rightHalfCenter);
    }

    Vector2 getArenaBottomLeft() {
        var leftWall = getWallWorldRect("wall_left");
        var bottomWall = getWallWorldRect("wall_down");
        return new Vector2(leftWall.xMax, bottomWall.yMax);
    }

    Vector2 getArenaTopRight() {
        var rightWall = getWallWorldRect("wall_right");
        var topWall = getWallWorldRect("wall_up");
        return new Vector2(rightWall.xMin, topWall.yMin);
    }

    Rect getWallWorldRect(string wallName) {
        var canvasScale = canvasController.getCanvasMinScale();
        return arenaView.transform.Find(wallName).GetComponent<RectTransform>().getWorldRect(canvasScale);
    }

    Rect getCornerWorldRect(string cornerName) {
        var canvasScale = canvasController.getCanvasMinScale();
        return arenaView.transform.Find(cornerName).GetComponent<RectTransform>().getWorldRect(canvasScale);
    }

    Vector2 getBottomLeftPositionForLeftArmy(Vector2 arenaBottomLeft, Vector2 leftHalfCenter) {
        var totalUnitHeight = getTotalUnitHeight();
        var x = arenaBottomLeft.x + settings.distanceToWall + unitScale.x / 2;
        var y = leftHalfCenter.y - totalUnitHeight / 2 + unitScale.y * UnitHeightToWidthRatio / 2;
        return new Vector2(x, y);
    }

    Vector2 getBottomLeftPositionForRightArmy(Vector2 arenaTopRight, Vector2 rightHalfCenter) {
        var totalUnitWidth = getTotalUnitWidth();
        var totalUnitHeight = getTotalUnitHeight();
        var x = arenaTopRight.x - settings.distanceToWall - totalUnitWidth + unitScale.x / 2;
        var y = rightHalfCenter.y - totalUnitHeight / 2 + unitScale.y * UnitHeightToWidthRatio / 2;
        return new Vector2(x, y);
    }

    float getTotalUnitWidth() {
        var numberOfRows = 3;
        return unitScale.x * numberOfRows + settings.distanceBetweenUnits.x * (numberOfRows - 1);
    }

    float getTotalUnitHeight() {
        return unitScale.y * UnitHeightToWidthRatio * settings.unitsPerRow + settings.distanceBetweenUnits.y * (settings.unitsPerRow - 1);
    }
    
    public Vector3 getPositionForUnit(int x, int y, bool leftArmy) {
        var bottomLeft = leftArmy ? leftHalfBottomLeftPosition : rightHalfBottomLeftPosition;
        return new Vector3(
            bottomLeft.x + x * horizontalStep,
            bottomLeft.y + y * verticalStep
        );
    }

    #region contracts
    public float getUnitLength() => unitScale.x;

    public Vector2 getBottomLeftCorner() => bottomLeftPosition;

    public Vector2 getTopRightCorner() => topRightPosition;
    #endregion
}
}