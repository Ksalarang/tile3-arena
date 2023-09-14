using System.Collections.Generic;
using System.Linq;
using GameScene.Settings;
using Services.LevelData;
using Zenject;

namespace GameScene {
public class GameModel : IInitializable {
    [Inject] LevelDataService levelDataService;
    [Inject] PrepActionSettings prepActionSettings;
    
    IList<LevelData> levelDataList;
    
    public GameLevel currentLevel;

    public void Initialize() {
        levelDataList = levelDataService.getLevelDataList();
        var levelData = getLevelData(1);
        currentLevel = new GameLevel {
            number = levelData.levelNumber,
            actionPoints = getActionPoints(levelData),
            enemyFormation = levelData.enemyFormation,
        };
    }

    LevelData getLevelData(int number) {
        return levelDataList.FirstOrDefault(level => level.levelNumber == number);
    }

    int getActionPoints(LevelData levelData) {
        return levelDataService.hasActionPointsData()
            ? levelData.actionPoints
            : prepActionSettings.actionPointsPerRound;
    }

    public bool nextLevel() {
        var levelData = getLevelData(currentLevel.number + 1);
        if (levelData == null) return false;
        currentLevel = new GameLevel {
            number = levelData.levelNumber,
            actionPoints = getActionPoints(levelData),
            enemyFormation = levelData.enemyFormation,
        };
        return true;
    }

    public void reset() {
        Initialize();
    }
}

public class GameLevel {
    public int number;
    public int actionPoints;
    public IList<UnitData> enemyFormation;
}
}