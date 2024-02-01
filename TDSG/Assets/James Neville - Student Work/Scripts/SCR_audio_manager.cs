using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_audio_manager : MonoBehaviour {
    private static AudioSource source; //Audio source, in external scene to reduce strain on game

    public enum sfx{
        WALK_STEP,
        HIT_ENEMY,
        HIT_SELF,
        GATHER_STONE,
        GATHER_WOOD,
        GATHER_MUSHROOM,
        GATHER_OTHER,
        ON_DEATH,
        EAT,
        CRAFT,
        DRINK,
    }

    [System.Serializable] private struct PASS_passStruct { public sfx key; public AudioClip[] clip; }

    private Dictionary<sfx, AudioClip[]> sfxs = new Dictionary<sfx, AudioClip[]>();
    [SerializeField] private PASS_passStruct[] PASS_sfxPasser;

    private static SCR_audio_manager instance;

    public static SCR_audio_manager returnInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;

        source = GetComponent<AudioSource>();

        foreach (PASS_passStruct passing in PASS_sfxPasser) {
            sfxs.Add(passing.key, passing.clip);
        }
        PASS_sfxPasser = null;
    }
    public void playOneEffect(sfx toPlay, float volume = 1f) { //Play single effect
        source.PlayOneShot(sfxs[toPlay][0], volume);
    }
    public void playManyEffect(sfx toPlay, float volume = 1f) { //Play multiple effect
        int rand = UnityEngine.Random.Range(0, sfxs[toPlay].Length);
        source.PlayOneShot(sfxs[toPlay][rand], volume);
    }
}
