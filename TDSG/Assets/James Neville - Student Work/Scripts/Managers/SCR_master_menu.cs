using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SCR_master_menu : MonoBehaviour {

    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject normalMenu;

    public void onStart() {
        onAnyButtonPress();
        SCR_master_main.returnInstance().startPlay();
    }
    public void onOptions() {
        onAnyButtonPress();
        optionsMenu.SetActive(true);
        normalMenu.SetActive(false);
    }
    public void onCloseOptions() {
        onAnyButtonPress();
        optionsMenu.SetActive(false);
        normalMenu.SetActive(true);
    }
    public void onExit() {
        onAnyButtonPress();
        Application.Quit();
    }
    public void onEnterSeedTextField(TMP_InputField field) {
        SCR_master_main.returnInstance().changeSeed(field.text);
    }
    public void onChangeMusicVolumeSlider(Slider refer) {
        SCR_master_audio.returnInstance().changeMusicVolume(refer.value);
    }
    public void onChangeSFXVolumeSlider(Slider refer) {
        SCR_master_audio.returnInstance().changeSFXVolume(refer.value);
    }

    private void onAnyButtonPress() {
        SCR_master_audio.returnInstance().playOneEffect(SCR_master_audio.sfx.GATHER_SOFT);
    }
}
