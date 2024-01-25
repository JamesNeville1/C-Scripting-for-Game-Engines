using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SCR_entity_attributes : MonoBehaviour {
    [System.Serializable]
    public class attribute {
        [SerializeField][Tooltip("Current Int")] private int current;
        [SerializeField][Tooltip("Max Int")] private int max;

        UnityEvent onZero = new UnityEvent();

        public void adjust(int adjustBy) { //Adjust attribute, can not go over max
            if (current + adjustBy > max) {
                current = max;
            }
            else {
                current += adjustBy;
            }

            if (current <= 0) {
                onZero.Invoke();
            }
        }
        public void subscribe(UnityAction toSub) {
            onZero.AddListener(toSub);
        }
        public int returnCurrent() {
            return current;
        }
        public bool check() { if (current <= 0) return true; return false; }
    }
    [Tooltip("Has little effect in overworld, but is used in combat")] public attribute health;
    [Tooltip("Player will take damage after a few ticks if at 0")] public attribute hunger;
    [Tooltip("Player will do less damage in combat until they have slept")] public attribute tiredness;
    [Tooltip("Speed in battle, and multiplied in overworld")] public attribute speed;

    private void Awake() {
        SCR_player_main playerRef = SCR_player_main.returnInstance();

        health.subscribe(() => { playerRef.die("death"); });
    }
}
