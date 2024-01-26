using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCO_ABS_item_useable_on_entity : SCO_item {
    [SerializeField] protected int attribute = 0;
    [SerializeField] private bool breakOnUse = false;
    public abstract void useOnEntity(SCR_entity_attributes ent);

    public bool returnBreakOnUse() {
        return breakOnUse;
    }
}
