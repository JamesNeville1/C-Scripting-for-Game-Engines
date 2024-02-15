using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] [MyReadOnly] private string seed;

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
    [SerializeField][MyReadOnly] private bool playerCraftingActive;
    [SerializeField] private GameObject masterCameraRef;
    [SerializeField][MyReadOnly] private bool startPressed;

    [Header("Loading Related")]
    [SerializeField] private GameObject loadingScreenRef;

    //
    Dictionary<sceneKey, string> formattedScenes = new Dictionary<sceneKey, string>();

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
        SCR_master_audio.returnInstance().playRandomMusic(SCR_master_audio.sfx.MUSIC_CALM);

        loadScene(sceneKey.SCE_MENU, LoadSceneMode.Additive);
        while (!pressedStart()) yield return null;
        unloadScene(sceneKey.SCE_MENU);

        loadScene(sceneKey.SCE_CHARACTER_SELECTION, LoadSceneMode.Additive);
        while (!isCharacterMade()) yield return null;

        masterCameraRef.SetActive(true);
        unloadScene(sceneKey.SCE_CHARACTER_SELECTION);
        
        AsyncOperation overworldScene = loadScene(sceneKey.SCE_OVERWORLD, LoadSceneMode.Additive);
        
        loadingScreenRef.SetActive(true);
        do {
            yield return null;
        } 
        while (overworldScene.progress < .9f || GameObject.Find(overworldParentName) == null);
        loadingScreenRef.SetActive(false);

        //Rest of setup in normal void
        setupMain();
    }
    private bool audioIsReady() {
        bool ready = SCR_master_audio.returnInstance() != null;
        return ready;
    }
    private bool pressedStart() {
        return startPressed;
    }
    private bool isCharacterMade() {
        return playerPreset != null;
    }

    private void setupMain() { //Controls initial execution order in code and unifies setups
        //Get Required References
        SCR_master_generation mapRef = GetComponent<SCR_master_generation>();

        //Make Map
        if(seed == "") {
            seed = mapRef.randomSeed();
        }
        //Debug.Log("Map Seed: " + randSeed);

        //Setup externals
        SCR_master_generation.returnInstance().setup(seed, "Ground Tilemap", "Water Tilemap", mapSize);
        SCR_master_inventory_main.returnInstance().setup(inventorySize.x, inventorySize.y);
        SCR_master_crafting.returnInstance().setup();

        //Make Player
        Instantiate(playerPrefab, mapRef.startPos(), Quaternion.identity, GameObject.Find(playerParent).transform);
        SCR_player_main.returnInstance().setup(playerPreset);
    }
    #endregion
    #region Publics
    public void whatMusic() {
        SCR_master_audio.returnInstance().playRandomMusic(SCR_master_audio.sfx.MUSIC_CALM); //Could use this to get different music depending on the situation
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
    public void startPlay() {
        startPressed = true;
    }
    public void changeSeed(string value) {
        seed = value;
    }
    #endregion
    #region Clean Scenes
    private string getSceneClean(sceneKey input) {
        return formattedScenes[input];
    }
    private void moveToScene(GameObject obj, sceneKey input) {
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByName(formattedScenes[input]));
    }
    public AsyncOperation loadScene(sceneKey input, LoadSceneMode mode) {
        var scene = SceneManager.LoadSceneAsync(formattedScenes[input], mode);
        return scene;
    }
    public void unloadScene(sceneKey input) {
        SceneManager.UnloadSceneAsync(formattedScenes[input]);
    }
    #endregion
}
