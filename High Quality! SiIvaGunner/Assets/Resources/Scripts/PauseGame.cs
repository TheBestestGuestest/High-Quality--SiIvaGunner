using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour {

    public Transform canvas;
    public Rip HQRip;

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Pause();
        }
	}

    public void Pause() {
        if (canvas.gameObject.activeInHierarchy) {
            StartCoroutine(HQRip.setPause(false));
            canvas.gameObject.SetActive(false);
        }
        else if(!HQRip.getPause()){
            StartCoroutine(HQRip.setPause(true));
            canvas.gameObject.SetActive(true);
        }
    }
}
