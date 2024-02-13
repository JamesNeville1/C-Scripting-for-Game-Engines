using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SCR_master_character_selection : MonoBehaviour {
    [SerializeField] [MyReadOnly] private SCO_character_preset selected;

    [SerializeField] private List<SCO_character_preset> presets;
    [SerializeField] private GameObject selectableCharacterTemplate;
    [SerializeField] private Transform buttonParent;

    private void Start() {
        setup(); //Public
    }

    public void setup() {
        foreach (var preset in presets) {
            GameObject newButton = Instantiate(selectableCharacterTemplate, buttonParent);
            newButton.GetComponentsInChildren<Image>()[1].sprite = preset.returnCharacterSelectionSprite();
            newButton.GetComponent<Button>().onClick.AddListener(delegate { selected = preset; Debug.Log(preset.name); });
        }
    }

    public void onConfirm() {
        SCR_master_main.returnInstance().setPlayerPreset(selected);
    }
}