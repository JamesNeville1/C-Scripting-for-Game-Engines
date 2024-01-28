using IzzetUtils.IzzetAttributes;
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

        internal attribute(int max) {
            this.current = max;
            this.max = max;
        }

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

    [System.Serializable]
    private struct entStats {
        [MyReadOnly] public int athletics;
        [MyReadOnly] public int dexterity;
        [MyReadOnly] public int endurance;
        [MyReadOnly] public int survival;
        [MyReadOnly] public int luck;

        public entStats(int athletics, int dexterity, int endurance, int survival, int luck) {
            this.athletics = athletics;
            this.endurance = endurance;
            this.survival = survival;
            this.luck = luck;
            this.dexterity = dexterity;
        }
    }

    [Header("Attributes")]
    [Tooltip("Has little effect in overworld, but is used in combat")] public attribute health;
    [Tooltip("Player will take damage after a few ticks if at 0")] public attribute hunger;
    [Tooltip("Player will do less damage in combat until they have slept")] public attribute tiredness;
    [Tooltip("Speed in battle, and multiplied in overworld")] public int speed;

    [Header("Stats")]
    [SerializeField] private entStats stats;

    [Header("Internal Modifiers")]
    [SerializeField] private bool isPlayer; //I DONT LIKE THIS, CHANGE LATER
    [SerializeField] private int healthModif;
    [SerializeField] private int hungerModif;
    [SerializeField] private int tirednessModif;
    [SerializeField] private int speedModif;

    private void Awake() {
        //Temp
        stats = new entStats(5,5,5,5,5);
        //Temp

        getAttFromStat();

        if(isPlayer) {
            SCR_player_main playerRef = SCR_player_main.returnInstance();
            health.subscribe(() => { playerRef.die("death"); });
        }
    }

    private void getAttFromStat() { //All calculations for getting attributes
        health = new attribute(stats.endurance * healthModif);
        hunger = new attribute((stats.endurance + stats.survival) * hungerModif);
        tiredness = new attribute(Math.Clamp(Mathf.RoundToInt((stats.endurance + stats.survival) / tirednessModif), 1, int.MaxValue));
        speed = (stats.dexterity + stats.athletics) * speedModif;
    }
}
