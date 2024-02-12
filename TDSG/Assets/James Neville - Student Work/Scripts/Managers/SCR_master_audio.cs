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
        MUSIC_CALM
    }

    [System.Serializable] private struct PASS_passStruct { public sfx key; public AudioClip[] clip; }
    [SerializeField] private PASS_passStruct[] PASS_sfxPasser; //Pass to sfxs

    private Dictionary<sfx, AudioClip[]> sfxs = new Dictionary<sfx, AudioClip[]>(); //Hold audio clips, called via enum

    private static AudioSource source; //Audio source, in external scene to reduce strain on game
    private AudioSource musicSource; //Play music in seperate source
    
    #region Set Instance
    private static SCR_master_audio instance;
    public static SCR_master_audio returnInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }
    #endregion

    #region Setup
    public void setup() {
        source = GetComponent<AudioSource>();
        musicSource = GetComponent<AudioSource>();

        foreach (PASS_passStruct passing in PASS_sfxPasser)
        {
            sfxs.Add(passing.key, passing.clip);
        }
        PASS_sfxPasser = null;
    }
    #endregion
    #region Play
    public void playOneEffect(sfx toPlay, float volume = 1f) { //Play single effect
        source.PlayOneShot(sfxs[toPlay][0], volume);
    }
    public void playRandomEffect(sfx toPlay, float volume = 1f) { //Play multiple effect
        int rand = UnityEngine.Random.Range(0, sfxs[toPlay].Length);
        source.PlayOneShot(sfxs[toPlay][rand], volume);
    }
    public void playRandomMusic(sfx toPlay, float volume = 1f) {
        StopCoroutine(findNewSong());
        musicSource.Stop();

        int rand = UnityEngine.Random.Range(0, sfxs[toPlay].Length);
        musicSource.PlayOneShot(sfxs[toPlay][rand], volume);
        //Debug.Log($"Now Playing: {sfxs[toPlay][rand].name}");
        StartCoroutine(findNewSong());
    }
    #endregion
    #region Find New Song
    private IEnumerator findNewSong() {
        UnityEvent ping = new UnityEvent();
        ping.AddListener(() => SCR_master_main.returnInstance().whatMusic());

        while (musicSource.isPlaying) {
            yield return null;
        }

        ping.Invoke();
    }
    #endregion
}
