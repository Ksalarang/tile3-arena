using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Windows;
using Zenject;

namespace GameScene.Windows {
public class DefeatWindow : Window, IInitializable {
    [SerializeField] Button retryButton;
    [SerializeField] TMP_Text retryButtonLabel;
    [SerializeField] Button exitButton;
    [SerializeField] TMP_Text exitButtonLabel;

    [Inject] Manager manager;

    public void Initialize() {
        retryButton.onClick.AddListener(onClickRetry);
        exitButton.onClick.AddListener(onClickExit);
    }
    
    void onClickRetry() {
        animateHide(() => manager.onClickRetry());
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
        public void onClickRetry();
        public void onClickExit();
    }
}
}