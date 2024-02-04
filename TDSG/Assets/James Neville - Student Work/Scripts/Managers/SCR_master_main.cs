using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_master_main : MonoBehaviour {

    [Header("Main")]
    [SerializeField] private GameObject playerPrefab;
    [Header("")]
    [SerializeField] private Vector2Int inventorySize;
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private Vector2Int combatBoardSize; 

    [Header("Other")]
    [SerializeField] private bool playerCrafting;
    [SerializeField] private string playerParent;
    [Header("")]
    [SerializeField] private string overworldMasterName;
    [SerializeField] private string combatMasterName;
    [Header("")]
    [SerializeField] private string overworldSceneName;
    [SerializeField] private string combatSceneName;
    [SerializeField] private string audioSceneName;
    [Header("")]
    [SerializeField] [MyReadOnly] private SCR_player_main player;
    [SerializeField] [MyReadOnly] private bool inBattle;
    [SerializeField] [MyReadOnly] private GameObject overworldMaster;
    [SerializeField] [MyReadOnly] private GameObject combatMaster;

    private static SCR_master_main instance;

    private void Awake() {
        instance = this;

        SceneManager.LoadScene(audioSceneName, LoadSceneMode.Additive);
        SceneManager.LoadScene(overworldSceneName, LoadSceneMode.Additive);
        SceneManager.LoadScene(combatSceneName, LoadSceneMode.Additive);
    }

    private void Start() {
        setup();
    }

    private void setup() { //Controls initial execution order in code and unifies setups
        //Get Required References
        SCR_master_generation mapRef = GetComponent<SCR_master_generation>();

        //Make Map
        string randSeed = mapRef.randomSeed();
        //Debug.Log("Map Seed: " + randSeed);

        //Setup externals
        SCR_master_generation.returnInstance().setup(randSeed, "Ground Tilemap", "Water Tilemap", mapSize);
        SCR_master_inventory_main.returnInstance().setup(inventorySize.x, inventorySize.y);
        SCR_master_crafting.returnInstance().setup();
        SCR_combat_master.returnInstance().setup();

        //Make Player (player contains inventory logic)
        player = Instantiate(playerPrefab, mapRef.startPos(), Quaternion.identity, GameObject.Find(playerParent).transform).GetComponent<SCR_player_main>();

        //Start music loop
        SCR_master_audio.returnInstance().playRandomMusic(SCR_master_audio.sfx.MUSIC_CALM);

        //Get other scene managers
        overworldMaster = GameObject.Find(overworldMasterName); overworldMasterName = "";
        combatMaster = GameObject.Find(combatMasterName); combatMasterName = "";

        //Mark Don't Destroy - I don't think this is necessary, check?
        //DontDestroyOnLoad(overworldMaster);
        //DontDestroyOnLoad(combatMaster);

        //Start Enemy Spawner
        //SCR_tick_system.returnTickSystem().subscribe(20f, delegate { SCR_enemy_spawner.returnInstance().spawnEnemy(); });
    }

    public void loadOverworld() {
        overworldMaster.SetActive(true);
        combatMaster.SetActive(false);
    }
    public void loadCombat() {
        combatMaster.SetActive(true);
        overworldMaster.SetActive(false);
    }

    public void whatMusic() {
        SCR_master_audio.returnInstance().playRandomMusic(inBattle ? SCR_master_audio.sfx.MUSIC_BATTLE : SCR_master_audio.sfx.MUSIC_CALM);
    }
    public static SCR_master_main returnInstance() {
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
