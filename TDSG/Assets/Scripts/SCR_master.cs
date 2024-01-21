using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_master : MonoBehaviour {

    [Header("Require Dev Input")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int inventorySizeX;
    [SerializeField] private int inventorySizeY;

    private void Start() {
        //Get Required References
        SCR_map_generation mapRef = GetComponent<SCR_map_generation>();

        //Make Map
        string randSeed = mapRef.randomSeed();
        //Debug.Log("Map Seed: " + randSeed);
        mapRef.generate(randSeed);

        SCR_player_inventory.returnInstance().setup(inventorySizeY, inventorySizeY);

        //Make Player (player contains inventory logic)
        SCR_player_main player = Instantiate(playerPrefab, mapRef.startPos(), Quaternion.identity).GetComponent<SCR_player_main>();
    }
}
