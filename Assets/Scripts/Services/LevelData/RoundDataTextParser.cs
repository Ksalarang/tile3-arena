using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GameScene.Controllers;
using Utils.Parsers;

namespace Services.LevelData {
public class RoundDataTextParser : AbstractTextParser {
    public IList<LevelData> parse(string filename) {
        var list = new List<LevelData>();
        var lines = parseLines(FilePath.Resources.Parsables + filename);
        LevelData currentLevelData = null;
        var rowIndex = -1;
        foreach (var line in lines) {
            if (line.Length == 0) {
                currentLevelData = null;
                rowIndex = -1;
                continue;
            }
            if (currentLevelData != null) {
                var unitDataList = parseUnitSymbols(line, ++rowIndex);
                foreach (var unitData in unitDataList) currentLevelData.enemyFormation.Add(unitData);
            }
            if (line.StartsWith("round", true, CultureInfo.InvariantCulture)) {
                currentLevelData = new LevelData {
                    levelNumber = Convert.ToInt32(line.Split(' ').Last()),
                    enemyFormation = new List<UnitData>(),
                };
                list.Add(currentLevelData);
            }
        }
        return list;
    }

    IList<UnitData> parseUnitSymbols(string line, int rowIndex) {
        var list = new List<UnitData>();
        var unitSymbols = Regex.Split(line, @"\s+");
        foreach (var unitSymbol in unitSymbols) {
            var unitData = new UnitData {
                level = unitSymbol[1] - '0',
                rowIndex = rowIndex,
                unitClass = getUnitClass(unitSymbol[0]),
            };
            list.Add(unitData);
        }
        return list;
    }

    UnitClass getUnitClass(char letter) {
        return letter switch {
            'w' => UnitClass.Warrior,
            'r' => UnitClass.Ranger,
            'a' => UnitClass.Artillery,
            _ => throw new ArgumentOutOfRangeException($"failed to find a unit class for letter '{letter}'"),
        };
    }
}
}