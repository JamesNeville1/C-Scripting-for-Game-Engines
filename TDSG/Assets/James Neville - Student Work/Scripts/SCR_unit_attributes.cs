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
        //Component
        myAnimatior = GetComponent<SCR_unit_animation>();

        //Temp Stats
        stats = new entStats(5, 5, 5, 5);

        attributes.speed = stats.dexterity;

        if (isOverworldPlayer()) {
            overworldPlayerSetup();
            hungerTicksSetup();
        }
        else {
            unitSetup();
        }
    }

    private void overworldPlayerSetup() {
        SCR_player_main.returnInstance().changeOverworldSpeed();

        attributes.health = new SCR_attribute(stats.athletics, delegate { onHealthEqualZeroOverworldPlayer(); });
        attributes.hunger = new SCR_attribute(
            stats.survival, 
            delegate { onHungerEqualZeroOverworldPlayer(); }, 
            delegate { stopBeingHungeryOverworldPlayer(); }
        );

        attributes.health.addUI(SCR_master_stats_display.returnInstance().returnHealthUI());
        attributes.hunger.addUI(SCR_master_stats_display.returnInstance().returnHungerUI());
    }
    private void unitSetup() {
        attributes.health = new SCR_attribute(stats.athletics, delegate { onHealthEqualZero(); });
    }

    #region When Health is Zero
    private void onHealthEqualZero() {
        myAnimatior.play(SCR_unit_animation.AnimationType.DEATH);

        SCR_master_timers.returnInstance().subscribe(
            "End_Health",
            delegate { SceneManager.LoadScene("SCE_menu"); SCR_master_timers.returnInstance().removeAll("End_Health"); }, //Reload menu, and remove timer once done
            2
        );

        Debug.Log("Player Died");
    }

    private void onHealthEqualZeroOverworldPlayer() {
        SCR_player_main.returnInstance().readyToDie();
        onHealthEqualZero();
    }
    #endregion
    #region When Hunger is Zero
    private void onHungerEqualZeroOverworldPlayer() {
        SCR_master_timers.returnInstance().subscribe(
            "End_Hunger",
            delegate { 
                attributes.health.adjust(-1);
                if (attributes.health.returnCurrent() <= 0) SCR_master_timers.returnInstance().removeAll("End_Hunger"); }, //Reload menu, and remove timer once done
            SCR_player_main.returnInstance().returnTimeBetweenHungerDamageTicks()
        );;
        attributes.speed = Mathf.RoundToInt(attributes.speed / 2); 
        SCR_player_main.returnInstance().changeOverworldSpeed();
        Debug.Log("Player: Too Hungry");
    }
    #endregion
    #region When hunger is greater than zero, after being zero
    private void stopBeingHungeryOverworldPlayer() {
        SCR_master_timers.returnInstance().removeAll("End_Hunger");
        attributes.speed = stats.dexterity;
        SCR_player_main.returnInstance().changeOverworldSpeed();
        Debug.Log("Player: No Longer Hungry");
    }
    #endregion
    #region Hunger Ticks
    private void hungerTicksSetup() {
        float timeBetweenHungerTicks = SCR_player_main.returnInstance().returnTimeBetweenHungerTicks();

        SCR_master_timers.returnInstance().subscribe("Normal_Hungry", delegate { attributes.hunger.adjust(-1); }, timeBetweenHungerTicks);
    }
    #endregion
    private bool isOverworldPlayer() {
        return SCR_player_main.returnInstance().gameObject == this.gameObject;
    }
}
