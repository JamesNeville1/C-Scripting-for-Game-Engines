using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note: After modules like C++ for Engines, I have learned that composition is often more effective than inheritance.
//I would likely have a single base class, utilising things like interfaces if I were to do this again.

[CreateAssetMenu(fileName = "SCO_item_resource_", menuName = "ScriptableObjects/Items/Resource")]
public class SCO_item : ScriptableObject {
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Vector2Int[] spaces = new Vector2Int[1];
    public Vector2Int[] ReturnSpaces() {
        return spaces;
    }
    public Sprite ReturnItemSprite(){
        return itemSprite;
    }
    public string ReturnName() {
        return itemName;
    }
}
