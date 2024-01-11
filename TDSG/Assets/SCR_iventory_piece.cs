using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_iventory_piece : MonoBehaviour {
    bool pressed = false;

    [SerializeField]
    private SCO_item source;

    [SerializeField]
    private Sprite blockSprite;

    SCR_player_inventory playerInventory;

    private void Awake() {
        playerInventory = SCR_player_inventory.returnInstance();
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            pressed = true;
        }
    }
    private void Update() {
        if (Input.GetMouseButton(0) && pressed) {
            transform.position = SCR_utils.functions.getMousePos(Camera.main);
        }
        else if(Input.GetMouseButtonUp(0)) {
            pressed = false;
            print(Camera.main);
            transform.position = playerInventory.closest(SCR_utils.functions.getMousePos(Camera.main));
        }
    }

    private void Start() {
        Vector2[] blocks = source.returnSpaces();

        foreach (Vector2 block in blocks) {
            GameObject newBlock = new GameObject("Block:" + block.x + ", " + block.y, typeof(SpriteRenderer));
            newBlock.transform.parent = transform;
            newBlock.transform.localPosition = block;

            newBlock.GetComponent<SpriteRenderer>().sprite = blockSprite;

            newBlock.AddComponent<BoxCollider2D>().usedByComposite = true;
        }
    }
}
