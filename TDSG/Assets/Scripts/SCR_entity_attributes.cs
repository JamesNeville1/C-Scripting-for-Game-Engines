using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_entity_attributes : MonoBehaviour {
    [System.Serializable]
    public class attribute {
        public int current;
        public int max;

        public attribute(int current, int max) {
            this.current = current;
            this.max = max;
        }

        public void adjust(int adjustBy) {
            if (current + adjustBy > max) {
                current = max;
            }
            else {
                current += adjustBy;
            }
        }

        public bool check() { if (current <= 0) return true; return false; }
    }
    public attribute health;
    public attribute hunger;
    public attribute tiredness;
    public attribute speed;
}
