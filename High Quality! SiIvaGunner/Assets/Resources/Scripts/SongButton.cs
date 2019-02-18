using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongButton : MonoBehaviour {
    public TextAsset file;

    public void onClick () {
        Scenes.setSongData(file);
        Scenes.Load("test", Scenes.getSongData());
    }
}
