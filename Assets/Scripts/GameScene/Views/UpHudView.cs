using TMPro;
using UnityEngine;

namespace GameScene.Views {
public class UpHudView : MonoBehaviour {
    [SerializeField] TMP_Text roundLabel;

    public void setRoundLabelText(string text) {
        roundLabel.SetText(text);
    }
}
}