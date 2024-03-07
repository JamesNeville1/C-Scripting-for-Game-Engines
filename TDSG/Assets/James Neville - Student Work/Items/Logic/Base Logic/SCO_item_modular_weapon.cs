using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SCO_item_weapon_", menuName = "ScriptableObjects/Items/Weapons")]
public class SCO_item_modular_weapon : SCO_item {
    [Header("Universal Weapon Vars")]
    [SerializeField] private int damage;
    
    [Header("Weapon Events")]
    [SerializeField] private UnityEvent onLeftClick;
    [SerializeField] private UnityEvent onRightClick;
}