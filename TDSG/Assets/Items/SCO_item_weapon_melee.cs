using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_item_weapon_melee_", menuName = "ScriptableObjects/Items/Weapons/Melee")]
public class SCO_item_weapon_melee : SCO_ABS_item_weapon {
    public sealed override void attack(SCR_entity_attributes target) {
        //Move towards and attack
    }
}