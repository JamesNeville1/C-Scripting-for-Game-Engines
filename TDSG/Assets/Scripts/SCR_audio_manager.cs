using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_audio_manager : MonoBehaviour {
    private static AudioSource source;

    private void Awake() {
        source = GetComponent<AudioSource>();
    }
    public static void playEffect(AudioClip clip) {
        source.PlayOneShot(clip);
    }
    public static void playEffect(AudioClip[] clips) {
        int rand = Random.Range(0, clips.Length);
        source.PlayOneShot(clips[rand]);
    }
}
