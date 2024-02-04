using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SCR_master_audio : MonoBehaviour {
    private static AudioSource source; //Audio source, in external scene to reduce strain on game

    public enum sfx{ //All sound effect and music types
        WALK_STEP,
        HIT_ENEMY,
        HIT_PLAYER,
        GATHER_HARD,
        GATHER_MEDIUM,
        GATHER_SOFT,
        GATHER_OTHER,
        ON_DEATH,
        EAT,
        CRAFT,
        DRINK,
        MUSIC_BATTLE,
        MUSIC_BATTLE_WON,
        MUSIC_BATTLE_LOST,
        MUSIC_CALM
    }

    [System.Serializable] private struct PASS_passStruct { public sfx key; public AudioClip[] clip; }
    private Dictionary<sfx, AudioClip[]> sfxs = new Dictionary<sfx, AudioClip[]>();
    [SerializeField] private PASS_passStruct[] PASS_sfxPasser;
    private static SCR_master_audio instance;
    private AudioSource musicSource;

    public static SCR_master_audio returnInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;

        source = GetComponent<AudioSource>();
        musicSource = GetComponent<AudioSource>();

        foreach (PASS_passStruct passing in PASS_sfxPasser) {
            sfxs.Add(passing.key, passing.clip);
        }
        PASS_sfxPasser = null;
    }
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

    private IEnumerator findNewSong() {
        UnityEvent ping = new UnityEvent();
        ping.AddListener(() => SCR_master_main.returnInstance().whatMusic());

        while (musicSource.isPlaying) {
            yield return null;
        }

        ping.Invoke();
    }
}
