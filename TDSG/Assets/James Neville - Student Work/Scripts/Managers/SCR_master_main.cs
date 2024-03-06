using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;

public class SCR_master_main : MonoBehaviour {
    #region Structs & Enums
    public enum sceneKey { SCE_MASTER, SCE_OVERWORLD, SCE_AUDIO_MANAGER, SCE_MENU, SCE_CHARACTER_SELECTION }
    #endregion

    [Header("Main - Game Heavily Realize On These")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector2Int inventorySize;
    [SerializeField] private Vector2Int mapSize;
    [Header("")]
    [SerializeField] [MyReadOnly] private SCO_character_preset playerPreset;
    [SerializeField] [MyReadOnly] private string seed;

    [Header("Object Names")]
    [SerializeField] private string overworldParentName;
    [SerializeField] private string playerParent;
    [SerializeField] private string groundTilemapName;

    [Header("Other")]
    [SerializeField] [MyReadOnly] private bool playerCraftingActive;
    [SerializeField] private GameObject masterCameraRef;
    [SerializeField] [MyReadOnly] private bool startPressed;

    [Header("Loading Related")]
    [SerializeField] private GameObject loadingScreenRef;

    [SerializedDictionary("ID", "Name")] [SerializeField]
    private SerializedDictionary<sceneKey, string> formattedScenes = new SerializedDictionary<sceneKey, string>();

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
        StartCoroutine(setup());
    }
    #endregion
    #region Setup
    private IEnumerator setup() {
        //Load Timer and Audio, wait till finished, setup after
        loadScene(sceneKey.SCE_AUDIO_MANAGER, LoadSceneMode.Additive);
        masterCameraRef.SetActive(false);
        while (!audioAndTimerAreReady()) yield return null;
        SCR_master_audio.returnInstance().playRandomMusic(SCR_master_audio.sfx.MUSIC_CALM);

        //Load Menu, wait until start is pressed
        loadScene(sceneKey.SCE_MENU, LoadSceneMode.Additive);
        while (!pressedStart()) yield return null;
        unloadScene(sceneKey.SCE_MENU);

        //Load Character Selection, wait until selected
        loadScene(sceneKey.SCE_CHARACTER_SELECTION, LoadSceneMode.Additive);
        while (SCR_master_character_selection.returnInstance() == null) yield return null;
        SCR_master_character_selection.returnInstance().setup();
        while (!isCharacterMade()) yield return null;
        unloadScene(sceneKey.SCE_CHARACTER_SELECTION);

        //Ready main camera
        masterCameraRef.SetActive(true);
        
        //Load overworld scene, Show loading screen until ready
        AsyncOperation overworldScene = loadScene(sceneKey.SCE_OVERWORLD, LoadSceneMode.Additive);
        loadingScreenRef.SetActive(true);
        do yield return null;
        while (overworldScene.progress < .9f || GameObject.Find(overworldParentName) == null); //Note: progress max is .9f not 1f, this is NOT an error
        SCR_master_map.returnInstance().setup(seed, groundTilemapName, mapSize);
        loadingScreenRef.SetActive(false);

        //Setup Inventory & Crafting
        SCR_master_inventory_main.returnInstance().setup(inventorySize.x, inventorySize.y);
        SCR_master_crafting.returnInstance().setup();

        //Make Player
        Instantiate(playerPrefab, SCR_master_map.returnInstance().startPos(), Quaternion.identity, GameObject.Find(playerParent).transform);
        SCR_player_main.returnInstance().setup(playerPreset);
    }

    #region Setup Yield Checks
    private bool audioAndTimerAreReady() {
        bool ready = SCR_master_audio.returnInstance() != null && SCR_master_timers.returnInstance() != null;
        return ready;
    }
    private bool pressedStart() {
        return startPressed;
    }
    private bool isCharacterMade() {
        return playerPreset != null;
    }
    #endregion
    #region Other Utils
    //private void formatScenes() {
    //    formattedScenes.Add(sceneKey.SCE_MASTER, masterSceneName);
    //    formattedScenes.Add(sceneKey.SCE_OVERWORLD, overworldSceneName);
    //    formattedScenes.Add(sceneKey.SCE_AUDIO_MANAGER, audioSceneName);
    //    formattedScenes.Add(sceneKey.SCE_MENU, menuSceneName);
    //    formattedScenes.Add(sceneKey.SCE_CHARACTER_SELECTION, characterSelectionSceneName);
    //}
    #endregion
    #endregion
    #region Publics
    public void whatMusic() {
        SCR_master_audio.returnInstance().playRandomMusic(SCR_master_audio.sfx.MUSIC_CALM); //Could use this to get different music depending on the situation
    }
    public bool isPlayerCraftingActive() {
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
