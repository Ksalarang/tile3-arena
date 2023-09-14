using UnityEngine;

namespace GameScene.Models {
public class Warrior : Unit {
    [SerializeField] TrailRenderer trail;
    [SerializeField] RotateAround rotateAround;

    protected override void start() {
        trail.startColor = trail.endColor = color;
        
    }

    protected override void attackTarget() {
        log.log($"attacks {target}", this);
        target.takeDamage(stats.damage);
        // var angle = transform.angleDegrees(target.transform);
        // rotateAround.startRotation(angle, isPlayer, () => {
            // target.takeDamage(stats.damage);
        // });
    }
}
}