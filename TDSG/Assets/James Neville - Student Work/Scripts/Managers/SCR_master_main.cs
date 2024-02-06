using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_master_main : MonoBehaviour {

    [Header("Main")]
    [SerializeField] private GameObject playerPrefab;
    [Header("")]
    [SerializeField] private Vector2Int inventorySize;
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private Vector2Int combatBoardSize;
    [Header("")]
    [SerializeField] private SCO_enemy tempPassEnemy;

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
    [SerializeField] [MyReadOnly] private bool inBattle;
    [SerializeField] [MyReadOnly] private GameObject overworldMasterObj;
    [SerializeField] [MyReadOnly] private GameObject combatMasterObj;
    [Header("")]
    #region Set Instance
    private static SCR_master_main instance;

    public static SCR_master_main returnInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }
    #endregion

    #region Unity
    private void Start() {
        StartCoroutine(setupAfterExternals());
    }
    #endregion
    #region Setup
    private IEnumerator setupAfterExternals() {
        //Load Additives
        SceneManager.LoadScene(audioSceneName, LoadSceneMode.Additive);
        SceneManager.LoadScene(overworldSceneName, LoadSceneMode.Additive);
        SceneManager.LoadScene(combatSceneName, LoadSceneMode.Additive);

        while (SCR_master_combat.returnInstance() == null || SCR_master_audio.returnInstance() == null) {
            yield return null;
        }
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
        SCR_master_combat.returnInstance().setup(combatBoardSize);
        SCR_master_audio.returnInstance().setup();

        //Make Player (player contains inventory logic)
        Instantiate(playerPrefab, mapRef.startPos(), Quaternion.identity, GameObject.Find(playerParent).transform);
        SCR_player_main.returnInstance().setup();

        //Start music loop
        SCR_master_audio.returnInstance().playRandomMusic(SCR_master_audio.sfx.MUSIC_CALM);

        //Get other scene managers
        overworldMasterObj = GameObject.Find(overworldMasterName); overworldMasterName = "";
        combatMasterObj = GameObject.Find(combatMasterName); combatMasterName = "";

        //Start Enemy Spawner
        //SCR_tick_system.returnTickSystem().subscribe(20f, delegate { SCR_enemy_spawner.returnInstance().spawnEnemy(); });
    }
    #endregion
    #region Load Different Mechanics
    public void loadOverworld() {
        overworldMasterObj.SetActive(true);
        combatMasterObj.SetActive(false);

        SCR_master_timers.returnInstance().unpause(SCR_master_timers.timerID.HUNGER_TICK);
    }
    public void loadCombat() {
        combatMasterObj.SetActive(true);
        overworldMasterObj.SetActive(false);

        SCR_master_timers.returnInstance().pause(SCR_master_timers.timerID.HUNGER_TICK);

        SCR_master_combat.setupEncounterEnemy[] temp = {
            new SCR_master_combat.setupEncounterEnemy(tempPassEnemy, new Vector2Int(5,5)),
        };
        SCR_master_combat.returnInstance().setupEncounter(temp);
    }
    #endregion
    #region Publics
    public void whatMusic() {
        SCR_master_audio.returnInstance().playRandomMusic(inBattle ? SCR_master_audio.sfx.MUSIC_BATTLE : SCR_master_audio.sfx.MUSIC_CALM);
    }
    public bool returnPlayerCrafting() {
        return playerCrafting;
    }
    public void setGatheringLocked(bool setTo) {
        playerCrafting = setTo;
    }
    #endregion
}
