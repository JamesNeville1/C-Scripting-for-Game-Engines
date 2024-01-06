using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_player_main : MonoBehaviour {
    [SerializeField]
    private float defaultSpeed;
    
    private Rigidbody2D rb;

    private SCR_player_inventory attributes;

    [SerializeField]
    private SCO_item[] startingItems;

    [SerializeField]
    private int inventorySize;
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        
        GameObject inv = new GameObject("Inventory");
        inv.transform.parent = gameObject.transform;
        inv.AddComponent<SCR_player_inventory>().setUp(inventorySize, startingItems.ToList());
    }
    private void Update() {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if(input.x == 0 || input.y == 0) {
            rb.velocity = input * defaultSpeed;
        }
        else {
            rb.velocity = (input * defaultSpeed) * 0.71f;
        }

        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
    public class SCR_player_inventory : MonoBehaviour {
        public SCO_item[] inventory;

        internal void setUp(int inventorySize, List<SCO_item> startingItems) {
            inventory = new SCO_item[inventorySize];

            foreach(SCO_item item in startingItems) {
                int slotRef = findFreeSlot();
                if(slotRef != -1) inventory[slotRef] = item;
            }
        }

        int findFreeSlot() {
            for (int i = 0; i < inventory.Length; i++) {
                if (inventory[i] == null) return i;
            }
            return -1;
        }
    }
}