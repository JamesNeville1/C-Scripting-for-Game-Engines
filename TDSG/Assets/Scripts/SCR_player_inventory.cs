using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_player_inventory : MonoBehaviour {
    
    private static SCR_player_inventory instance;

    public static SCR_player_inventory returnInstance() {
        return instance;
    }
    
    public List<SCO_item> inventory = new List<SCO_item>();

    [SerializeField]
    private int startingX;

    [SerializeField]
    private int startingY;

    [SerializeField]
    private Sprite cellSprite;

    private void Awake() {
        instance = this;
    }
    private void Start() {
        for (int y = startingX; y <= startingY; y++) { 
            for (int x = startingY; x <= startingX; x++) {
                GameObject cell = new GameObject("Inventory Grid Cell: " + x.ToString() + ", " + y.ToString(), typeof(SpriteRenderer));
                cell.transform.position = new Vector2(x, y);
                cell.transform.parent = transform;
                cell.GetComponent<SpriteRenderer>().sprite = cellSprite;
            }
        }
    }
}
