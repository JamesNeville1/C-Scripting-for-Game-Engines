using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_master : MonoBehaviour {

    [Header("Require Dev Input")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int inventorySizeX;
    [SerializeField] private int inventorySizeY;

    [Header("Other")]
    [SerializeField] private bool playerCrafting;

    [SerializeField] [MyReadOnly] private SCR_player_main player;

    //
    private static SCR_master instance;

    private void Awake() {
        instance = this;
    }
    private void Start() {
        //Get Required References
        SCR_map_generation mapRef = GetComponent<SCR_map_generation>();

        //Make Map
        string randSeed = mapRef.randomSeed();
        //Debug.Log("Map Seed: " + randSeed);
        mapRef.generate(randSeed);


        SCR_player_inventory.returnInstance().setup(inventorySizeY, inventorySizeY);

        SCR_player_crafting.returnInstance().setup();

        //Make Player (player contains inventory logic)
        player = Instantiate(playerPrefab, mapRef.startPos(), Quaternion.identity).GetComponent<SCR_player_main>();
    }

    public static SCR_master returnInstance() {
        return instance;
    }
    public bool returnPlayerCrafting() {
        return playerCrafting;
    }
    public void setGatheringLocked(bool setTo) {
        playerCrafting = setTo;
    }
    public SCR_player_main returnPlayer() {
        return player;
    }
}
