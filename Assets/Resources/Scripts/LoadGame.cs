using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public class LoadGame : MonoBehaviour {
    public void loadScene(string scene) {
        Scenes.Load(scene);
    }

    public void startGame(TextAsset songdata) {
        Scenes.setSongData(songdata);
        Scenes.Load("test", Scenes.getSongData());
    }

    public void clearSceneParameters() {
        Scenes.clearAll();
    }
}
