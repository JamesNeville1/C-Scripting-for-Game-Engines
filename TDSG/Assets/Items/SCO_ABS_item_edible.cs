using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCO_ABS_item_edible : SCO_item {
    public abstract void eat(SCR_entity_attributes who, int amount);
}
