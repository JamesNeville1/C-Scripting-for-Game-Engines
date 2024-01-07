using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_item_edible_food_", menuName = "ScriptableObjects/Items/Edible/Food")]
public class SCO_item_edible_food : SCO_ABS_item_edible {
    public sealed override void eat(SCR_entity_attributes who) {
        who.hunger.adjust(attribute);
    }
}
