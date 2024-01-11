using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SCR_iventory_piece : MonoBehaviour {
    bool pressed = false;

    [SerializeField]
    private SCO_item source;

    [SerializeField]
    private Sprite blockSprite;

    SCR_player_inventory playerInventory;

    private void Awake() {
        playerInventory = SCR_player_inventory.returnInstance();
        print(playerInventory);
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            pressed = true;
        }
    }
    private void Update() {
        if (pressed) {
            if (Input.GetMouseButton(0)) {
                transform.position = SCR_utils.functions.getMousePos(Camera.main);
            }
            else if (Input.GetMouseButtonUp(0)) {
                pressed = false;
                transform.position = (Vector3Int)playerInventory.closest(SCR_utils.functions.getMousePos(Camera.main));
            }
        }
    }

    private void Start() {
        Vector2[] blocks = source.returnSpaces();

        Color blockColour = source.returnColor();

        foreach (Vector2 block in blocks) {
            GameObject newBlock = new GameObject("Block:" + block.x + ", " + block.y, typeof(SpriteRenderer));
            newBlock.transform.parent = transform;
            newBlock.transform.localPosition = block;

            SpriteRenderer sr = newBlock.GetComponent<SpriteRenderer>();
            sr.sprite = blockSprite;
            sr.color = blockColour;

            newBlock.AddComponent<BoxCollider2D>().usedByComposite = true;
        }
    }
}
