using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameScene.Views {
public class UnitHealthBars : MonoBehaviour {
    const float epsilon = 1E-6f;
    [Inject(Id = ViewId.PlayerHealthBar)] Slider playerHealthBar;
    [Inject(Id = ViewId.EnemyHealthBar)] Slider enemyHealthBar;

    public float playerHealth {
        get => playerHealthBar.value;
        set => playerHealthBar.value = value > epsilon ? value : 0f;
    }
    public float enemyHealth {
        get => enemyHealthBar.value;
        set => enemyHealthBar.value = value > epsilon ? value : 0f;
    }

    public void setActive(bool active) {
        playerHealthBar.gameObject.SetActive(active);
        enemyHealthBar.gameObject.SetActive(active);
    }
}
}