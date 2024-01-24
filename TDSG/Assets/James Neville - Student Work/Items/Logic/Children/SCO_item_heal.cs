using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_item_useable_heal_", menuName = "ScriptableObjects/Items/Useable/Heal")]
public class SCO_item_heal : SCO_ABS_item_useable_on_entity {
    public override void useOnEntity(SCR_entity_attributes ent) {
        ent.health.adjust(attribute);
    }
}
