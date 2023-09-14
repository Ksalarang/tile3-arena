using GameScene.Models;

namespace GameScene.Events {
/// <summary>
/// Событие перемещения юнита с Области спавна на Линию.
/// </summary>
public class UnitOnLinePlacementEvent : GameEvent {
    public Unit unit;

    public UnitOnLinePlacementEvent(Unit unit) {
        this.unit = unit;
    }
}
}