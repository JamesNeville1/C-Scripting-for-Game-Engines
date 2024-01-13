using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class SCR_inventory_piece : MonoBehaviour {
    bool pressed = false;

    [SerializeField]
    private SCO_item source;

    [SerializeField]
    private Sprite blockSprite;

    private SCR_player_inventory playerInventory;

    private List<SpriteRenderer> data = new List<SpriteRenderer>();

    private Vector2 originPos;

    private void Awake() {
        playerInventory = SCR_player_inventory.returnInstance();
        originPos = transform.position;
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
                adjustSortingOrder(2);
                //print(name + " " + returnPositions()[1]);
            }
            else if (Input.GetMouseButtonUp(0)) {
                pressed = false;
                adjustSortingOrder(1);
                playerInventory.removePiece(this);
                playerInventory.tryPlace(this);
            }
        }
    }

    private void Start() {
        Vector2[] blocks = source.returnSpaces();

        Color blockColour = source.returnColor();

        foreach (Vector2 blockPos in blocks) {
            GameObject newBlock = new GameObject("Block:" + blockPos.x + ", " + blockPos.y, typeof(SpriteRenderer));
            newBlock.transform.parent = transform;
            newBlock.transform.localPosition = blockPos;

            SpriteRenderer sr = newBlock.GetComponent<SpriteRenderer>();
            sr.sprite = blockSprite;
            sr.color = blockColour;
            sr.sortingOrder = 1;

            newBlock.AddComponent<BoxCollider2D>().usedByComposite = true;
        }
    }

    private void adjustSortingOrder(int adjustTo) {
        foreach (SpriteRenderer sr in data) {
            sr.sortingOrder = adjustTo;
        }
    }

    public List<Vector2> returnChildren() {
        List<Vector2> children = new List<Vector2>();
        for (int i = 0; i < transform.childCount; i++) {
            children.Add(gameObject.transform.GetChild(i).transform.position);
        }
        return children.ToList();
    }
}
