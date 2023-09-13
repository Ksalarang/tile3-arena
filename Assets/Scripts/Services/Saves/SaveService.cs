using Services.ServiceManager;

namespace Services.Saves {
public interface SaveService : Service {
    public PlayerSave getSave();

    public void clearSave();
}
}