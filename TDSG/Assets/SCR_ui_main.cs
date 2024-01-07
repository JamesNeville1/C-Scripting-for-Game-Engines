using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SCR_ui_main : MonoBehaviour {
    private static GameObject inventorySlot;
    private static GameObject inventorySlotParent;
    private static List<GameObject> inventorySlots = new List<GameObject>();

    private void Start () {
        inventorySlotParent = GameObject.Find("Inventory Slots");
        inventorySlot = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PRE_inventory_pannel.prefab");
    }

    public static void setupInventory(int size) {
        for (int i = 0; i < size; i++) {
            GameObject current = Instantiate(inventorySlot, inventorySlotParent.transform);
            current.name = inventorySlot.name + (i);
            current.transform.SetSiblingIndex(i);
            
        }
    }

    public static void setImage(int index, Sprite image) {
        inventorySlots[index].GetComponentsInChildren<Image>()[1].sprite = image;
    }
}
