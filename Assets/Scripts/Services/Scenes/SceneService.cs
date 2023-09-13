using System;
using Services.ServiceManager;
using UnityEngine.SceneManagement;

namespace Services.Scenes {
public class SceneService: Service {

    public void loadScene(Scene scene) {
        SceneManager.LoadScene(getSceneName(scene));
    }

    string getSceneName(Scene scene) {
        return scene switch {
            Scene.MainMenu => "MainMenuScene",
            Scene.Game => "GameScene",
            _ => throw new ArgumentOutOfRangeException(nameof(scene), scene, null)
        };
    }
}

public enum Scene {
    MainMenu,
    Game,
}
}