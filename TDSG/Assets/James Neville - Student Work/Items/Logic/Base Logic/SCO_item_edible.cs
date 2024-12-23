using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_item_useable_edible_", menuName = "ScriptableObjects/Items/Useable/Edible")]
public class SCO_item_edible : SCO_ABS_item_useable_on_entity {
    public override void useOnEntity(SCR_player_attributes ent) {
        ent.returnHunger().adjust(attribute);
    }
}