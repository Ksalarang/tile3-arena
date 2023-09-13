using UnityEngine;
using Utils;

namespace GameScene {
public class GameController : MonoBehaviour {
    
    Log log;

    void Awake() {
        log = new Log(GetType());
    }

    void Start() {
        log.log("start");
    }
}
}