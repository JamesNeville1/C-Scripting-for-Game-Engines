using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SCO_ABS_item_useable_on_entity : SCO_item {
    [SerializeField] protected int attribute = 0;
    [SerializeField] private bool breakOnUse = false;

    [SerializeField] private SCR_master_audio.sfx onUseSFX;
    [SerializeField] private bool shouldSFX = true; //Can't serialise var? so had to use this as work around 

    [SerializeField] private UnityAction onUseLogic;

    public abstract void UseOnEntity(SCR_player_attributes ent);

    public bool ReturnBreakOnUse() {
        return breakOnUse;
    }
    public SCR_master_audio.sfx ReturnOnUse() {
        return onUseSFX;
    }

    public bool ReturnShouldSFX() {
        return shouldSFX;
    }
}
