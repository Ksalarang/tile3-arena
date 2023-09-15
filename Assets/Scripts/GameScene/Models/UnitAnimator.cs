using UnityEngine;

namespace GameScene.Models {
public class UnitAnimator {
    readonly bool enabled;
    
    public readonly Animator animator;
    public readonly AnimationHashes hashes = new();

    public float skeletonHeight { get; private set; }

    public UnitAnimator(Animator animator = null) {
        enabled = animator != null;
        if (!enabled) return;
        this.animator = animator;
        skeletonHeight = computeHeight();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    float computeHeight() {
        var transform = animator.transform;
        var firstChildBounds = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds;
        var minY = firstChildBounds.min.y;
        var maxY = firstChildBounds.max.y;
        for (var i = 0; i < transform.childCount; i++) {
            var renderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (renderer == null) continue;
            var bounds = renderer.bounds;
            if (bounds.min.y < minY) minY = bounds.min.y;
            if (bounds.max.y > maxY) maxY = bounds.max.y;
        }
        return maxY - minY;
    }

    public void playAnimation(int animationHash, float normalizedTime = 0f) {
        if (!enabled) return;
        animator.Play(animationHash, 0, normalizedTime);
    }

    public void triggerAnimation(int animationHash, float speed = 1f) {
        if (!enabled) return;
        animator.speed = speed;
        animator.SetTrigger(animationHash);
    }

    public void triggerAnimationOnce(int animationHash) {
        if (!enabled) return;
        if (animationHash == getCurrentAnimationHash()) return;
        triggerAnimation(animationHash);
    }

    public float getAnimationDuration(string animationName) {
        if (!enabled) return 0f;
        foreach (var clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == animationName) return clip.length;
        }
        return 0f;
    }

    public bool isCurrent(int animationHash) => enabled && animationHash == getCurrentAnimationHash();

    int getCurrentAnimationHash() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

    public class AnimationHashes {
        public readonly int Idle = Animator.StringToHash(UnitAnimations.Idle);
        public readonly int RandomIdle = Animator.StringToHash(UnitAnimations.RandomIdle);
        public readonly int Walk = Animator.StringToHash(UnitAnimations.Walk);
        public readonly int Attack = Animator.StringToHash(UnitAnimations.Attack);
        public readonly int Die = Animator.StringToHash(UnitAnimations.Die);
    }
}

public static class UnitAnimations {
    public const string Empty = "empty";
    public const string Idle = "idle_0";
    public const string RandomIdle = "idle_1";
    public const string Walk = "walk";
    public const string Attack = "fight";
    public const string Die = "die";
}
}