using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_overworld_player_attributes : SCR_ABS_attributes {
    [Header("Player - Attributes")]
    [SerializeField] [Tooltip("")] [MyReadOnly] private SCR_attribute hunger;
    //[SerializeField] [Tooltip("")] [MyReadOnly] private SCR_attribute stamina;

    [Header("Player - Keys & Timers")]
    [SerializeField] [Tooltip("")] private string deathWaitTimerKey;
    [SerializeField][Tooltip("")] private float waitBeforeReloadSceneOnDeath;
    [Header("")]
    [SerializeField] [Tooltip("")] private string hungerDamageTimerKey;
    [SerializeField] [Tooltip("")] private float hungerDamageTimer;
    [Header("")]
    [SerializeField] [Tooltip("")] private string hungerTimerKey;
    [SerializeField] [Tooltip("")] private float hungerTimer;

    protected override void setupSpecific() {
        hunger = new SCR_attribute(stats.survival, delegate { startHunger(); }, delegate { stopHunger(); } );

        hunger.addUI(SCR_master_stats_display.returnInstance().returnHungerUI());
        health.addUI(SCR_master_stats_display.returnInstance().returnHealthUI());

        float timeBetweenHungerTicks = hungerTimer;
        SCR_master_timers.returnInstance().subscribe(hungerTimerKey, delegate { hunger.adjust(-1); }, timeBetweenHungerTicks);
    }
    #region Health
    protected override void onHealthEqualZero() {
        myAnimatior.play(SCR_unit_animation.AnimationType.DEATH); //Check if this should be in parent?
        SCR_player_main.returnInstance().readyToDie();

        SCR_master_timers.returnInstance().subscribe(
            deathWaitTimerKey,
            delegate { SceneManager.LoadScene("SCE_menu"); SCR_master_timers.returnInstance().removeAll(deathWaitTimerKey); }, //Reload menu, and remove timer once done
            waitBeforeReloadSceneOnDeath
        );

        Debug.Log("Player Died");
    }
    #endregion
    #region Hunger
    private void startHunger() {
        SCR_master_timers.returnInstance().subscribe(
            hungerDamageTimerKey,
            delegate {
                health.adjust(-1);
                if (health.returnCurrent() <= 0) SCR_master_timers.returnInstance().removeAll(hungerDamageTimerKey);
            }, //Reload menu, and remove timer once done
            hungerDamageTimer
        );

        speed = Mathf.RoundToInt(speed / 2);
        SCR_player_main.returnInstance().changeOverworldSpeed();
    }
    private void stopHunger() {
        SCR_master_timers.returnInstance().removeAll(hungerDamageTimerKey);

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