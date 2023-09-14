using System.Collections.Generic;
using GameScene.Contracts;
using GameScene.Settings;
using UnityEngine;
using Utils;
using Zenject;

namespace GameScene.Models {
public abstract class Projectile : MonoBehaviour, Colorable {
    [Inject] protected UnitMeasureProvider unitMeasureProvider;
    [Inject] protected ArenaBordersProvider arenaBordersProvider;
    [Inject] ProjectileManager manager;
    
    protected ProjectileStats stats;
    protected State state = State.Idle;
    protected Unit target;
    protected List<Unit> enemies;
    protected Vector3 startPosition;
    protected Vector3 lastTargetPosition;
    protected float damage;
    SpriteRenderer spriteRenderer;
    
    public Color color {
        get => spriteRenderer.color;
        set => spriteRenderer.color = value;
    }

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        awake();
    }

    void Start() {
        manager.onCreate(this);
        start();
    }

    protected virtual void awake() {}

    protected virtual void start() {}

    public void setStats(ProjectileStats stats) {
        this.stats = stats;
        this.stats.speed *= unitMeasureProvider.getUnitLength();
        this.stats.collateralDamageRadius *= unitMeasureProvider.getUnitLength();
    }

    public void launch(Unit target, List<Unit> enemies, float damage) {
        this.target = target;
        this.enemies = enemies;
        this.damage = damage;
        startPosition = transform.position;
        lastTargetPosition = target.transform.position;
        state = State.Launched;
    }

    protected void destroyItself() {
        if (state == State.Destroyed) return;
        Destroy(gameObject);
        state = State.Destroyed;
        manager.onDestroy(this);
    }

    protected enum State {
        Idle,
        Launched,
        Flying,
        Destroyed,
    }
}

public interface ProjectileManager {
    public void onCreate(Projectile projectile);
    public void onDestroy(Projectile projectile);
}
}