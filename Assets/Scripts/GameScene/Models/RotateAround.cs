using System;
using UnityEngine;

namespace GameScene.Models {
public class RotateAround : MonoBehaviour {
    [SerializeField] float speedDegrees;
    [SerializeField] float radius;
    [SerializeField] float duration;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] GameObject target;
    float angle;
    bool started;
    float progress;
    Action action;

    public void startRotation(float angle, bool clockWise, Action action = null) {
        this.action = action;
        this.angle = clockWise ? -angle : -(angle + 180f);
        speedDegrees = clockWise ? Mathf.Abs(speedDegrees) : -Mathf.Abs(speedDegrees);
        progress = 0f;
        trailRenderer.enabled = true;
        started = true;
    }

    void Update() {
        if (!started) return;
        var delta = Time.deltaTime;
        progress += delta;
        if (progress < duration) {
            angle += speedDegrees * delta;
            var offset = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) * radius;
            transform.position = target.transform.position + offset;
        } else {
            if (action != null) {
                action.Invoke();
                action = null;
            }
            trailRenderer.enabled = false;
            started = false;
        }
    }
}
}