using System;
using UnityEngine;

namespace Services.Saves {
[Serializable]
public class PlayerSave {
    public AudioSave audio;

    public string toJson() => JsonUtility.ToJson(this);

    public void fromJson(string json) => JsonUtility.FromJsonOverwrite(json, this);
}

[Serializable]
public class AudioSave {
    public float soundVolume = 1f;
    public float musicVolume = 1f;
    public bool vibrationEnabled = true;
}
}