using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_item_resource_", menuName = "ScriptableObjects/Items/Resource")]
public class SCO_item : ScriptableObject {
    [SerializeField] private string itemName;
    [SerializeField] internal int attribute = 0;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite displayBlock;
    [SerializeField] private Color displayBlockColour;
    [SerializeField] private Vector2[] spaces = new Vector2[1];
    public Sprite returnSprite() {
        return sprite;
    }

    public Vector2[] returnSpaces() {
        return spaces;
    }
}
