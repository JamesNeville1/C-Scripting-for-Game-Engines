using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_master : MonoBehaviour {

    SCR_map_generation mapGen;

    private void Awake() {
        mapGen = GetComponent<SCR_map_generation>();
    }
    private void Start() {
        mapGen.generate();

        Vector2 cameraPos = mapGen.mapCentre();
        Camera.main.transform.position = new Vector3(cameraPos.x, cameraPos.y, -10);
    }
}
