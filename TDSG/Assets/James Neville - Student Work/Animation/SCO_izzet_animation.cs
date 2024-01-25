using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_izzet_animation", menuName = "ScriptableObjects/Animation")]
public class SCO_izzet_animation : ScriptableObject {
    [System.Serializable]
    private struct frameStr {
        public Sprite sprite;
        public float timeAfter;
    }

    [SerializeField] private frameStr[] frames;

    public IEnumerator play(SpriteRenderer sr) {
        foreach (frameStr frame in frames) {
            sr.sprite = frame.sprite;
            yield return new WaitForSeconds(frame.timeAfter);
        }
    }
}
