using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_player_main : MonoBehaviour {
    [SerializeField]
    private float defaultSpeed;
    
    private Rigidbody2D rb;

    private SCR_player_attributes attributes;
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        
        new GameObject("Inventory Obj", typeof(SCR_player_attributes)).transform.parent = gameObject.transform;
        //attributes.setSizeOfInventory(10);
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
}
public class SCR_player_attributes : MonoBehaviour{
    public SCO_item[] inventory = new SCO_item[10];
}