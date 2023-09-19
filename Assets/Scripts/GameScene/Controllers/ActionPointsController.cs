using Events;
using GameScene.Events;
using GameScene.Settings;
using TMPro;
using UnityEngine;
using Zenject;

namespace GameScene.Controllers {
public class ActionPointsController : MonoBehaviour {
    [Inject] PrepActionSettings settings;
    [Inject(Id = ViewId.ActionPointsLabel)] TMP_Text actionPointsLabel;
    [Inject] EventBus<GameEvent> eventBus;
    
    int _actionPoints;
    public int actionPoints {
        get => _actionPoints;
        set {
            _actionPoints = value;
            updateLabel();
        }
    }

    public void spendPoints(PrepAction action) {
        actionPoints -= getPointsForAction(action);
        if (actionPoints == 0) eventBus.sendEvent(new ActionPointsDepletedEvent());
    }

    public bool isEnoughPointsForAction(PrepAction action) {
        return actionPoints - getPointsForAction(action) >= 0;
    }

    public void resetPoints() {
        actionPoints = settings.actionPointsPerRound;
    }

    int getPointsForAction(PrepAction action) {
        return action switch {
            PrepAction.PlaceUnitOnMergeLine => settings.placeUnitOnMergeLine,
            PrepAction.PlaceUnitOnArena => settings.placeUnitOnArena,
            PrepAction.RespawnUnitsInSpawnArea => settings.respawnUnitsInSpawnArea,
            _ => 1
        };
    }

    void updateLabel() {
        actionPointsLabel.SetText($"Action points: {actionPoints}");
    }
}

public enum PrepAction {
    PlaceUnitOnMergeLine,
    PlaceUnitOnArena,
    RespawnUnitsInSpawnArea,
}
}