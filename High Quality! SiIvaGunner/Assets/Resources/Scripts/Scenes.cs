using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Scenes {

    private static SongData parameters;
    private static PlayData endGame;

    public static void clearAll() {
        parameters = null;
        endGame = null;
    }
    public static void Load(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
    public static void Load(string sceneName, SongData parameters) {
        Scenes.parameters = parameters;
        SceneManager.LoadScene(sceneName);
    }
    public static void Load(string sceneName, PlayData endGame) {
        Scenes.endGame = endGame;
        SceneManager.LoadScene(sceneName);
    }

    public static void setSongData(TextAsset songdata) {
        parameters = new SongData(songdata);
    }
    public static SongData getSongData() {
        return parameters;
    }

    public static void setEndGame(PlayData playdata) {
        endGame = playdata;
    }
    public static PlayData getEndGame() {
        return endGame;
    }
}

public class SongData {
    public AudioClip rip;
    public float offset;
    public float bpm;
    public float moveSpeed;
    public long maxScore;
    public Texture background;
    public int difficulty;
    public List<string> beatmap;

    public SongData(TextAsset songdata) {
        string[] data = songdata.text.Split('\n');
        if (data[0].IndexOf("Song Name: ") != -1) rip = Resources.Load("Music/" + data[0].Substring("Song Name: ".Length).Trim()) as AudioClip;
        if (data[1].IndexOf("Offset: ") != -1) offset = Convert.ToSingle(data[1].Substring("Offset: ".Length).Trim());
        if (data[2].IndexOf("BPM: ") != -1) bpm = Convert.ToSingle(data[2].Substring("BPM: ".Length).Trim());
        if (data[3].IndexOf("Move Speed: ") != -1) moveSpeed = Convert.ToSingle(data[3].Substring("Move Speed: ".Length).Trim());
        if (data[4].IndexOf("Max Score: ") != -1) maxScore = Convert.ToInt64(data[4].Substring("Max Score: ".Length).Trim());
        if (data[5].IndexOf("Background: ") != -1) background = Resources.Load("Backgrounds/" + data[5].Substring("Background: ".Length).Trim()) as Texture;
        if (data[6].IndexOf("Difficulty: ") != -1) difficulty = Convert.ToInt32(data[6].Substring("Difficulty: ".Length).Trim());

        if (difficulty > 0) {
            beatmap = new List<string>();
            for (int i = 7; i < data.Length; i++) beatmap.Add(data[i]);
        }
    }
}

public class PlayData {
    public long score;
    public long maxCombo;
    public long[] qualityList;

    public PlayData(long s, long mC, long[] qL) {
        score = s;
        maxCombo = mC;
        qualityList = qL;
    }
}
