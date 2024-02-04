using IzzetUtils.IzzetAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SCR_unit_attributes : MonoBehaviour {
    [System.Serializable]
    public struct entStats {
        [MyReadOnly] public int athletics;
        [MyReadOnly] public int survival;
        [MyReadOnly] public int dexterity;
        [MyReadOnly] public int endurance;

        public entStats(int athletics, int dexterity, int endurance, int survival) {
            this.athletics = athletics;
            this.survival = survival;
            this.endurance = endurance;
            this.dexterity = dexterity;
        }
    }

    [System.Serializable]
    public struct entAttributes {
        [Tooltip("Has little effect in overworld, but is used in combat")] public SCR_attribute health;
        [Tooltip("Player will take damage after a few ticks if at 0")] public SCR_attribute hunger;
        [Tooltip("Player will do less damage in combat until they have slept")] public SCR_attribute tiredness;
        [Tooltip("Speed in battle, and multiplied in overworld")] public int speed;
    }

    [Header("Stats")]
    public entStats stats;

    [Header("Attributes")]
    public entAttributes attributes;

    private void Awake() {
        stats = new entStats(5, 5, 5, 5);

        setup();

        SCR_master_timers.returnInstance().subscribe(
            "Test", 
            delegate { 
                attributes.hunger.reduce(5); 
                Debug.Log("Timer Triggered");
                SCR_master_timers.returnInstance().removeAll("Test"); 
            }, 
            2);
    }

    private void setup() {
        attributes.health = new SCR_attribute(
            stats.athletics,
            delegate { 
                GetComponent<SCR_player_main>().enabled = false;
                GetComponent<SCR_unit_animation>().play(SCR_unit_animation.AnimationType.DEATH);
                SCR_master_timers.returnInstance().subscribe(
                    "Before_End",
                    delegate {
                        SceneManager.LoadScene("SCE_menu");
                    },
                    1
                );
                Debug.Log("ded"); 
            }
        );

        attributes.hunger = new SCR_attribute(
            stats.survival,
            delegate {
                SCR_master_timers.returnInstance().subscribe(
                    "Hunger Ticks",
                    delegate {
                        GetComponent<SCR_unit_animation>().play(SCR_unit_animation.AnimationType.HURT);
                        attributes.health.reduce(1);
                        print(attributes.health.returnCurrent());
                    },
                    1
                );

                Debug.Log("hrt"); 
            }
        );
    }
}
