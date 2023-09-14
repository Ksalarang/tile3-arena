using System.Collections.Generic;
using Services.ServiceManager;

namespace Services.LevelData {
public interface LevelDataService: Service {
    public IList<LevelData> getLevelDataList();

    public bool hasActionPointsData();
}
}