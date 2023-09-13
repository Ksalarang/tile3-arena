using System;

namespace Services {
[Serializable]
public class GlobalConfig {
    public LogConfig logConfig;
}

[Serializable]
public struct LogConfig {
    public bool serviceManager;
    public bool soundService;
    public bool vibrationService;
    public bool saveService;
}
}