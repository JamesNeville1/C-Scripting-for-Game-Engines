using IzzetUtils.IzzetAttributes;
using System;
using System.CodeDom.Compiler;
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

    [Header("Read Only")]
    SCR_unit_animation myAnimatior;

    public void setup() {
        stats = new entStats(5, 5, 5, 5);

        myAnimatior = GetComponent<SCR_unit_animation>();

        attributes.health = new SCR_attribute(stats.athletics, delegate { onHeathEqualZero(); });
        attributes.speed = stats.dexterity;

        if (isPlayer()) {
            SCR_player_main.returnInstance().changeOverworldSpeed();
            attributes.hunger = new SCR_attribute(stats.survival, delegate { onHungerEqualZeroPlayer(); });

            attributes.health.addUI(SCR_master_stats_display.returnInstance().returnHealthUI());
            attributes.hunger.addUI(SCR_master_stats_display.returnInstance().returnHungerUI());
        }
    }

    private void onHeathEqualZero() {
        myAnimatior.play(SCR_unit_animation.AnimationType.DEATH);
        
        if(isPlayer()) {
            SCR_player_main.returnInstance().readyToDie();
        }

        SCR_master_timers.returnInstance().subscribe(
            "End_Health",
            delegate { SceneManager.LoadScene("SCE_menu"); SCR_master_timers.returnInstance().removeAll("End_Health"); }, //Reload menu, and remove timer once done
            2
        );

        Debug.Log("Player Died");
    }

    private void onHungerEqualZeroPlayer() {
        SCR_master_timers.returnInstance().subscribe(
            "End_Hunger",
            delegate { 
                attributes.health.adjust(-1); Debug.Log($"Health now {attributes.health.returnCurrent()}");
                if (attributes.health.returnCurrent() <= 0) SCR_master_timers.returnInstance().removeAll("End_Hunger"); }, //Reload menu, and remove timer once done
            2
        );
        attributes.speed = Mathf.RoundToInt(attributes.speed / 2); 
        SCR_player_main.returnInstance().changeOverworldSpeed();
        Debug.Log("Player Too Hungry");
    }

    private bool isPlayer() {
        return SCR_player_main.returnInstance().gameObject == this.gameObject;
    }

    //Function onHungerEqualZeroPlayer & onHealthEqualZeroPlayer
}
