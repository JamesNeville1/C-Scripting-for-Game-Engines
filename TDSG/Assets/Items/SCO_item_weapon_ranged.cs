using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_item_weapon_ranged_", menuName = "ScriptableObjects/Items/Weapons/Ranged")]
public class SCO_item_weapon_ranged : SCO_ABS_item_weapon {
    [SerializeField] protected float maxDistance;
    [SerializeField] protected Sprite projectile;
    public sealed override void attack(SCR_entity_attributes target) {
        //IF DIST FROM ME IS OK
        target.health.current -= attribute;
    }
}