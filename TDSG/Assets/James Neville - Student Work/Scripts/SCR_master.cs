using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_master : MonoBehaviour {

    [Header("Require Dev Input")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector2Int inventorySize;
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private Vector2Int combatBoardSize; 

    [Header("Other")]
    [SerializeField] private bool playerCrafting;

    [SerializeField] [MyReadOnly] private SCR_player_main player;
    [SerializeField] [MyReadOnly] private bool inBattle;

    //
    private static SCR_master instance;

    private void Awake() {
        instance = this;

        SceneManager.LoadScene("SCE_audio_manager", LoadSceneMode.Additive);
        SceneManager.LoadScene("SCE_overworld", LoadSceneMode.Additive);
        SceneManager.LoadScene("SCE_combat", LoadSceneMode.Additive);
    }

    private void Start() {
        setup();
    }

    private void setup() { //Controls initial execution order in code and unifies setups
        //Get Required References
        SCR_map_generation mapRef = GetComponent<SCR_map_generation>();

        //Make Map
        string randSeed = mapRef.randomSeed();
        //Debug.Log("Map Seed: " + randSeed);

        //Setup externals
        SCR_map_generation.returnInstance().setup(randSeed, "Ground Tilemap", "Water Tilemap", mapSize);
        SCR_player_inventory.returnInstance().setup(inventorySize.x, inventorySize.y);
        SCR_player_crafting.returnInstance().setup();
        SCR_combat_manager.returnInstance().setup();

        //Make Player (player contains inventory logic)
        player = Instantiate(playerPrefab, mapRef.startPos(), Quaternion.identity).GetComponent<SCR_player_main>();

        //Start music loop
        SCR_audio_manager.returnInstance().playRandomMusic(SCR_audio_manager.sfx.MUSIC_CALM);
    }

    public void whatMusic() {
        SCR_audio_manager.returnInstance().playRandomMusic(inBattle ? SCR_audio_manager.sfx.MUSIC_BATTLE : SCR_audio_manager.sfx.MUSIC_CALM);
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
