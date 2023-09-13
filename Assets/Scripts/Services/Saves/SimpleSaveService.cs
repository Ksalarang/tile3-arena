using Services.ServiceManager;
using UnityEngine;
using Utils;
using Zenject;

namespace Services.Saves {
public class SimpleSaveService : SaveService, AppLifecycleListener {
    const string SaveFileName = "savedata.dat";
    readonly Log log;
    PlayerSave save;

    [Inject]
    public SimpleSaveService(LogConfig logConfig) {
        log = new(GetType(), logConfig.saveService);
        save = new();
        loadData();
    }

    void loadData() {
        if (FileUtils.loadFromFile(SaveFileName, out var data)) {
            log.log($"loaded from file: {data}");
            save.fromJson(data);
        } else {
            log.warn($"failed to load data from file '{SaveFileName}'");
        }
    }

    void saveData() {
        if (FileUtils.writeToFile(SaveFileName, save.toJson())) {
            log.log("data saved");
        } else {
            log.error("failed to save data");
        }
    }

    public PlayerSave getSave() => save;

    public void clearSave() {
        save = new PlayerSave();
        saveData();
        Application.Quit();
    }

    public void onPause() {
        saveData();
    }

    public void onQuit() {
        saveData();
    }
}
}