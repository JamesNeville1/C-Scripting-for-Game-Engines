using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SCR_master_character_selection : MonoBehaviour {
    [Header("Main")]
    [SerializeField] [MyReadOnly] private SCO_character_preset selected;
    [SerializeField] private List<SCO_character_preset> presets;

    [Header("UI Related")]
    [SerializeField] [Tooltip("Button Prefab")] private GameObject selectableCharacterTemplate;
    [SerializeField] private Transform buttonParent;
    [SerializeField] [Tooltip("Goes to Selected Preset")] private GameObject selectedDisplay;
    [SerializeField] [Tooltip("Reference to text that shows stats and description")]private TextMeshProUGUI presetInfoText;

    #region Instance
    private static SCR_master_character_selection instance;
    public static SCR_master_character_selection returnInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }
    #endregion

    #region Setup
    public void setup() {
        selectedDisplay.SetActive(false);

        foreach (var preset in presets) { //Make a button for each preset
            GameObject newButton = Instantiate(selectableCharacterTemplate, buttonParent);
            
            newButton.GetComponentsInChildren<Image>()[1].sprite = preset.returnCharacterSelectionSprite();
            
            newButton.GetComponent<Button>().onClick.AddListener(() => onClickPreset(preset, newButton.transform));

            TextMeshProUGUI nameTMP = newButton.GetComponentInChildren<TextMeshProUGUI>();
            nameTMP.text = preset.returnName();

            nameTMP.raycastTarget = false;
        }
    }
    #endregion
    #region Logic
    private void onClickPreset(SCO_character_preset preset, Transform buttonTransform) {
        //Selected is now the preset, if player presses confirm game starts with that preset
        selected = preset;

        //All design related
        SCR_master_audio.returnInstance().playRandomEffect(SCR_master_audio.sfx.GATHER_SOFT);
        selectedDisplay.SetActive(true);
        selectedDisplay.transform.position = buttonTransform.position;
        presetInfoText.text = makePresetDescription(preset.returnStartingStats(), preset.returnFlavourText());
    }

    private string makePresetDescription(SCR_overworld_player_attributes.entStats stats, string flavourText) { //Here is the description shown
        string info = @$"
Endurance: {stats.endurance}

Dexterity: {stats.dexterity}

Survival: {stats.survival}

Athletics: {stats.athletics}


{flavourText}
        ";

        return info;
    }

    public void onConfirm() {
        SCR_master_main.returnInstance().setPlayerPreset(selected); //Confirm ends the corourtines waiting condition in master, the game will now start
    }
    #endregion
}