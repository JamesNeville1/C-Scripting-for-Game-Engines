using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SCR_master_menu : MonoBehaviour {

    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject normalMenu;

    public void Handle_OnStart() {
        AnyButtonPress();
        SCR_master_main.instance.StartPlay();
    }
    public void Handle_OnOptions() {
        AnyButtonPress();
        optionsMenu.SetActive(true);
        normalMenu.SetActive(false);
    }
    public void Handle_OnCloseOptions() {
        AnyButtonPress();
        optionsMenu.SetActive(false);
        normalMenu.SetActive(true);
    }
    public void Handle_OnExit() {
        AnyButtonPress();
        Application.Quit();
    }
    public void Handle_OnEnterSeedTextField(TMP_InputField field) {
        SCR_master_main.instance.ChangeSeed(field.text);
    }
    public void Handle_OnChangeMusicVolumeSlider(Slider refer) {
        SCR_master_audio.instance.ChangeMusicVolume(refer.value);
    }
    public void Handle_OnChangeSFXVolumeSlider(Slider refer) {
        SCR_master_audio.instance.ChangeSFXVolume(refer.value);
    }

    private void AnyButtonPress() {
        SCR_master_audio.instance.PlayOneEffect(SCR_master_audio.sfx.GATHER_SOFT);
    }
}
