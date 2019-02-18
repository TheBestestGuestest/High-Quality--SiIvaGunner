using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScript : MonoBehaviour {
    public Text songTitle;
    public Text score;
    public Text maxCombo;

    private PlayData playData;

    void Start() {
        playData = Scenes.getEndGame();
        titleAnimation();
        scoreAnimation();
        comboAnimation();
        qualityAnimation();
    }

    void Update() {
        //might need for title animation;
    }
    private void titleAnimation() {
        songTitle.text = "";  //rip
    }

    private void scoreAnimation() {
        score.text = playData.score.ToString();
        if (!PlayerPrefs.HasKey("highscore") || Convert.ToInt64(PlayerPrefs.GetString("highscore")) < playData.score) {
            PlayerPrefs.SetString("highscore", playData.score.ToString());
            //NEW! animation
        }
    }

    private void comboAnimation() {
        maxCombo.text = playData.maxCombo.ToString();
        if (!PlayerPrefs.HasKey("maxCombo") || Convert.ToInt64(PlayerPrefs.GetString("maxCombo")) < playData.maxCombo) {
            PlayerPrefs.SetString("maxCombo", playData.maxCombo.ToString());
            //NEW! animation
        }
    }

    private void qualityAnimation() {

    }
}
