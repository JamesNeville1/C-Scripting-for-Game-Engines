using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using IzzetUtils;
using TMPro;

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
    [SerializeField] private TextMeshProUGUI infoText;

    [Header("Loading Related")]
    [SerializeField] private GameObject loadingScreenRef;

    [SerializedDictionary("ID", "Name")] [SerializeField]
    private SerializedDictionary<sceneKey, string> formattedScenes = new SerializedDictionary<sceneKey, string>();

    #region Set Instance
    public static SCR_master_main instance {get; private set;}

    private void Awake() {
        instance = this;
    }
    #endregion

    #region Unity
    private void Start() {
        StartCoroutine(Setup());
    }
    private IEnumerator ToggleUpdate() {
        Transform infoTextParent = infoText.transform.parent.transform;

        while(true) {
            infoTextParent.transform.position = IzzetMain.GetMousePos(Camera.main);

            yield return null;
        }
    }
    #endregion

    #region Setup
    private IEnumerator Setup() { //Setup Execution Order
        //Load Timer and Audio, wait till finished, setup after
        LoadScene(sceneKey.SCE_AUDIO_MANAGER, LoadSceneMode.Additive);
        masterCameraRef.SetActive(false);
        while (!AudioAndTimerAreReady()) yield return null;
        SCR_master_audio.instance.PlayRandomMusic(SCR_master_audio.music.MENU);

        //Load Menu, wait until start is pressed
        LoadScene(sceneKey.SCE_MENU, LoadSceneMode.Additive);
        while (!PressedStart()) yield return null;
        UnloadScene(sceneKey.SCE_MENU);

        //Load Character Selection, wait until selected
        LoadScene(sceneKey.SCE_CHARACTER_SELECTION, LoadSceneMode.Additive);
        while (SCR_master_character_selection.instance == null) yield return null;
        SCR_master_character_selection.instance.Setup();
        while (!IsCharacterMade()) yield return null;
        UnloadScene(sceneKey.SCE_CHARACTER_SELECTION);

        //Ready main camera
        masterCameraRef.SetActive(true);
        
        //Load overworld scene, Show loading screen until ready
        AsyncOperation overworldScene = LoadScene(sceneKey.SCE_OVERWORLD, LoadSceneMode.Additive);
        loadingScreenRef.SetActive(true);
        do yield return null;
        while (overworldScene.progress < .9f || GameObject.Find(overworldParentName) == null); //Note: progress max is .9f not 1f, this is NOT an error
        SCR_master_map.instance.setup(seed, groundTilemapName, mapSize);
        loadingScreenRef.SetActive(false);

        //Setup Inventory & Crafting
        SCR_master_inventory_main.instance.Setup(inventorySize.x, inventorySize.y);
        SCR_master_crafting.instance.setup();

        //Make Player
        Instantiate(playerPrefab, SCR_master_map.instance.StartPos(), Quaternion.identity, GameObject.Find(playerParent).transform);
        SCR_player_main.returnInstance().Setup(playerPreset);

        //Play Correct Music
        SCR_master_audio.instance.PlayRandomMusic(SCR_master_audio.music.CALM);

        //Start Update
        StartCoroutine(ToggleUpdate());
        infoText.raycastTarget = false;
    }

    #region Setup Yield Checks
    private bool AudioAndTimerAreReady() {
        bool ready = SCR_master_audio.instance != null && SCR_master_timers.instance != null;
        return ready;
    }
    private bool PressedStart() {
        return startPressed;
    }
    private bool IsCharacterMade() {
        return playerPreset != null;
    }
    #endregion

    #endregion

    #region Publics
    public void WhatMusic() {
        if (IsCharacterMade()) {
            SCR_master_audio.instance.PlayRandomMusic(SCR_master_audio.music.MENU);
        }
        else {
            SCR_master_audio.instance.PlayRandomMusic(SCR_master_audio.music.CALM);
        }
    }
    public bool IsPlayerCraftingActive() {
        return playerCraftingActive;
    }
    public void SetGatheringLocked(bool setTo) {
        playerCraftingActive = setTo;
    }
    public void SetPlayerPreset(SCO_character_preset preset) {
        playerPreset = preset;
    }
    public void StartPlay() {
        startPressed = true;
    }
    public void ChangeSeed(string value) {
        seed = value;
    }
    public void SetInfoText(string value, Color color) {
        infoText.text = value;
        infoText.color = color;
    }
    #endregion

    #region Clean Scenes
    private void MoveToScene(GameObject obj, sceneKey input) {
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByName(formattedScenes[input]));
    }
    public AsyncOperation LoadScene(sceneKey input, LoadSceneMode mode) {
        var scene = SceneManager.LoadSceneAsync(formattedScenes[input], mode);
        return scene;
    }
    public void UnloadScene(sceneKey input) {
        SceneManager.UnloadSceneAsync(formattedScenes[input]);
    }

    #endregion
}
