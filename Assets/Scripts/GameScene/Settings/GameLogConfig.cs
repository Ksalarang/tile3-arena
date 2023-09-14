using System;

namespace GameScene.Settings {
[Serializable]
public struct GameLogConfig {
    // controllers
    public bool gameController;
    public bool unitSpawner;
    public bool spawnAreaController;
    public bool mergeLineController;
    public bool arenaController;
    public bool battleController;
    // entities
    public bool unit;
}
}