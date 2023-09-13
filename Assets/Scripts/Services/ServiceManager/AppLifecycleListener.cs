namespace Services.ServiceManager {
public interface AppLifecycleListener {
    public void onPause();
    
    public void onQuit();
}
}