using System.Collections.Generic;
using GameScene.Controllers;

namespace Services.LevelData {
public class LevelData {
    public int levelNumber;
    public int actionPoints;
    public IList<UnitData> enemyFormation;
}

public class UnitData {
    public UnitClass unitClass;
    public int level;
    public int rowIndex;

    public override string ToString() {
        return $"{unitClass}_L{level}_R{rowIndex}";
    }
}
}