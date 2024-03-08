using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_item_resource_", menuName = "ScriptableObjects/Items/Resource")]
public class SCO_item : ScriptableObject {
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Vector2Int[] spaces = new Vector2Int[1];
    public Vector2Int[] returnSpaces() {
        return spaces;
    }
    public Sprite returnItemSprite(){
        return itemSprite;
    }
    public string returnName() {
        return itemName;
    }
}
