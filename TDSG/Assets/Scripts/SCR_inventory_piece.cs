using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class SCR_inventory_piece : MonoBehaviour {
    bool pressed = false;

    private SCR_player_inventory playerInventory;

    private List<SpriteRenderer> srs = new List<SpriteRenderer>();

    public static GameObject createInstance(SCO_item item, Vector2 spawnPos) {
        GameObject newPiece = new GameObject(item.name + " Piece", typeof(SCR_inventory_piece));
        newPiece.transform.position = spawnPos;

        SCR_player_inventory instance = SCR_player_inventory.returnInstance();
        newPiece.transform.parent = instance.returnCellParent();

        SCR_inventory_piece newScript = newPiece.GetComponent<SCR_inventory_piece>();
        newScript.setup(item, instance.returnItemSprite());

        return newPiece;
    }

    private void Awake() {
        playerInventory = SCR_player_inventory.returnInstance();
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) { 
            pressed = true;
            playerInventory.removePiece(this);
        }
        //Debug.Log("This is a " + itemName);
    }
    private void Update() {
        if (pressed) {
            if (Input.GetMouseButton(0)) {
                transform.position = SCR_utils.functions.getMousePos(Camera.main);
            }
            else if (Input.GetMouseButtonUp(0)) {
                pressed = false;
                if (!playerInventory.tryPlace(this)) {
                    if (!playerInventory.tryPlaceTempSlot(this)) {
                        Debug.Log(gameObject.name + " has been destroyed");
                        Destroy(this.gameObject);
                    }
                }
            }
        }
    }

    public void setup(SCO_item source, Sprite blockSprite) {
        Vector2[] blocks = source.returnSpaces();

        Color blockColour = source.returnColor();

        foreach (Vector2 blockPos in blocks) {
            GameObject newBlock = new GameObject("Block:" + blockPos.x + ", " + blockPos.y, typeof(SpriteRenderer));
            newBlock.transform.parent = transform;
            newBlock.transform.localPosition = blockPos;

            srs.Add(newBlock.GetComponent<SpriteRenderer>());
            int arrayPos = srs.Count - 1;

            srs[arrayPos].sprite = blockSprite;
            srs[arrayPos].color = blockColour;
            srs[arrayPos].sortingOrder = 2;
            srs[arrayPos].sortingLayerName = "Inventory Piece";

            newBlock.AddComponent<BoxCollider2D>().usedByComposite = true;
        }

        
        gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        CompositeCollider2D compCol = gameObject.AddComponent<CompositeCollider2D>();
        compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;
        compCol.isTrigger = true;
    }

    public List<Vector2> returnChildren(Vector2 parentPos) {
        List<Vector2> children = new List<Vector2>();
        for (int i = 0; i < transform.childCount; i++) {
            children.Add(gameObject.transform.GetChild(i).transform.localPosition + (Vector3)parentPos);
        }
        return children.ToList();
    }
}
