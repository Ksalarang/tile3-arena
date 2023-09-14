using GameScene.Controllers;
using UnityEngine;

namespace GameScene.Contracts {
public interface UnitAnimatorFactory {
    Animator create(ArmyType armyType, UnitClass unitClass, int level);
}
}