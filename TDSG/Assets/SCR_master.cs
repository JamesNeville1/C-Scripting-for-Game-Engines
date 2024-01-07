using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_master : MonoBehaviour {

    private SCR_map_generation mapGen;

    [SerializeField]
    private GameObject playerPrefab;

    private void Awake() {
        mapGen = GetComponent<SCR_map_generation>();
    }
    private void Start() {
        string randSeed = mapGen.randomSeed();
        Debug.Log("Map Seed: " + randSeed);
        mapGen.generate(randSeed);


        //Make Player (player contains inventory logic)
        SCR_player_main player = Instantiate(playerPrefab, mapGen.mapCentre(true), Quaternion.identity).GetComponent<SCR_player_main>();

        //Make Inventory Display
        SCR_ui_main.setupInventory(SCR_player_main.SCR_player_inventory.inventory.Count);
    }
}
