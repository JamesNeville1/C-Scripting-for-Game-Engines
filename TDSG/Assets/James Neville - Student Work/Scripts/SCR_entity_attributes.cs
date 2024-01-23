using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_entity_attributes : MonoBehaviour {
    [System.Serializable]
    public class attribute {
        [Tooltip("Current Int")] public int current;
        [Tooltip("Max Int")] public int max;

        public attribute(int current, int max) { //Constructor
            this.current = current;
            this.max = max;
        }

        public void adjust(int adjustBy) { //Adjust attribute, can not go over max
            if (current + adjustBy > max) {
                current = max;
            }
            else {
                current += adjustBy;
            }
        }

        public bool check() { if (current <= 0) return true; return false; }
    }
    [Tooltip("Has little effect in overworld, but is used in combat")] public attribute health;
    [Tooltip("Player will take damage after a few ticks if at 0")] public attribute hunger;
    [Tooltip("Player will do less damage in combat until they have slept")] public attribute tiredness;
    [Tooltip("Speed in battle, and multiplied in overworld")] public attribute speed;
}
