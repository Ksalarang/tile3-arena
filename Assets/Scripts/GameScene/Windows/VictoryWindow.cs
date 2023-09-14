using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Windows;
using Zenject;

namespace GameScene.Windows {
public class VictoryWindow : Window, IInitializable {
    [SerializeField] Button continueButton;
    [SerializeField] TMP_Text continueButtonLabel;
    [SerializeField] Button exitButton;
    [SerializeField] TMP_Text exitButtonLabel;

    [Inject] Manager manager;

    public void Initialize() {
        continueButton.onClick.AddListener(onClickContinue);
        exitButton.onClick.AddListener(onClickExit);
    }
    
    void onClickContinue() {
        animateHide(() => {
            manager.onClickContinue();
        });
    }

    void onClickExit() {
        manager.onClickExit();
    }

    public void animateShow(Action action = null) {
        base.show(FadeDuration, action);
    }

    public void animateHide(Action action = null) {
        base.hide(FadeDuration, action);
    }
    
    public interface Manager {
        public void onClickContinue();
        public void onClickExit();
    }
}
}