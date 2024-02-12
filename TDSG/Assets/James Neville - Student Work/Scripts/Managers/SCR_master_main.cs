using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_master_main : MonoBehaviour {
    #region Structs & Enums
    public enum sceneKey { SCE_MASTER, SCE_OVERWORLD, SCE_AUDIO_MANAGER, SCE_MENU }
    #endregion

    [Header("Main")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector2Int inventorySize;
    [SerializeField] private Vector2Int mapSize;

    [Header("Scene Names")]
    [SerializeField] private string overworldSceneName;
    [SerializeField] private string audioSceneName;
    [SerializeField] private string masterSceneName;
    [SerializeField] private string menuSceneName;

    [Header("Other")]
    [SerializeField] private bool playerCraftingActive;
    [SerializeField] private string playerParent;

    //
    Dictionary <sceneKey, string> formattedScenes = new Dictionary<sceneKey, string>();

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
        formattedScenes.Add(sceneKey.SCE_MASTER, masterSceneName);
        formattedScenes.Add(sceneKey.SCE_OVERWORLD, overworldSceneName);
        formattedScenes.Add(sceneKey.SCE_AUDIO_MANAGER, audioSceneName);
        formattedScenes.Add(sceneKey.SCE_MENU, menuSceneName);

        //Load Additives
        loadScene(sceneKey.SCE_AUDIO_MANAGER, LoadSceneMode.Additive);
        loadScene(sceneKey.SCE_OVERWORLD, LoadSceneMode.Additive);

        while (!isReady()) {
            yield return null;
        }

        while (!isCharacterMade()) {
            yield return null;
        }

        setup();
    }
    private bool isReady() {
        bool ready = SCR_master_audio.returnInstance() != null;
        return ready;
    }
    private bool isCharacterMade() {
        return true;
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
        SCR_master_audio.returnInstance().setup();

        //Make Player
        Instantiate(playerPrefab, mapRef.startPos(), Quaternion.identity, GameObject.Find(playerParent).transform);
        SCR_player_main.returnInstance().setup();

        //Start music loop
        SCR_master_audio.returnInstance().playRandomMusic(SCR_master_audio.sfx.MUSIC_CALM);
    }
    #endregion
    #region Publics
    public void whatMusic() {
        SCR_master_audio.returnInstance().playRandomMusic(SCR_master_audio.sfx.MUSIC_CALM);
    }
    public bool returnPlayerCrafting() {
        return playerCraftingActive;
    }
    public void setGatheringLocked(bool setTo) {
        playerCraftingActive = setTo;
    }
    public string getSceneClean(sceneKey input) {
        return formattedScenes[input];
    }
    public void moveToScene(GameObject obj,sceneKey input) {
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByName(formattedScenes[input]));
    }
    public void loadScene(sceneKey input, LoadSceneMode mode) {
        SceneManager.LoadScene(formattedScenes[input], mode);
    }
    #endregion
}
