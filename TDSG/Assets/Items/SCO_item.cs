using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_item_resource_", menuName = "ScriptableObjects/Items/Resource")]
public class SCO_item : ScriptableObject {
    [SerializeField] private string itemName;
    [SerializeField] private int attribute = 0;
    [SerializeField] private Sprite sprite;

    public Sprite returnSprite() {
        return sprite;
    }
}
