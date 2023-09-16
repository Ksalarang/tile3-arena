using GameScene.Controllers;
using Zenject;

namespace GameScene.Models {
public class Ranger : Unit {
    [Inject] ProjectileFactory projectileFactory;

    protected override void attackTarget() {
        log.log($" attacks {target}", this);
        getProjectile().launch(target, enemies, stats.damage);
    }

    Projectile getProjectile() {
        var projectile = projectileFactory.create(unitClass, isPlayer);
        projectile.transform.position = transform.position;
        projectile.color = defaultColor;
        return projectile;
    }
}
}