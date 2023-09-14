using GameScene.Settings;
using GameScreen;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace GameScene.Models {
public class Shell : Projectile {
    const float AltitudeFactor = 0.75f;

    [Inject(Id = PrefabId.Explosion)] GameObject explosionPrefab;
    new ShellStats stats;
    bool hitOccured;

    protected override void start() {
        stats = (ShellStats) base.stats;
    }

    void Update() {
        if (state == State.Launched) {
            state = State.Flying;
        }
        if (state == State.Flying) {
            var position = transform.position;
            var startX = startPosition.x;
            var endX = lastTargetPosition.x;
            var distance = endX - startX;
            var absDistance = Mathf.Abs(distance);
            if (stats.maxAltitude > absDistance) stats.maxAltitude = absDistance * AltitudeFactor;
            var nextX = Mathf.MoveTowards(position.x, endX, stats.speed * Time.deltaTime);
            var baseY = Mathf.Lerp(startPosition.y, lastTargetPosition.y, (nextX - startX) / distance);
            var deltaY = stats.maxAltitude * (nextX - startX) * (nextX - endX) / (-0.25f * distance * distance);
            transform.position = new Vector3(nextX, baseY + deltaY);
            if (transform.position.approximately(lastTargetPosition)) onHitGround();
        }
    }

    void onHitGround() {
        var mainDamageTaken = false;
        var unitHalfLength = unitMeasureProvider.getUnitLength() / 2;
        var halfWidth = transform.localScale.x / 2;
        var collateralDamage = damage * stats.collateralDamageFactor;
        foreach (var enemy in enemies) {
            if (!enemy.isAlive) continue;
            var distance = transform.position.distanceTo(enemy.transform.position);
            if (!mainDamageTaken && distance - unitHalfLength <= halfWidth) {
                enemy.takeDamage(damage);
                mainDamageTaken = true;
                continue;
            }
            if (distance - unitHalfLength <= stats.collateralDamageRadius) {
                enemy.takeDamage(collateralDamage);
            }
        }
        explode();
        destroyItself();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void explode() {
        var explosion = Instantiate(explosionPrefab).GetComponent<Animator>();
        explosion.transform.position = transform.position;
        var delay = explosion.GetCurrentAnimatorStateInfo(0).length;
        Destroy(explosion.gameObject, delay);
    }
}
}