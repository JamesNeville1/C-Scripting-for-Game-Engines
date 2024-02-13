using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_master_main : MonoBehaviour {
    #region Structs & Enums
    public enum sceneKey { SCE_MASTER, SCE_OVERWORLD, SCE_AUDIO_MANAGER, SCE_MENU, SCE_CHARACTER_SELECTION }
    #endregion

    [Header("Main")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector2Int inventorySize;
    [SerializeField] private Vector2Int mapSize;
    [Header("")]
    [SerializeField] [MyReadOnly] private SCO_character_preset playerPreset;

    [Header("Scene Names")]
    [SerializeField] private string overworldSceneName;
    [SerializeField] private string audioSceneName;
    [SerializeField] private string masterSceneName;
    [SerializeField] private string menuSceneName;
    [SerializeField] private string characterSelectionSceneName;

    [Header("Object Names")]
    [SerializeField] private string overworldParentName;
    [SerializeField] private string playerParent;

    [Header("Other")]
    [SerializeField] [MyReadOnly] private bool playerCraftingActive;
    [SerializeField] private GameObject masterCameraRef;

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
        StartCoroutine(preSetup());
    }
    #endregion
    #region Setup
    private IEnumerator preSetup() {
        formattedScenes.Add(sceneKey.SCE_MASTER, masterSceneName);
        formattedScenes.Add(sceneKey.SCE_OVERWORLD, overworldSceneName);
        formattedScenes.Add(sceneKey.SCE_AUDIO_MANAGER, audioSceneName);
        formattedScenes.Add(sceneKey.SCE_MENU, menuSceneName);
        formattedScenes.Add(sceneKey.SCE_CHARACTER_SELECTION, characterSelectionSceneName);

        //Load Additives
        loadScene(sceneKey.SCE_AUDIO_MANAGER, LoadSceneMode.Additive);
        masterCameraRef.SetActive(false);
        while (!audioIsReady()) yield return null;
        SCR_master_audio.returnInstance().setup();

        loadScene(sceneKey.SCE_CHARACTER_SELECTION, LoadSceneMode.Additive);
        while (!isCharacterMade()) yield return null;

        masterCameraRef.SetActive(true);
        unloadScene(sceneKey.SCE_CHARACTER_SELECTION); //Why no work?
        loadScene(sceneKey.SCE_OVERWORLD, LoadSceneMode.Additive);
        while (GameObject.Find(overworldParentName) == null) yield return null;

        setupMain();
    }
    private bool audioIsReady() {
        bool ready = SCR_master_audio.returnInstance() != null;
        return ready;
    }
    private bool isCharacterMade() {
        return playerPreset != null;
    }

    private void setupMain() { //Controls initial execution order in code and unifies setups
        //Get Required References
        SCR_master_generation mapRef = GetComponent<SCR_master_generation>();

        //Make Map
        string randSeed = mapRef.randomSeed();
        //Debug.Log("Map Seed: " + randSeed);

        //Setup externals
        SCR_master_generation.returnInstance().setup("12", "Ground Tilemap", "Water Tilemap", mapSize);
        SCR_master_inventory_main.returnInstance().setup(inventorySize.x, inventorySize.y);
        SCR_master_crafting.returnInstance().setup();

        //Make Player
        Instantiate(playerPrefab, mapRef.startPos(), Quaternion.identity, GameObject.Find(playerParent).transform);
        SCR_player_main.returnInstance().setup(playerPreset);

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
    public void setPlayerPreset(SCO_character_preset preset) { 
        playerPreset = preset;
    }
    #endregion
    #region Clean Scenes
    private string getSceneClean(sceneKey input) {
        return formattedScenes[input];
    }
    private void moveToScene(GameObject obj, sceneKey input) {
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByName(formattedScenes[input]));
    }
    private void loadScene(sceneKey input, LoadSceneMode mode) {
        SceneManager.LoadScene(formattedScenes[input], mode);
    }
    private void unloadScene(sceneKey input) {
        SceneManager.UnloadSceneAsync(formattedScenes[input]);
    }
    #endregion
}
