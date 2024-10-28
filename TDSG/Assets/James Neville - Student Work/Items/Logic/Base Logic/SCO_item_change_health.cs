using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_item_useable_health_", menuName = "ScriptableObjects/Items/Useable/DamageOrHeal")]
public class SCO_item_change_health : SCO_ABS_item_useable_on_entity {
    public override void useOnEntity(SCR_overworld_player_attributes ent) {
        ent.returnHealth().adjust(attribute);
    }
}
