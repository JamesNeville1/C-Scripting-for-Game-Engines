using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SCO_item_edible_healing_", menuName = "ScriptableObjects/Items/Edible/Healing")]
public class SCO_item_edible_healing : SCO_ABS_item_edible
{
    public sealed override void eat(SCR_entity_attributes who, int amount) {
        //Who.health +
    }
}
