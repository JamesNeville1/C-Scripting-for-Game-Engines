using IzzetUtils.IzzetAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SCR_player_attributes : MonoBehaviour {

    #region Structs & Delegates 
    [System.Serializable]
    public struct entStats
    {
        public int athletics;
        public int survival;
        public int dexterity;
        public int endurance;

        public entStats(int athletics, int dexterity, int endurance, int survival)
        {
            this.athletics = athletics;
            this.survival = survival;
            this.endurance = endurance;
            this.dexterity = dexterity;
        }
        public entStats(entStats toPass)
        {
            this.dexterity = toPass.dexterity;
            this.endurance = toPass.endurance;
            this.survival = toPass.survival;
            this.athletics = toPass.athletics;
        }
    }

    protected delegate void voidDelegate();
    #endregion

    //Delegates
    private voidDelegate onHealthEqualZeroHandler;

    [Header("Stats")]
    public entStats stats;

    [Header("Attributes")]
    [SerializeField][Tooltip("")][MyReadOnly] protected SCR_attribute health;
    [SerializeField][Tooltip("")][MyReadOnly] protected int speed;
    [SerializeField][Tooltip("")][MyReadOnly] private SCR_attribute hunger;

    [Header("Read Only")]
    protected SCR_unit_animation myAnimatior;

    //Delegates
    private voidDelegate startHungerHandler;
    private voidDelegate stopHungerHandler;
    private voidDelegate hungerRemoveHandler;

    private voidDelegate onDeathTimerLogicHandler;
    private voidDelegate onNoHungerTimerLogicHandler;

    public void SetupUniversal(entStats stats)
    {
        onHealthEqualZeroHandler = OnHealthEqualZeroFunc;

        //Component
        myAnimatior = GetComponent<SCR_unit_animation>();

        //
        this.stats = stats;

        //
        health = new SCR_attribute(CalculateHealth(this.stats), () => onHealthEqualZeroHandler());
        speed = CalculateSpeed(this.stats);

        //
        startHungerHandler = StartHungerFunc;
        stopHungerHandler = StopHungerFunc;
        hungerRemoveHandler = delegate { hunger.Adjust(-1); };

        //Timers
        onDeathTimerLogicHandler = delegate {
            SCR_master_timers.instance.RemoveAll(SCR_master_timers.timerID.WAIT_AFTER_DEATH);
            SCR_master_main.instance.LoadScene(SCR_master_main.sceneKey.SCE_MASTER, LoadSceneMode.Single);
        };
        onNoHungerTimerLogicHandler = delegate {
            health.Adjust(-1);
            if (health.ReturnCurrent() <= 0) SCR_master_timers.instance.RemoveAll(SCR_master_timers.timerID.HUNGER_DAMAGE_TICK);
        };

        //Setup hunger
        hunger = new SCR_attribute(CalculateHunger(), () => startHungerHandler(), () => stopHungerHandler());

        //Setup UI
        hunger.AddUI(SCR_master_stats_display.returnInstance().ReturnHungerUI());
        health.AddUI(SCR_master_stats_display.returnInstance().ReturnHealthUI());

        //Start Hunger Ticks
        SCR_master_timers.instance.Subscribe(SCR_master_timers.timerID.HUNGER_TICK, () => hungerRemoveHandler());
    }
    #region Speed
    public int ReturnSpeed()
    {
        return speed;
    }
    protected int CalculateSpeed(entStats stats)
    {
        return stats.dexterity + stats.athletics;
    }
    #endregion
    #region Health
    public SCR_attribute ReturnHealth() {
        return health;
    }
    protected int CalculateHealth(entStats stats) {
        return stats.endurance + stats.athletics;
    }
    private void OnHealthEqualZeroFunc() {
        myAnimatior.play(SCR_unit_animation.AnimationType.DEATH); //Check if this should be in parent?
        SCR_player_main.returnInstance().readyToDie();

        SCR_master_timers.instance.Subscribe(
            SCR_master_timers.timerID.WAIT_AFTER_DEATH,
           () => onDeathTimerLogicHandler() //Reload menu, and remove timer once done
        );

        Debug.Log("Player Died");
    }
    #endregion
    #region Hunger
    public void StartHungerFunc() {
        SCR_master_timers.instance.Subscribe(
            SCR_master_timers.timerID.HUNGER_DAMAGE_TICK,
            () => onNoHungerTimerLogicHandler() //Reload menu, and remove timer once done
        );

        speed = Mathf.RoundToInt(speed / 2);
        SCR_player_main.returnInstance().changeOverworldSpeed();
    }
    public void StopHungerFunc() {
        SCR_master_timers.instance.RemoveAll(SCR_master_timers.timerID.HUNGER_DAMAGE_TICK);

        speed = CalculateSpeed(stats);
        SCR_player_main.returnInstance().changeOverworldSpeed();
    }
    private int CalculateHunger() {
        return stats.survival;
    }
    #endregion
    #region Returns
    public SCR_attribute ReturnHunger() {
        return hunger;
    }
    #endregion
}