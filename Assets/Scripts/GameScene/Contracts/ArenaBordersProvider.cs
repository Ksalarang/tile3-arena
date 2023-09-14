using UnityEngine;

namespace GameScene.Contracts {
public interface ArenaBordersProvider {
    public Vector2 getBottomLeftCorner();
    public Vector2 getTopRightCorner();
}
}