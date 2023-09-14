using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameScene.Controllers {
public class CanvasController : MonoBehaviour {
    [Inject] CanvasScaler canvasScaler;

    public float getCanvasWidthScale() => Screen.width / canvasScaler.referenceResolution.x;
    
    public float getCanvasHeightScale() => Screen.height / canvasScaler.referenceResolution.y;

    public float getCanvasMinScale() => Mathf.Min(getCanvasWidthScale(), getCanvasHeightScale());
}
}