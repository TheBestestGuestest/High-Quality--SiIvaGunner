using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimation : MonoBehaviour {
    public GameObject scoreText;
    public Text comboText;
    public Text comboNumText;
    public Text qualityText;

    private readonly string[] QUALITY = { "MISS", "Bad", "Good", "GREAT", "PERFECT", "QUALITY" };
    private readonly Color[] QUALITY_COLOR = { Color.grey, Color.red, Color.blue, Color.green, Color.magenta, Color.yellow };

    private Coroutine qTextAnim;
    private Coroutine cTextAnim;

    void Start() {
        displayScore(0);
    }

    /** Animates all text involving the score. **/
    public void updateScore(int q, long s, long c) {
        displayQuality(q);
        displayScore(s);
        displayCombo(c);
    }

    /** Updates the quality text. **/
    public void displayQuality(int q) {
        if (qTextAnim != null) StopCoroutine(qTextAnim);
        qTextAnim = StartCoroutine(qTextAnimation(q));
    }

    /** The quality text animation. **/
    public IEnumerator qTextAnimation(int q) {
        qualityText.text = QUALITY[q];
        qualityText.fontSize = 60;
        qualityText.color = QUALITY_COLOR[q];
        qualityText.CrossFadeAlpha(0f, 0f, false);
        qualityText.CrossFadeAlpha(1f, 0.07f, false);
        for (; qualityText.fontSize < 81; qualityText.fontSize += 5) yield return null;
        yield return null;
        qualityText.fontSize = 77;
        yield return new WaitForSeconds(0.2f);
        qualityText.CrossFadeAlpha(0f, 0.1f, false);
        for (; qualityText.fontSize > 60; qualityText.fontSize--) yield return null;
    }

    /** Updates the score text. **/
    public void displayScore(long s) {
        foreach (Transform child in scoreText.transform) {
            Text digit = child.GetComponent<Text>();
            digit.text = "" + s%10;
            s /= 10;
        }
    }

    /** Updates the combo text. **/
    public void displayCombo(long c) {
        if (cTextAnim != null) StopCoroutine(cTextAnim);
        cTextAnim = StartCoroutine(cTextAnimation(c));
    }

    /** The combo text animation. **/
    public IEnumerator cTextAnimation(long combo) {
        comboNumText.text = combo == 0 ? "" : "" + combo;
        comboText.text = combo == 0 ? "" : "combo";
        comboNumText.fontSize = 17;
        comboText.fontSize = 17;
        for (; comboText.fontSize < 20 && comboNumText.fontSize < 20; comboText.fontSize++, comboNumText.fontSize++) yield return null;
        yield return null;
        comboNumText.fontSize = 17;
        comboText.fontSize = 17;
    }
}
