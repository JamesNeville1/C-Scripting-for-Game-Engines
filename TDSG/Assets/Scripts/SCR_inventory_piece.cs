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
    private HashSet<Vector2> BlockPos = new HashSet<Vector2>(); 

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
                adjustSortingOrder(2);
                //print(name + " " + returnPositions()[1]);
            }
            else if (Input.GetMouseButtonUp(0)) {
                pressed = false;
                transform.position = playerInventory.closest(SCR_utils.functions.getMousePos(Camera.main), this);
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
            sr.sortingOrder = 1;
            BlockPos.Add(newBlock.transform.localPosition);

            newBlock.AddComponent<BoxCollider2D>().usedByComposite = true;
        }
    }

    private void adjustSortingOrder(int adjustTo) {
        foreach (SpriteRenderer sr in data) {
            sr.sortingOrder = adjustTo;
        }
    }

    public List<Vector2> returnPositions() {
        List<Vector2> vecs = BlockPos.ToList();
        List<Vector2> toLoop = vecs;

        for (int i = 0; i < toLoop.Count; i++) {
            vecs[i] += (Vector2)transform.position;
        }

        return vecs;
    }
}
