using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using IzzetUtils;
using IzzetUtils.IzzetAttributes;
using System.Runtime.CompilerServices;

public class SCR_inventory_piece : MonoBehaviour {
    [SerializeField] [Tooltip("IsPressed")] [MyReadOnly] private bool active = true;
    [SerializeField] [Tooltip("IsPressed")] [MyReadOnly] private bool mouseOver = true;
    [SerializeField] [Tooltip("Slots the piece takes up")] [MyReadOnly] private Vector2Int[] slots;
    [SerializeField] [Tooltip("The Item this piece represents")] [MyReadOnly] private SCO_item pieceItem;

    private SCR_player_inventory playerInventory; //Reference to inventory
    private List<SpriteRenderer> srs = new List<SpriteRenderer>(); //All sprite renderers of children

    //Create brand new instance from anywhere with no reference required
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
    private void Update() {
        move();
    }

    private void OnMouseOver() {
        mouseOver = true;

        if(Input.GetMouseButtonDown(1)) {
            switch (pieceItem) {
                case SCO_ABS_item_edible:
                    Debug.Log("Eating");
                    SCO_ABS_item_edible casted = pieceItem as SCO_ABS_item_edible;
                    casted.eat(SCR_player_main.returnInstance().returnAttributes());
                    break;
                case SCO_ABS_item_weapon:
                    print("This is a weapon");
                    break;
                case SCO_item:
                    Debug.Log("This is just a resource, you may not eat it!");
                    break;

            }
        }
    }

    private void OnMouseExit() {
        mouseOver = false;
    }

    private void move() { //Move piece via mouse input
        if (active) {
            transform.position = IzzetMain.getMousePos(Camera.main);
            if (!Input.GetMouseButton(0)) {
                active = false;
                if (!playerInventory.tryPlaceGrid(this)) {
                    //Debug.Log("I've fallen, and I can't get up");
                    drop();
                }
                adjustSortingOrder(1);
            }
        }
        else {
            if(Input.GetMouseButtonDown(0) && mouseOver) {
                active=true;
                pickUp();
            }
        }
    }

    private void drop() {
        playerInventory.removePiece(this);
        transform.parent = null;
        adjustSize(.55f);
    }
    private void pickUp() {
        adjustSize(1);
        adjustSortingOrder(2);
        transform.parent = playerInventory.returnCellParent();
        playerInventory.removePiece(this);
    }

    private void setup(SCO_item source, Sprite blockSprite) { //Called from create instance. It creates children acording to the source (item)
        Vector2Int[] blocks = source.returnSpaces();

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

        slots = source.returnSpaces();

        pieceItem = source;
    }

    public Vector2Int[] returnChildren(Vector2Int parentPos) { //Return all positions the piece takes up
        List<Vector2Int> vecs = new List<Vector2Int>();
        foreach (Vector2Int item in slots) {
            vecs.Add(item + parentPos);
        }
        return vecs.ToArray();
    }

    private void adjustSortingOrder(int i) {
        foreach (SpriteRenderer sr in srs) {
            sr.sortingOrder = i;
        }
    }
    
    private void adjustSize(float i) {
        transform.localScale = new Vector2(i,i);
    }
}
