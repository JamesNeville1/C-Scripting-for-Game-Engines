using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CreateAssetMenu(fileName = "SCO_item_resource_", menuName = "ScriptableObjects/Items/Resource")]
public class SCO_item : ScriptableObject {
    [SerializeField] private string itemName;
    [SerializeField] private int attribute = 0;
}

#region edible
public abstract class SCO_item_edible : SCO_item {
    public abstract void eat(SCR_entity_attributes who, int amount);
}

[CreateAssetMenu(fileName = "SCO_item_edible_food_", menuName = "ScriptableObjects/Items/Edible/Food")]
public class SCO_item_edible_food : SCO_item_edible {
    public sealed override void eat(SCR_entity_attributes who, int amount) {
        //Who.hunger +
    }
}
[CreateAssetMenu(fileName = "SCO_item_edible_healing_", menuName = "ScriptableObjects/Items/Edible/Healing")]
public class SCO_item_edible_healing : SCO_item_edible {
    public sealed override void eat(SCR_entity_attributes who, int amount) {
        //Who.health +
    }
}
#endregion

#region Weapons
public abstract class SCO_item_weapon : SCO_item {
    [SerializeField] protected int damage;
    public abstract void attack(SCR_entity_attributes target);
}

[CreateAssetMenu(fileName = "SCO_item_weapon_melee_", menuName = "ScriptableObjects/Items/Weapons/Melee")]
public class SCO_item_weapon_melee : SCO_item_weapon {
    public sealed override void attack(SCR_entity_attributes target) {
        //Move towards and attack
    }
}
[CreateAssetMenu(fileName = "SCO_item_weapon_ranged_", menuName = "ScriptableObjects/Items/Weapons/Ranged")]
public class SCO_item_weapon_ranged : SCO_item_weapon {
    [SerializeField] protected float maxDistance;
    public sealed override void attack(SCR_entity_attributes target) {
        //Attack from a distance
    }
}
#endregion

