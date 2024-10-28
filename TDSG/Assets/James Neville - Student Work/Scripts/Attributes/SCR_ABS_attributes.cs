using IzzetUtils.IzzetAttributes;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public abstract class SCR_ABS_attributes : MonoBehaviour {
    //#region Structs & Delegates 
    //[System.Serializable]
    //public struct entStats {
    //    public int athletics;
    //    public int survival;
    //    public int dexterity;
    //    public int endurance;

    //    public entStats(int athletics, int dexterity, int endurance, int survival) {
    //        this.athletics = athletics;
    //        this.survival = survival;
    //        this.endurance = endurance;
    //        this.dexterity = dexterity;
    //    }
    //    public entStats(entStats toPass) {
    //        this.dexterity = toPass.dexterity;
    //        this.endurance = toPass.endurance;
    //        this.survival = toPass.survival;
    //        this.athletics = toPass.athletics;
    //    }
    //}

    //protected delegate void voidDelegate();
    //#endregion

    ////Delegates
    //private voidDelegate onHealthEqualZeroHandler;

    //[Header("Stats")]
    //public entStats stats;

    //[Header("Universal Attributes")]
    //[SerializeField] [Tooltip("")] [MyReadOnly] protected SCR_attribute health;
    //[Header("")]
    //[SerializeField] [Tooltip("")] [MyReadOnly] protected int speed;

    //[Header("Read Only")]
    //protected SCR_unit_animation myAnimatior;

    //protected abstract void setupSpecific();
    //protected abstract void onHealthEqualZeroFunc();

    //public void setupUniversal(entStats stats) {
    //    onHealthEqualZeroHandler = onHealthEqualZeroFunc;

    //    //Component
    //    myAnimatior = GetComponent<SCR_unit_animation>();

    //    //
    //    this.stats = stats;

    //    //
    //    health = new SCR_attribute(calculateHealth(this.stats), () => onHealthEqualZeroHandler());
    //    speed = calculateSpeed(this.stats);

    //    //
    //    setupSpecific();
    //}

    //public int returnSpeed() {
    //    return speed;
    //}
    //public SCR_attribute returnHealth() {
    //    return health;
    //}

    //protected int calculateHealth(entStats stats) {
    //    return stats.endurance + stats.athletics;
    //}
    //protected int calculateSpeed(entStats stats) {
    //    return stats.dexterity + stats.athletics;
    //}
}
