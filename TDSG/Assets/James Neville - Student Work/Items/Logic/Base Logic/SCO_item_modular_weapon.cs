using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SCO_item_weapon_", menuName = "ScriptableObjects/Items/Weapons")]
public class SCO_item_modular_weapon : SCO_item{
    [SerializeField] private UnityEvent<aaaaa> onLeftClick ;
    [SerializeField] private UnityEvent onRightClick;

    [System.Serializable]
    public struct aaaaa {
        [SerializeField] public int a;
        [SerializeField] public int b;
    }

    public void a(aaaaa a) {

    }
    public void b() { 
    
    }

}