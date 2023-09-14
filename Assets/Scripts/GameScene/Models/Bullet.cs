using UnityEngine;
using Utils;

namespace GameScene.Models {
public class Bullet : Projectile {
    Vector3 moveDirection;
    
    void Update() {
        if (state == State.Launched) {
            moveDirection = (lastTargetPosition - startPosition).normalized;
            var angle = MathUtils.vector2ToAngle(moveDirection);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            state = State.Flying;
        }
        if (state == State.Flying) {
            transform.position += moveDirection * (stats.speed * Time.deltaTime);
            if (!isInsideArena()) destroyItself();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        var unit = other.gameObject.GetComponent<Unit>();
        if (unit != null && unit.isAlive && unit.isPlayer == target.isPlayer) {
            unit.takeDamage(damage);
            destroyItself();
        }
    }

    bool isInsideArena() {
        var position = transform.position;
        var bottomLeft = arenaBordersProvider.getBottomLeftCorner();
        var topRight = arenaBordersProvider.getTopRightCorner();
        return bottomLeft.x <= position.x && position.x <= topRight.x &&
               bottomLeft.y <= position.y && position.y <= topRight.y;
    }
}
}