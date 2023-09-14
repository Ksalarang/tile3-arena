using GameScene.Controllers;
using GameScene.Models;

namespace GameScene.Events {
public class BattleEvent {
    public class UnitDied : GameEvent {
        public Unit unit;

        public UnitDied(Unit unit) {
            this.unit = unit;
        }
    }

    public class DamageTaken : GameEvent {
        public float damage;
        public bool isPlayer;

        public DamageTaken(float damage, bool isPlayer) {
            this.damage = damage;
            this.isPlayer = isPlayer;
        }
    }
    
    public class BattleEnded : GameEvent {
        public BattleResult result;

        public BattleEnded(BattleResult result) {
            this.result = result;
        }
    }

    public class TestBattleEnded : GameEvent {
        public BattleResult result;

        public TestBattleEnded(BattleResult result) {
            this.result = result;
        }
    }
}
}