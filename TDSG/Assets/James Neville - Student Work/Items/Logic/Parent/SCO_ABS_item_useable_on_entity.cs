using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCO_ABS_item_useable_on_entity : SCO_item, I_used_on_entity {
    [SerializeField] protected int attribute = 0;
    public abstract void useOnEntity(SCR_entity_attributes ent);
}
