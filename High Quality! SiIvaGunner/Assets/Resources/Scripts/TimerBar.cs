using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBar : MonoBehaviour {

    public Rip rip;
    private float OFFSET_LEFT = 3 - 8;
    private float OFFSET_RIGHT = 1.77f;
    private float max_position;

    private LineRenderer bar;

    private Vector3 pos;

    void Start() {
        float worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        bar = GetComponent<LineRenderer>() as LineRenderer;
        bar.SetPosition(0, new Vector3(-worldScreenWidth / 2, worldScreenHeight / 2, -2));

        bar.SetPosition(1, bar.GetPosition(0));
        max_position = worldScreenWidth / 2 - OFFSET_RIGHT;
        pos = bar.GetPosition(1);
    }

    void Update() {
        float X2position = Mathf.Min(rip.getSamples() * (max_position - OFFSET_LEFT) / rip.getTotalSamples() + OFFSET_LEFT, max_position);
        if (bar.GetPosition(1).x < X2position) bar.SetPosition(1, new Vector3(X2position, pos.y, pos.z));
    }
}
