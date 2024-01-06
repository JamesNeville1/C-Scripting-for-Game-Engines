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

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer sr;

    public static SCO_gatherable.gatherableHook target; //What should be picked up
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

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
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(movement.x != 0 || movement.y != 0) animator.SetBool("isMoving", true);
        else animator.SetBool("isMoving", false);
        if (movement.x == -1) {
            sr.flipX = true;
        }
        else if (movement.x == 1){
            sr.flipX = false;
        }
        return movement;
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