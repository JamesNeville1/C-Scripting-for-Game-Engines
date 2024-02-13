using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SCR_master_character_selection : MonoBehaviour {
    [SerializeField] [MyReadOnly] private SCO_character_preset selected;

    [SerializeField] private List<SCO_character_preset> presets;
    [SerializeField] private GameObject selectableCharacterTemplate;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject selectedDisplay;
    [SerializeField] private TextMeshProUGUI presetInfoText;

    private void Start() {
        setup(); //Public
    }

    public void setup() {
        selectedDisplay.SetActive(false);

        foreach (var preset in presets) {
            GameObject newButton = Instantiate(selectableCharacterTemplate, buttonParent);
            
            newButton.GetComponentsInChildren<Image>()[1].sprite = preset.returnCharacterSelectionSprite();
            
            newButton.GetComponent<Button>().onClick.AddListener(() => onClickPreset(preset, newButton.transform));

            TextMeshProUGUI nameTMP = newButton.GetComponentInChildren<TextMeshProUGUI>();
            nameTMP.raycastTarget = false;
            nameTMP.text = preset.returnName();
        }
    }

    private void onClickPreset(SCO_character_preset preset, Transform buttonTransform) {
        selected = preset;
        SCR_master_audio.returnInstance().playRandomEffect(SCR_master_audio.sfx.GATHER_SOFT);
        selectedDisplay.SetActive(true);
        selectedDisplay.transform.position = buttonTransform.position;
        presetInfoText.text = makePresetDescription(preset.returnStartingStats(), preset.returnFlavourText());
    }

    private string makePresetDescription(SCR_ABS_attributes.entStats stats, string flavourText) {
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
        SCR_master_main.returnInstance().setPlayerPreset(selected);
    }
}