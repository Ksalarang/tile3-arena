using System;
using GameScene.Models;
using GameScene.Settings;
using GameScreen;
using UnityEngine;
using Zenject;

namespace GameScene.Controllers {
public class ProjectileFactory : MonoBehaviour {
    [SerializeField] Sprite bulletSprite;
    [SerializeField] Sprite laserSprite;
    [SerializeField] Sprite zombieBombSprite;
    [SerializeField] Sprite alienBombSprite;

    [Inject(Id = PrefabId.Bullet)] GameObject bulletPrefab;
    [Inject(Id = PrefabId.Shell)] GameObject shellPrefab;
    [Inject] DiContainer diContainer;
    [Inject] UnitStatsSettings unitStatsSettings;
    [Inject] ProjectileSettings projectileSettings;

    // ReSharper disable Unity.PerformanceAnalysis
    public Projectile create(UnitClass unitClass, bool isPlayer = true) {
        var projectile = diContainer.InstantiatePrefabForComponent<Projectile>(getPrefab(unitClass));
        setSprite(projectile, unitClass, isPlayer);
        var stats = getStats(unitClass).clone();
        projectile.setStats(stats);
        return projectile;
    }

    GameObject getPrefab(UnitClass unitClass) {
        return unitClass switch {
            UnitClass.Warrior => null,
            UnitClass.Ranger => bulletPrefab,
            UnitClass.Artillery => shellPrefab,
            _ => throw new ArgumentOutOfRangeException(nameof(unitClass), unitClass, null)
        };
    }

    void setSprite(Projectile projectile, UnitClass unitClass, bool isPlayer) {
        var spriteRenderer = projectile.GetComponent<SpriteRenderer>();
        if (unitClass == UnitClass.Ranger) {
            spriteRenderer.sprite = isPlayer ? bulletSprite : laserSprite;
        } else if (unitClass == UnitClass.Artillery) {
            spriteRenderer.sprite = isPlayer ? zombieBombSprite : alienBombSprite;
        }
    }

    ProjectileStats getStats(UnitClass unitClass) {
        return unitClass switch {
            UnitClass.Warrior => null,
            UnitClass.Ranger => projectileSettings.bulletStats,
            UnitClass.Artillery => projectileSettings.shellStats,
            _ => throw new ArgumentOutOfRangeException(nameof(unitClass), unitClass, null)
        };
    }

    float getDamage(UnitClass unitClass) {
        return unitClass switch {
            UnitClass.Warrior => 0,
            UnitClass.Ranger => unitStatsSettings.rangerStats.damage,
            UnitClass.Artillery => unitStatsSettings.artilleryStats.damage,
            _ => throw new ArgumentOutOfRangeException(nameof(unitClass), unitClass, null)
        };
    }
}
}