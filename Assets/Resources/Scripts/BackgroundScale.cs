using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundScale : MonoBehaviour {
    void Start() {
        MeshRenderer mr = GetComponent<MeshRenderer>();

        if (Scenes.getSongData() != null) mr.material.mainTexture = Scenes.getSongData().background;
        mr.material.shader = Shader.Find("Unlit/Texture");

        float worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector3(worldScreenWidth / mr.bounds.size.x, worldScreenHeight / mr.bounds.size.y, 1);
    }
}
