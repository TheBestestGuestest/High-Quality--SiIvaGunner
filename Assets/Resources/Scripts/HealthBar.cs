using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public GameObject healthbar;
    private int maxHealth;
    private int currHealth;
    private float OFFSET_BOTTOM = 3 - 8;
    private float OFFSET_TOP = 2.4f;
    private float max_position;

    private LineRenderer bar;

    private Vector3 pos;


    //gotta make this relate to the rip lmao
    void Start() {
        float worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        bar = healthbar.GetComponent<LineRenderer>() as LineRenderer;
        max_position = worldScreenHeight / 2 - OFFSET_TOP;
        pos = bar.GetPosition(1);

        maxHealth = 77;
        currHealth = 7;
    }

    void Update() {
        if (currHealth < maxHealth && currHealth != 0) currHealth++;
        float Y2position = Mathf.Min(currHealth * (max_position - OFFSET_BOTTOM) / maxHealth + OFFSET_BOTTOM, max_position);
        bar.SetPosition(1, new Vector3(pos.x, Y2position, pos.z));
    }

    public void addHealth(int h) { currHealth = h > 0 ? Mathf.Min(maxHealth, currHealth + h) : Mathf.Max(0, currHealth + h); }
}
