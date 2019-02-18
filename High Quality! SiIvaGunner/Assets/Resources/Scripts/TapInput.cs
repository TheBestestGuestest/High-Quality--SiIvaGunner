using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapInput : MonoBehaviour {

    public KeyCode corrKey;
    /* @return  true if the note was tapped, false if otherwise. */
    public bool getHold() {
        if (Application.platform == RuntimePlatform.WindowsEditor) return Input.GetKeyDown(corrKey);
        else {
            foreach (Touch t in Input.touches) {
                if (t.phase != TouchPhase.Began) continue;
                Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(t.position);
                Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
                RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
                if (hitInformation.collider == GetComponent<Collider2D>()) return true;
            }
            return false;
        }
    }

    /* @return  true if the note was released, false if otherwise. */
    public bool getRelease() {  
        if (Application.platform == RuntimePlatform.WindowsEditor) return Input.GetKeyUp(corrKey);
        else {
            foreach (Touch t in Input.touches) {
                Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(t.position);
                Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
                RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
                if(hitInformation.collider == GetComponent<Collider2D>() && (t.phase == TouchPhase.Began || t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved)) return false;
            }
            return true;
        }
    }
}
