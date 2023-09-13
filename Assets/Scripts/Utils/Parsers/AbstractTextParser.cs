using System.Text.RegularExpressions;
using UnityEngine;

namespace Utils.Parsers {
public class AbstractTextParser {
    const string lineBreakPattern = @"\r?\n";
    
    public string[] parseLines(string resourcePath) {
        var asset = Resources.Load<TextAsset>(resourcePath);
        var lines = Regex.Split(asset.text, lineBreakPattern);
        for (var i = 0; i < lines.Length; i++) {
            lines[i] = lines[i].Trim();
        }
        return lines;
    }
}
}