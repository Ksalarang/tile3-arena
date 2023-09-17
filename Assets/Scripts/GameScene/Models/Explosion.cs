using UnityEngine;
using Utils;

namespace GameScene.Models {
public class Explosion : MonoBehaviour, Colorable {
    [SerializeField] Vector3 minSize;
    [SerializeField] Vector3 maxSize;
    
    [SerializeField] Color startColor;
    [SerializeField] Color midColor;
    [SerializeField] Color endColor;
    [SerializeField] Color fadeoutColor;

    [SerializeField] float explosionDuration;
    [SerializeField] float fadeoutDuration;

    SpriteRenderer spriteRenderer;

    public Color color {
        get => spriteRenderer.color;
        set => spriteRenderer.color = value;
    }

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void start() {
        var thisTransform = transform;
        thisTransform.localScale = minSize;
        StartCoroutine(Coroutines.scaleTo(thisTransform, maxSize, explosionDuration + fadeoutDuration));
        spriteRenderer.color = startColor;
        StartCoroutine(Coroutines.colorTo(this, midColor, explosionDuration * 0.666f, () => {
            StartCoroutine(Coroutines.colorTo(this, endColor, explosionDuration * 0.333f, () => {
                StartCoroutine(Coroutines.colorTo(this, fadeoutColor, fadeoutDuration / 2, () => {
                    StartCoroutine(Coroutines.fadeTo(this, 0f, fadeoutDuration / 2, () => {
                        Destroy(gameObject);
                    }));
                }));
            }));
        }));
    }
}
}