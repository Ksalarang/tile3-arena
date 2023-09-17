using UnityEngine;
using Utils.Extensions;

namespace GameScene.Models {
public class Warrior : Unit {
    [SerializeField] TrailRenderer trail;
    [SerializeField] RotateAround rotateAround;

    protected override void start() {
        trail.startColor = trail.endColor = color;
    }

    protected override void attackTarget() {
        var angle = transform.angleDegrees(target.transform);
        log.log($"attacks {target}, angle {angle}", this);
        rotateAround.startRotation(angle, isPlayer, () => {
            target.takeDamage(stats.damage);
        });
    }
}
}