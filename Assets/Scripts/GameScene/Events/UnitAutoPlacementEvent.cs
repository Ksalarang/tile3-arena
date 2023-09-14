using GameScene.Models;

namespace GameScene.Events {
/// <summary>
/// Событие автоматического перемещения юнита с Линии на Арену, когда заканчиваются очки действий.
/// </summary>
public class UnitAutoPlacementEvent : GameEvent {
    public Unit unit;

    public UnitAutoPlacementEvent(Unit unit) {
        this.unit = unit;
    }
}
}