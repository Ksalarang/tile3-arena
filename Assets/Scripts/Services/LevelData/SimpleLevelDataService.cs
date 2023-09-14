using System.Collections.Generic;
using Utils;

namespace Services.LevelData {
public class SimpleLevelDataService : LevelDataService {
    readonly Log log = new(typeof(SimpleLevelDataService), false);
    readonly IList<LevelData> levelDataList;

    public SimpleLevelDataService() {
        var parser = new RoundDataTextParser();
        levelDataList = parser.parse("enemy-formation");
        if (!log.enabled) return;
        foreach (var roundData in levelDataList) {
            log.log($"round {roundData.levelNumber}");
            foreach (var unitData in roundData.enemyFormation) {
                log.log(unitData);
            }
        }
    }

    public IList<LevelData> getLevelDataList() => levelDataList;

    public bool hasActionPointsData() => false;
}
}