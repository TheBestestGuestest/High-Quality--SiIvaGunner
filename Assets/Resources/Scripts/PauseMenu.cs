using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public Text songTitle;
    public Text highscore;
    public Text maxCombo;
    // Use this for initialization
    void Start() {
        //song title thing
        highscore.text = PlayerPrefs.HasKey("highscore") ? PlayerPrefs.GetString("highscore") : "0000000";
        maxCombo.text = PlayerPrefs.HasKey("maxCombo") ? PlayerPrefs.GetString("maxCombo") : "000";
    }

    // Update is called once per frame
    void Update() {
        //song title animationnnnnn
    }
}
