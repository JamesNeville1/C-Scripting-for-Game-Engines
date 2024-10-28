using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SCR_master_audio : MonoBehaviour {
    public enum sfx { //All sound effect and music types
        WALK_STEP,
        HURT_PLAYER,
        GATHER_HARD,
        GATHER_MEDIUM,
        GATHER_SOFT,
        GATHER_OTHER,
        ON_DEATH,
        EAT,
        CRAFT,
        DRINK,
    }

    public enum music {
        CALM,
        BATTLE,
        MENU
    }

    [SerializedDictionary("ID", "Clips")] [SerializeField]
    private SerializedDictionary<sfx, AudioClip[]> sfxs = new SerializedDictionary<sfx, AudioClip[]>(); //Hold audio clips, called via enum

    [SerializedDictionary("ID", "Clips")] [SerializeField]
    private SerializedDictionary<music, AudioClip[]> musicDict = new SerializedDictionary<music, AudioClip[]>(); //Hold audio clips, called via enum

    //Having these seperate allows me to control their individual volume
    [SerializeField] private AudioSource sfxSource; //Audio source, in external scene to reduce strain on game
    [SerializeField] private AudioSource musicSource; //Play music in seperate source
    
    #region Set Instance
    private static SCR_master_audio instance;
    public static SCR_master_audio returnInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }
    #endregion
    #region Play
    public void playOneEffect(sfx toPlay, float volume = 1f) { //Play single effect
        sfxSource.PlayOneShot(sfxs[toPlay][0], volume);
    }
    public void playRandomEffect(sfx toPlay, float volume = 1f) { //Play single effect in array, randomly selected
        int rand = UnityEngine.Random.Range(0, sfxs[toPlay].Length);
        sfxSource.PlayOneShot(sfxs[toPlay][rand], volume);
    }
    public void playRandomMusic(music toPlay, float volume = 1f) {
        StopCoroutine(findNewSong());
        musicSource.Stop();

        int rand = UnityEngine.Random.Range(0, musicDict[toPlay].Length);
        musicSource.PlayOneShot(musicDict[toPlay][rand], volume);
        //Debug.Log($"Now Playing: {sfxs[toPlay][rand].name}");
        StartCoroutine(findNewSong()); //Find new songe from master, would allow me to have additional complexity, for example if it were winter, a more appropriate song would play
    }
    #endregion
    #region Find New Song
    private IEnumerator findNewSong() { //Wait untill finished, ping when done
        UnityEvent ping = new UnityEvent();
        ping.AddListener(() => SCR_master_main.returnInstance().whatMusic());

        while (musicSource.isPlaying) {
            yield return null;
        }

        ping.Invoke();
    }
    #endregion
    #region Change Volume
    //Change volume - Used in menu
    public void changeSFXVolume(float value) {
        sfxSource.volume = value;
    }
    public void changeMusicVolume(float value) {
        musicSource.volume = value;
    }
    #endregion
}
