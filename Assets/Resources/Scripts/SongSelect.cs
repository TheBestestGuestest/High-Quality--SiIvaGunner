using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SongSelect : MonoBehaviour {
    public SongButton buttonPrefab;

    void Start() {
        Object[] songs = Resources.LoadAll("SongData/") as Object[];
        foreach (TextAsset song in songs) {
            SongButton button = (SongButton)Instantiate(buttonPrefab);
            button.transform.SetParent(transform);
            button.transform.GetChild(0).GetComponent<Text>().text = song.text.Split('\n')[0].Substring("Song Name: ".Length).Trim();
            button.file = song;
        }
    }
}
