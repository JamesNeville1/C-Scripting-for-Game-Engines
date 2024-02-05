using IzzetUtils.IzzetAttributes;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public abstract class SCR_ABS_attributes : MonoBehaviour {
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

    [Header("Stats")]
    public entStats stats;

    [Header("Universal Attributes")]
    [SerializeField] [Tooltip("")] [MyReadOnly] protected SCR_attribute health;
    [SerializeField] [Tooltip("")] [MyReadOnly] protected int speed;

    [Header("Read Only")]
    protected SCR_unit_animation myAnimatior;

    protected abstract void setupSpecific();
    protected abstract void onHealthEqualZero();

    public void setupUniversal(int athletics, int dexterity, int endurance, int survival) {
        //Component
        myAnimatior = GetComponent<SCR_unit_animation>();

        //
        stats = new entStats(athletics, dexterity, endurance, survival);

        //
        health = new SCR_attribute(stats.endurance, delegate { onHealthEqualZero(); });
        speed = stats.dexterity;

        //
        setupSpecific();
    }

    public int returnSpeed() {
        return speed;
    }
    public SCR_attribute returnHealth() {
        return health;
    }
}
