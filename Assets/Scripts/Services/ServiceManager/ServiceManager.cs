using System.Collections.Generic;
using Services.Saves;
using Services.Scenes;
using Services.Sounds;
using Services.Vibrations;
using UnityEngine;
using Utils;
using Zenject;

namespace Services.ServiceManager {
public class ServiceManager: MonoBehaviour {
    [Inject] LogConfig logConfig;
    
    [Inject] SceneService sceneService;
    [Inject] SoundService soundService;
    [Inject] VibrationService vibrationService;
    [Inject] SaveService saveService;

    Log log;
    List<AppLifecycleListener> appLifecycleListeners;
    List<SaveLoadListener> saveLoadListeners;

    void Awake() {
        log = new(GetType(), logConfig.serviceManager);
        appLifecycleListeners = new List<AppLifecycleListener>();
        saveLoadListeners = new List<SaveLoadListener>();
        registerServices();
        onSavesLoaded();
    }

    void registerServices() {
        log.log("register services");
        registerService(sceneService);
        registerService(soundService);
        registerService(vibrationService);
        registerService(saveService);
    }

    void registerService(Service service) {
        if (service is AppLifecycleListener appLifecycleListener) {
            appLifecycleListeners.Add(appLifecycleListener);
        }
        if (service is SaveLoadListener saveLoadListener) {
            saveLoadListeners.Add(saveLoadListener);
        }
        if (service is Initializable initializable) {
            initializable.initialize();
        }
    }

    void onSavesLoaded() {
        log.log("on save loaded");
        var save = saveService.getSave();
        foreach (var listener in saveLoadListeners) {
            listener.onSaveLoaded(save);
        }
    }

    void OnApplicationPause(bool pauseStatus) {
        log.log("on pause");
        foreach (var listener in appLifecycleListeners) {
            listener.onPause();
        }
    }

    void OnApplicationQuit() {
        log.log("on quit");
        foreach (var listener in appLifecycleListeners) {
            listener.onQuit();
        }
    }
}

public interface Service {}
}