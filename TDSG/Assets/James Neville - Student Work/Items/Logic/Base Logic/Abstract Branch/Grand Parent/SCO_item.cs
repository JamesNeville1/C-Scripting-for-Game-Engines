using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_item_resource_", menuName = "ScriptableObjects/Items/Resource")]
public class SCO_item : ScriptableObject {
    [SerializeField] private int id;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Color displayBlockColour;
    [SerializeField] private Vector2Int[] spaces = new Vector2Int[1];
    public Sprite returnSprite() {
        return sprite;
    }
    public Vector2Int[] returnSpaces() {
        return spaces;
    }
    public Color returnColor(){
        return displayBlockColour;
    }
    public string returnName() {
        return itemName;
    }
    public int returnID() {
        return id;
    }
}
