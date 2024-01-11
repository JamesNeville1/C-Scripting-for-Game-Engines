using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class SCR_player_main : MonoBehaviour {

    [Header("Main")]
    [SerializeField] [SCR_utils.customAttributes.ReadOnly]
    private float overworldSpeed;

    [Header("Components")]
    [SerializeField] [SCR_utils.customAttributes.ReadOnly]
    private Rigidbody2D rb;

    [SerializeField] [SCR_utils.customAttributes.ReadOnly]
    private Animator animator;

    [SerializeField] [SCR_utils.customAttributes.ReadOnly]
    private SpriteRenderer sr;

    [SerializeField] [SCR_utils.customAttributes.ReadOnly]
    private SCR_entity_attributes att;

    [Header("Inventory Vars")]
    [SerializeField]
    private int inventorySizeX;
    [SerializeField]
    private int inventorySizeY;

    [SerializeField]
    private Sprite inventorySlot;

    [SerializeField]
    private SCO_item[] startingItems;

    private static SCR_player_inventory inventory;

    //[Header("Combat")]
    //[SerializeField]
    //private SCO_ABS_item_weapon mainHand;
    //[SerializeField]
    //private SCO_ABS_item_weapon offHand;

    [Header("Animation")]
    [Tooltip("")][SerializeField]
    private string animationPrefix;

    [Header("Will not change once built")]
    [SerializeField]
    private float modifOverworldSpeed;

    private void Awake() {
        //Get Components
        rb = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        att = GetComponent<SCR_entity_attributes>();

        //Create Inventory
        //GameObject inv = inventory.setUp(inventorySlot, inventorySizeX, inventorySizeY).transform.parent = gameObject.transform;
        //inv.transform.parent = gameObject.transform;
        //inv.transform.localPosition = Vector3.zero;
        //inventory = inv.AddComponent<SCR_player_inventory>();

        //Define Animation Prefix
        animationPrefix = "ANI_" + animationPrefix + "_";

        //Adjust Speed
        changeOverworldSpeed();



    }
    private void Update() {
        playerMovementMain();
    }
    //All movement related stuff here
    private void playerMovementMain() {
        Vector2 input = returnMovementInput(); //Get Input
        movePlayer(input); //Move Player
        SCR_utils.functions.animate(animator, animationPrefix, rb.velocity != Vector2.zero); //Animate Player
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10); //Move Camera to follow player
    }
    #region movement
    private Vector2 returnMovementInput() {
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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
            rb.velocity = input * overworldSpeed;
        }
        else {
            rb.velocity = (input * overworldSpeed) * 0.71f;
        }
    }
    public void changeOverworldSpeed(int modifBy = 0) {
        overworldSpeed = (att.speed.current * modifOverworldSpeed) - modifBy;
    }
    #endregion
    #region inventory
    public class SCR_player_inventory : MonoBehaviour {
        List<List<Vector2>> inventoryGrid = new List<List<Vector2>>();

        public GameObject setUp(Sprite display,int sizeX, int sizeY) {
            GameObject parent = new GameObject("Slots Parent");
            for (int x = 0; x < sizeX; x++) {
                inventoryGrid.Add(new List<Vector2>());
                for (int y = 0; y < sizeY; y++) {
                    inventoryGrid[x].Add(new Vector2(x,y));


                    GameObject displaySlot = new GameObject(x.ToString() + " " + y.ToString(), typeof(SpriteRenderer));
                    displaySlot.transform.position = inventoryGrid[x][y];
                    displaySlot.transform.parent = parent.transform;
                    displaySlot.GetComponent<SpriteRenderer>().sprite = display;
                }
            }
            return parent;
        }
    }
    #endregion
}