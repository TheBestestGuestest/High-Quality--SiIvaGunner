using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldNoteUp : MonoBehaviour {
    public static Object prefab;
    public static Rip rip;
    public static int destroyAfter;

    private Note parent;

    private float moveSpeed;
    private int spawnTime;
    private Vector3 pos;

    private LineRenderer lr;

    public static void initAll(Rip r, int desA) {
        rip = r;
        prefab = Resources.Load("Prefabs/HeldNoteUp");
        destroyAfter = desA;
    }

    public static HeldNoteUp Create(Note parent, NoteData nd) {
        HeldNoteUp note = (Instantiate(prefab, nd.lane, Quaternion.identity) as GameObject).GetComponent<HeldNoteUp>();
        note.moveSpeed = nd.moveSpeed;
        note.spawnTime = nd.timePosition;
        note.parent = parent;
        return note;
    }

    /** Animation for when the note is hit successfully. **/
    public IEnumerator hitAnimation() {
        for (int i = 0; i < 7; i++) {
            Color c = gameObject.GetComponent<Renderer>().material.color;
            transform.localScale += Vector3.one / 7;
            gameObject.GetComponent<Renderer>().material.color = new Color(c.r, c.g, c.b, c.a * 6 / 7);
            yield return null;
        }
        Destroy(gameObject);
    }

    void Start() {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        pos = transform.position;
    }

    void Update() {
        int time = rip.getSamples() - spawnTime;
        int diff = destroyAfter - time;
        transform.position = new Vector3(pos.x, Mathf.Max((diff) * moveSpeed - 3, -3), pos.z);
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, parent.GetComponent<Transform>().position);
    }
}
