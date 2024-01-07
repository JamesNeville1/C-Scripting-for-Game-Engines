using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SCR_player_main : MonoBehaviour {

    [Header("Main")]

    [SerializeField]
    private float defaultSpeed;

    [Header("Components")]

    [SerializeField] [SCR_utils.customAttributes.ReadOnly]
    private Rigidbody2D rb;

    [SerializeField] [SCR_utils.customAttributes.ReadOnly]
    private Animator animator;

    [SerializeField] [SCR_utils.customAttributes.ReadOnly]
    private SpriteRenderer sr;

    [Header("Inventory Vars")]

    [SerializeField]
    private int inventorySize;

    [SerializeField]
    private GameObject inventorySlot;

    [SerializeField] [SCR_utils.customAttributes.ReadOnly]
    private GameObject inventorySlotsParent;

    [SerializeField]
    private SCO_item[] startingItems;

    private static SCR_player_inventory inventory;

    public static SCO_gatherable.gatherableHook target; //What should be picked up
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        GameObject inv = new GameObject("Inventory");
        inv.transform.parent = gameObject.transform;
        inventory = inv.AddComponent<SCR_player_inventory>();

        inventorySlotsParent = GameObject.Find("Inventory Slots");
        inventory.setUp(inventorySize, startingItems.ToList(), inventorySlot, inventorySlotsParent.transform);
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
        public class inventoryData {
            public SCO_item item;
            public Image display;

            public inventoryData(Image display, SCO_item item = null) {
                this.item = item;
                this.display = display;
            }

            public void changeDisplay(Sprite sprite) {
                this.display.sprite = sprite;

            }
        }
        public static List<inventoryData> inventory = new List<inventoryData>(); //Display and Scriptable Object

        internal void setUp(int inventorySize, List<SCO_item> startingItems, GameObject uiSlot, Transform parent) {
            for (int i = 0; i < inventorySize; i++) {
                Image image = Instantiate(uiSlot, parent).GetComponentsInChildren<Image>()[1];
                inventory.Add(new inventoryData(image));
            }

            foreach(SCO_item item in startingItems) {
                addItem(item);
            }
        }

        internal static int findFreeSlot() { //Find free slot in inventory array, if can't, return minus one
            for (int i = 0; i < inventory.Count; i++) {
                if (inventory[i].item == null) return i;
            }
            return -1;
        }
        public static bool addItem(SCO_item item) {
            int id = findFreeSlot();
            if(id == -1) return false;
            inventory[id].item = item;
            inventory[id].changeDisplay(item.returnSprite());
            return true;
        }
        public static bool removeItem(int index) {
            if (inventory[index].item == null) return false;

            inventory[index].item = null;
            inventory[index].changeDisplay(null);
            return true;
        }

        public static void dump() {
            for (int i = 0; i < inventory.Count; i++) {
                removeItem(i);
            }
        }

        private void Update() { 
            if(Input.GetKeyDown(KeyCode.Delete)) {
                dump();
            }
        }
    }
}