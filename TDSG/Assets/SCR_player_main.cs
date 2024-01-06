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

    private static SCR_player_inventory inventory;

    [SerializeField]
    private SCO_item[] startingItems;

    [SerializeField]
    private int inventorySize;

    public static SCO_gatherable.gatherableHook target; //What should be picked up
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        
        GameObject inv = new GameObject("Inventory");
        inv.transform.parent = gameObject.transform;
        inventory = inv.AddComponent<SCR_player_inventory>();
        inventory.setUp(inventorySize, startingItems.ToList());
    }
    private void Update() {
        Vector2 input = returnInput();
        movePlayer(input);
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    private Vector2 returnInput() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    private void movePlayer(Vector2 input) {
        if(input.x == 0 || input.y == 0) {
            rb.velocity = input * defaultSpeed;
        }
        else {
            rb.velocity = (input * defaultSpeed) * 0.71f;
        }
    }

    public class SCR_player_inventory : MonoBehaviour {
        public static SCO_item[] inventory;

        internal void setUp(int inventorySize, List<SCO_item> startingItems) {
            inventory = new SCO_item[inventorySize];

            foreach(SCO_item item in startingItems) {
                int slotRef = findFreeSlot();
                if(slotRef != -1) inventory[slotRef] = item;
            }
        }

        internal static int findFreeSlot() {
            for (int i = 0; i < inventory.Length; i++) {
                if (inventory[i] == null) return i;
            }
            return -1;
        }
        public static bool addItem(SCO_item item) {
            int id = findFreeSlot();
            if(id == -1) return false;
            inventory[id] = item;
            return true;
        }

        private void Update() {

        }
    }
}