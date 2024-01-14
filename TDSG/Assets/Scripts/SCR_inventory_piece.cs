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

    private List<SpriteRenderer> srs = new List<SpriteRenderer>();

    private void Awake() {
        playerInventory = SCR_player_inventory.returnInstance();
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            adjustSortingOrder(2);
            pressed = true;
        }
    }
    private void Update() {
        if (pressed) {
            if (Input.GetMouseButton(0)) {
                transform.position = SCR_utils.functions.getMousePos(Camera.main);
                adjustSortingOrder(2);
                //print(name + " " + returnPositions()[1]);
                playerInventory.removePiece(this);
            }
            else if (Input.GetMouseButtonUp(0)) {
                pressed = false;
                if(playerInventory.tryPlace(this)) adjustSortingOrder(1);
                else adjustSortingOrder(2);
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

            srs.Add(newBlock.GetComponent<SpriteRenderer>());
            srs[srs.Count-1].sprite = blockSprite;
            srs[srs.Count-1].color = blockColour;
            srs[srs.Count-1].sortingOrder = 2;

            newBlock.AddComponent<BoxCollider2D>().usedByComposite = true;
        }
    }

    private void adjustSortingOrder(int adjustTo) {
        foreach (SpriteRenderer sr in srs) {
            sr.sortingOrder = adjustTo;
        }
    }

    public List<Vector2> returnChildren(Vector2 parentPos) {
        List<Vector2> children = new List<Vector2>();
        for (int i = 0; i < transform.childCount; i++) {
            children.Add(gameObject.transform.GetChild(i).transform.localPosition + (Vector3)parentPos);
            print(children[i]);
        }
        return children.ToList();
    }
}
