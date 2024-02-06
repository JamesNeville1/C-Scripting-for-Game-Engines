using IzzetUtils.IzzetAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SCR_overworld_player_attributes : SCR_ABS_attributes {

    //Delegates
    private voidDelegate startHungerHandler;
    private voidDelegate stopHungerHandler;
    private voidDelegate hungerRemoveHandler;

    private voidDelegate onDeathTimerLogicHandler;
    private voidDelegate onNoHungerTimerLogicHandler;

    [Header("Player - Attributes")]
    [SerializeField] [Tooltip("")] [MyReadOnly] private SCR_attribute hunger;
    //[SerializeField] [Tooltip("")] [MyReadOnly] private SCR_attribute stamina;

    [Header("Player - Keys & Timers")]
    [SerializeField][Tooltip("")] private float waitBeforeReloadSceneOnDeath;
    [SerializeField] [Tooltip("")] private float hungerDamageTimer;
    [SerializeField] [Tooltip("")] private float hungerTimer;

    protected override void setupSpecific() {
        //Base
        startHungerHandler = startHungerFunc;
        stopHungerHandler = stopHungerFunc;
        hungerRemoveHandler = delegate { hunger.adjust(-1); };
        
        //Timers
        onDeathTimerLogicHandler = delegate { SceneManager.LoadScene("SCE_menu"); SCR_master_timers.returnInstance().removeAll(SCR_master_timers.timerID.WAIT_AFTER_DEATH); };
        onNoHungerTimerLogicHandler = delegate {
            health.adjust(-1);
            if (health.returnCurrent() <= 0) SCR_master_timers.returnInstance().removeAll(SCR_master_timers.timerID.HUNGER_DAMAGE_TICK);
        };


        hunger = new SCR_attribute(stats.survival, () => startHungerHandler(), () => stopHungerHandler() );

        hunger.addUI(SCR_master_stats_display.returnInstance().returnHungerUI());
        health.addUI(SCR_master_stats_display.returnInstance().returnHealthUI());

        SCR_master_timers.returnInstance().subscribe(SCR_master_timers.timerID.HUNGER_TICK, () => hungerRemoveHandler(), hungerTimer);
    }
    #region Health
    protected override void onHealthEqualZeroFunc() {
        myAnimatior.play(SCR_unit_animation.AnimationType.DEATH); //Check if this should be in parent?
        SCR_player_main.returnInstance().readyToDie();

        SCR_master_timers.returnInstance().subscribe(
            SCR_master_timers.timerID.WAIT_AFTER_DEATH,
           () => onDeathTimerLogicHandler(), //Reload menu, and remove timer once done
            waitBeforeReloadSceneOnDeath
        );

        Debug.Log("Player Died");
    }
    #endregion
    #region Hunger
    public void startHungerFunc() {
        SCR_master_timers.returnInstance().subscribe(
            SCR_master_timers.timerID.HUNGER_DAMAGE_TICK,
            () => onNoHungerTimerLogicHandler(), //Reload menu, and remove timer once done
            hungerDamageTimer
        );

        speed = Mathf.RoundToInt(speed / 2);
        SCR_player_main.returnInstance().changeOverworldSpeed();
    }
    public void stopHungerFunc() {
        SCR_master_timers.returnInstance().removeAll(SCR_master_timers.timerID.HUNGER_DAMAGE_TICK);

        speed = stats.dexterity;
        SCR_player_main.returnInstance().changeOverworldSpeed();
    }
    #endregion
    #region Stamina
    private void onStaminaEqualZero() { }
    private void onStaminaNoZero() { }
    #endregion
    #region Returns
    public SCR_attribute returnHunger() {
        return hunger;
    }
    //public SCR_attribute returnStamina() { 
    //return stamina;
    //}
    #endregion
}