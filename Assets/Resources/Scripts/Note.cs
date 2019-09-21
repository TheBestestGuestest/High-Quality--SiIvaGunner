using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour {
    public static KeyCode[] KEYS = { KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M };
    private bool debugAuto = false;

    public static UnityEngine.Object prefab;
    public static Rip rip;
    public static int destroyAfter;
    public static float threshold;

    private float moveSpeed;
    private int spawnTime;
    private KeyCode corrKey;
    private Vector3 pos;
    private bool isHittable = false;
    private bool isHit = false;

    private bool isHeldNote = false;
    private bool isHeld = false;
    private int heldFor;
    private HeldNoteUp hnu = null;

    private TapInput tInput;

    /** 
     * Instantiates all the note's static variables.
     * @param rip   The rip that generated the note.
     * @param desA  How long until the note will be hit.
     **/
    public static void initAll(Rip r, int desA) {
        rip = r;
        prefab = Resources.Load("Prefabs/Note");
        destroyAfter = desA;
        threshold = 777;
    }
    /** 
     * Generates a new Note object with the intended parameters.
     * @param v     The initial position of the note.
     * @param ms    The note's move speed.
     * @param st    The note's spawn time.
     **/
    public static Note Create(NoteData nd) {
        Note note = (Instantiate(prefab, nd.lane, Quaternion.identity) as GameObject).GetComponent<Note>();
        note.moveSpeed = nd.moveSpeed;
        note.spawnTime = nd.timePosition;
        note.isHeldNote = nd.isHeld;
        note.heldFor = nd.heldFor;

        if (nd.isHeld) {
            nd.timePosition += nd.heldFor;
            note.hnu = HeldNoteUp.Create(note, nd);
            note.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/sprite_1", typeof(Sprite)) as Sprite;
        }

        note.tInput = note.GetComponent("TapInput") as TapInput;
        note.tInput.corrKey = KEYS[(int)note.transform.position.x / 2 + 3];
        return note;
    }
    void Start() {
        pos = transform.position;
    }

    void Update() {
        if (rip.getPause()) return;
        int time = rip.getSamples() - spawnTime;
        int diff = destroyAfter - time;

        if (isHeldNote) holdOnly(time, diff);
        else hitOnly(time, diff);
    }

    /** 
     * Emulates the behavior of a hit-only note.
     * @param time     The current time since the note has spawned.
     * @param diff     The time difference between the current and the intended beat.
     **/
    void hitOnly(int time, int diff) {
        //auto hit function
        if (debugAuto && isHittable && time >= destroyAfter) {
            Tap(diff);
            return;
        }
        if (!isHit) transform.position = new Vector3(pos.x, (diff) * moveSpeed - 3, pos.z);
        if (isHittable && ((tInput.getHold() && Mathf.Abs(diff) <= 7 * threshold) || diff < -7 * threshold)) Tap(Mathf.Abs(diff));
        //auto destroy
        if (time >= destroyAfter * 2) Destroy(gameObject);
    }
    /** 
     * Processes the player's tap.
     * @param diff     The time difference between the player's tap and the intended beat.
     **/
    void Tap(int diff) {
        isHittable = false;
        rip.updateScore(getQuality(diff), this);
        if ((getQuality(diff) != 0 || isHeldNote) || (getQuality(diff) == 0 && isHeld)) {
            isHit = true;
            StartCoroutine(hitAnimation());
        }
        if (getQuality(diff) != 0) rip.playHit(pos);
    }

    /** 
     * Emulates the behavior of a held note.
     * @param time     The current time since the note has spawned.
     * @param diff     The time difference between the current and the intended beat.
     **/
    void holdOnly(int time, int diff) {
        int diff2 = diff + heldFor;
        if (!isHeld) transform.position = new Vector3(pos.x, (diff) * moveSpeed - 3, pos.z);
        //auto hit function
        if (debugAuto && isHittable) {
            if (!isHeld && time >= destroyAfter) Hold(Mathf.Abs(diff));
            if (time >= destroyAfter + heldFor) Tap(Mathf.Abs(diff2));
            return;
        }
        if (isHittable) {
            if (isHeld && (tInput.getRelease() || diff2 < -7 * threshold)) Tap(Mathf.Abs(diff2));
            else if (!isHeld) {
                if (tInput.getHold() && Mathf.Abs(diff) <= 7 * threshold) Hold(Mathf.Abs(diff));
                else if (diff < -6 * threshold) Tap(Mathf.Abs(diff));
            }
        }

        //auto destroy
        if (time >= (destroyAfter + heldFor) * 2) Destroy(gameObject);
    }
    /** 
     * Processes the player's hold.
     * @param diff     The time difference between the player's hold and the intended beat.
     **/
    void Hold(int diff) {
        if (getQuality(diff) == 0) Tap(diff);
        else {
            isHeld = true;
            transform.position = new Vector3(pos.x, -3, pos.z);
            rip.updateScore(getQuality(diff), this);
            rip.playHit(pos);
        }
    }

    /** 
     * Calculats the quality of the tap.
     * @param diff     The time difference between the player's hold and the intended beat.
     * @return         The quality score.
     **/
    private int getQuality(int diff) {
        int quality = 0;
        for (float thr = 6; thr > 1; thr--) if (diff <= threshold * thr) quality++;
        return quality;
    }

    /** Animation for when the note is hit successfully. **/
    public IEnumerator hitAnimation() {
        if (hnu != null) StartCoroutine(hnu.hitAnimation());
        for (int i = 0; i < 7; i++) {
            Color c = gameObject.GetComponent<Renderer>().material.color;
            transform.localScale += Vector3.one / 7;
            gameObject.GetComponent<Renderer>().material.color = new Color(c.r, c.g, c.b, c.a * 6 / 7);
            yield return null;
        }
        Destroy(gameObject);
    }

    /** @return     true if both notes have the same spawn time, false otherwise. **/
    public bool sameTime(Note other) {
        return this.spawnTime == other.spawnTime;
    }
    /** @return     true if this is closer than the other to the given time, false otherwise. **/
    public bool isCloserThan(Note other, int ts) {
        return Mathf.Abs(this.spawnTime + destroyAfter - ts) < Mathf.Abs(other.spawnTime + destroyAfter - ts);
    }
    /** @param iH   Sets the note's hittabillity state to iH. **/
    public void setInteractable(bool iH) {
        isHittable = iH;
    }
    /** @return     true if this is a held note, false if otherwise. **/
    public bool isHold() {
        return isHeldNote;
    }
    /** @return     the note's lane. **/
    public int getLane() {
        return (int)transform.position.x / 2 + 3;
    }
}
