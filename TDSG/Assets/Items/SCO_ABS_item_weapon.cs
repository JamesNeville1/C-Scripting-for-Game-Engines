using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SCO_ABS_item_weapon : SCO_item {
    [SerializeField] protected int damage;
    public abstract void attack(SCR_entity_attributes target);
}
