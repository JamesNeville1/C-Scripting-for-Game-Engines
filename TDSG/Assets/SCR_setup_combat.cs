using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_setup_combat : MonoBehaviour {
    [SerializeField]
    private GameObject tile;

    [SerializeField]
    private int boundsX;
    
    [SerializeField]
    private int boundsY;

    private void Start() {
        //gridSetup();
    }

    //private List<Vector2> gridSetup(int boundsX=16, int boundsY=9) {
        //for (int x = 0; x < boundsX; x++) {
            //for (int y = 0; y < boundsY; y++) {
                //Instantiate(tile, new Vector2(x, y), Quaternion.identity);
            //}
        //}
    //}
}
