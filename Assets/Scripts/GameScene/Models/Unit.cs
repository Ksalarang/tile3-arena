using System;
using System.Collections.Generic;
using System.Globalization;
using Events;
using GameScene.Contracts;
using GameScene.Controllers;
using GameScene.Events;
using GameScene.Settings;
using TMPro;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Zenject;

namespace GameScene.Models {
public abstract class Unit : MonoBehaviour, Colorable {
    public const int MaxLevel = 3;
    [field: SerializeField] public UnitClass unitClass { get; private set; }
    
    [Inject] protected UnitAnimationSettings animationSettings;
    [Inject] protected EventBus<GameEvent> eventBus;
    [Inject] protected Log log;

    GameObject secondLevelStar;
    GameObject thirdLevelStar1;
    GameObject thirdLevelStar2;
    UnitStats defaultStats;
    TMP_Text label;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidBody;
    UnitAnimator unitAnimator;
    GameObject animationContainer;
    bool moveForward;

    protected UnitState state = UnitState.Idle;
    protected List<Unit> enemies;
    protected Unit target;
    
    public Action<Unit> onClick;
    
    public int level { get; private set; }
    public bool isPlayer { get; private set; }
    public bool isAlive { get; private set; }
    public UnitStats stats { get; private set; }
    public Color defaultColor { get; private set; }
    public Color color {
        get => spriteRenderer.color;
        set {
            spriteRenderer.color = value;
            defaultColor = value;
        }
    }
    public new Collider2D collider { get; private set; }

    #region initialize
    void Awake() {
        level = 1;
        isAlive = true;
        secondLevelStar = transform.Find("SecondLevelStar").gameObject;
        thirdLevelStar1 = transform.Find("ThirdLevelStar1").gameObject;
        thirdLevelStar2 = transform.Find("ThirdLevelStar2").gameObject;
        label = transform.Find("Canvas/Background/Label").GetComponent<TMP_Text>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.isKinematic = true;
        animationContainer = new GameObject("animationContainer");
        animationContainer.transform.SetParent(transform);
        stats = createUnitStats();
        unitAnimator = new UnitAnimator();
        awake();
    }

    void Start() {
        start();
    }

    protected virtual void awake() {}
    
    protected virtual void start() {}

    public void setAnimator(Animator animator) {
        var firstTime = unitAnimator == null;
        if (!firstTime) unitAnimator.animator.gameObject.SetActive(false);
        animator.transform.SetParent(animationContainer.transform, false);
        unitAnimator = new UnitAnimator(animator);
        if (firstTime) animationContainer.transform.Translate(0, -unitAnimator.skeletonHeight / 2, 0);
        setIdleAnimation();
    }

    public void setStats(UnitStats unitStats) {
        defaultStats = unitStats;
        var modifier = getLevelModifier(unitStats);
        stats.health = unitStats.health * modifier;
        stats.damage = unitStats.damage * modifier;
        stats.attackRate = unitStats.attackRate * modifier;
        stats.viewRadius = unitStats.viewRadius;
        stats.attackRadius = unitStats.attackRadius;
        stats.speed = unitStats.speed;
        stats.levelModifiers = (float[]) unitStats.levelModifiers.Clone();
        label.SetText(stats.health.ToString(CultureInfo.InvariantCulture));
    }

    float getLevelModifier(UnitStats defaultStats) {
        var result = 1f;
        for (var i = 0; i < level; i++) {
            result *= defaultStats.levelModifiers[i];
        }
        return result;
    }

    public void setSide(bool isPlayer) {
        this.isPlayer = isPlayer;
        color = isPlayer ? Color.green : Color.red;
    }

    UnitStats createUnitStats() {
        return unitClass switch {
            UnitClass.Warrior => new WarriorStats(),
            UnitClass.Ranger => new RangerStats(),
            UnitClass.Artillery => new ArtilleryStats(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    #endregion

    #region battle
    float attackTimeProgress;
    float lastAttackTime;
    Coroutine attackCoroutine;

    void Update() {
        if (state == UnitState.Idle || !isAlive) return;
        var transform = this.transform;
        if (target is not null && !target.isAlive) state = UnitState.LookForEnemy;
        if (state == UnitState.LookForEnemy) {
            var minDistance = float.MaxValue;
            var index = 0;
            for (var i = 0; i < enemies.Count; i++) {
                var enemy = enemies[i];
                if (!enemy.isAlive) continue;
                var distance = transform.position.distanceTo(enemy.transform.position);
                if (distance < minDistance) {
                    minDistance = distance;
                    index = i;
                }
            }
            if (minDistance <= stats.viewRadius * 2 * transform.localScale.x) {
                target = enemies[index];
                state = UnitState.ApproachEnemy;
                moveForward = false;
                unitAnimator.triggerAnimationOnce(unitAnimator.hashes.Walk);
            } else {
                setMoveForward(true);
            }
        }
        if (state == UnitState.ApproachEnemy) {
            var thisPosition = transform.position;
            var targetPosition = target.transform.position;
            var progress = stats.speed * transform.localScale.x * Time.deltaTime;
            var distance = thisPosition.distanceTo(targetPosition);
            transform.position = Vector3.Lerp(thisPosition, targetPosition, progress / distance);
            if (distance <= stats.attackRadius * 2 * transform.localScale.x) {
                state = UnitState.Attack;
                unitAnimator.triggerAnimation(unitAnimator.hashes.Idle);
                var attackInterval = 1 / stats.attackRate;
                var timeSinceLastAttack = Time.time - lastAttackTime;
                attackTimeProgress = timeSinceLastAttack >= attackInterval ? attackInterval : timeSinceLastAttack;
            }
        }
        if (state == UnitState.Attack) {
            attackTimeProgress += Time.deltaTime;
            if (attackTimeProgress >= 1 / stats.attackRate) {
                attackTimeProgress = 0f;
                if (target.isAlive) {
                    var duration = unitAnimator.getAnimationDuration(UnitAnimations.Attack);
                    var speed = duration / animationSettings.attackDuration * stats.attackRate;
                    unitAnimator.triggerAnimation(unitAnimator.hashes.Attack, speed);
                    var delay = animationSettings.attackDuration / stats.attackRate * animationSettings.attackTime;
                    attackCoroutine = StartCoroutine(Coroutines.delayAction(delay, () => {
                        attackTarget();
                        lastAttackTime = Time.time;
                    }));
                }
            }
        }
        if (moveForward) {
            var x = transform.localScale.x * stats.speed * Time.deltaTime;
            if (!isPlayer) x = -x;
            transform.Translate(x, 0, 0);
        }
        rigidBody.velocity = Vector3.zero; // disable influence of other rigid bodies' forces
    }

    public void takeDamage(float damage) {
        if (!isAlive) return;
        log.log($"takes {damage} damage", this);
        stats.health -= damage;
        signalDamageTake(damage);
        if (stats.health <= 0) {
            stats.health = 0;
            die();
        }
        // indicateDamage();
        label.SetText(stats.health.ToString("0"));
    }
    
    void signalDamageTake(float damage) {
        var actualDamage = stats.health >= 0 ? damage : damage + stats.health;
        eventBus.sendEvent(new BattleEvent.DamageTaken(actualDamage, isPlayer));
    }

    Coroutine damageIndicationCoroutine;

    void indicateDamage() {
        spriteRenderer.color = defaultColor * (1 - animationSettings.damageShadeIntensity);
        if (damageIndicationCoroutine != null) StopCoroutine(damageIndicationCoroutine);
        damageIndicationCoroutine = StartCoroutine(Coroutines.delayAction(animationSettings.damageIndicationDuration, () => {
            if (isAlive) spriteRenderer.color = defaultColor;
        }));
    }
    
    void die() {
        log.log("dies", this);
        isAlive = false;
        spriteRenderer.color = defaultColor * (1 - animationSettings.damageShadeIntensity);
        collider.enabled = false;
        spriteRenderer.sortingOrder = 0;
        eventBus.sendEvent(new BattleEvent.UnitDied(this));
        unitAnimator.triggerAnimation(unitAnimator.hashes.Die);
        StartCoroutine(Coroutines.delayAction(animationSettings.deathFadeDelay, () => {
            var fadeDuration = 0.1f;
            StartCoroutine(Coroutines.fadeTo(this, 0f, fadeDuration, () => {
                gameObject.SetActive(false);
            }));
        }));
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
    }
    
    protected abstract void attackTarget();
    #endregion

    #region interface
    public void enableBattleMode(List<Unit> enemies) {
        rigidBody.isKinematic = false;
        setMoveForward(true);
        this.enemies = enemies;
        state = UnitState.LookForEnemy;
        setLabelActive(animationSettings.showHealth);
    }

    public void setMoveForward(bool value) {
        moveForward = value;
        unitAnimator.triggerAnimationOnce(moveForward ? unitAnimator.hashes.Walk : unitAnimator.hashes.Idle);
    }

    public void disableBattleMode() {
        state = UnitState.Idle;
        moveForward = false;
        unitAnimator.triggerAnimation(unitAnimator.hashes.Idle);
    }

    public void levelUp() {
        if (level == MaxLevel) {
            log.warn($"cannot level up: at maximum level {MaxLevel}", this);
            return;
        }
        level++;
        setStats(defaultStats);
        updateStarVisibility();
    }

    public override string ToString() {
        var side = isPlayer ? "player" : "enemy";
        return $"{side}-{unitClass}-{level}";
    }
    #endregion

    void updateStarVisibility() {
        secondLevelStar.SetActive(level == 2);
        thirdLevelStar1.SetActive(level == 3);
        thirdLevelStar2.SetActive(level == 3);
    }

    void setLabelActive(bool active) {
        transform.Find("Canvas").gameObject.SetActive(active);
    }

    void setIdleAnimation() {
        unitAnimator.playAnimation(unitAnimator.hashes.Idle, RandomUtils.nextFloat());
        cycleRandomIdleAnimation();
    }

    void cycleRandomIdleAnimation() {
        var delay = RandomUtils.nextFloat(10f, 70f);
        StartCoroutine(Coroutines.delayAction(delay, () => {
            if (!unitAnimator.isCurrent(unitAnimator.hashes.Idle)) return;
            unitAnimator.triggerAnimation(unitAnimator.hashes.RandomIdle);
            cycleRandomIdleAnimation();
        }));
    }
}

public enum UnitState {
    Idle,
    LookForEnemy,
    ApproachEnemy,
    Attack,
}
}