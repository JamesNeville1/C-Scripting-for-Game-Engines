using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IzzetUtils;
using IzzetUtils.IzzetAttributes;

public class SCR_inventory_piece : MonoBehaviour {
    [Header("Input Related")]
    [SerializeField] [Tooltip("Is the piece currently moving?")] [MyReadOnly] private bool active = true;
    [SerializeField] [Tooltip("Is the mouse currently over the piece?")] [MyReadOnly] private bool mouseOver = true;

    [Header("Mechanical Implementations")]
    [SerializeField] [Tooltip("Slots the piece takes up")] [MyReadOnly] private Vector2Int[] slots;
    [SerializeField] [Tooltip("The Item this piece represents")] [MyReadOnly] private SCO_item pieceItem;

    #region Won't be Seen in Inspector
    private SCR_player_inventory playerInventory; //Reference to inventory
    private SCR_player_crafting playerCrafting;
    private SCR_master master;
    private List<SpriteRenderer> srs = new List<SpriteRenderer>(); //All sprite renderers of children
    #endregion

    #region Create Instance
    //Create brand new instance from anywhere with no reference required
    public static GameObject createInstance(SCO_item item, Vector2 spawnPos) {
        GameObject newPiece = new GameObject(item.name + " Piece", typeof(SCR_inventory_piece));
        newPiece.transform.position = spawnPos;

        SCR_player_inventory instance = SCR_player_inventory.returnInstance();
        newPiece.transform.parent = instance.returnCellParent();

        SCR_inventory_piece newScript = newPiece.GetComponent<SCR_inventory_piece>();
        newScript.setup(item, instance.returnItemSprite(), item.returnSprite());

        return newPiece;
    }
    private void setup(SCO_item source, Sprite blockSprite, Sprite itemSprite) { //Called from create instance. It creates children acording to the source (item)
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
    #endregion

    private void Awake() {
        playerInventory = SCR_player_inventory.returnInstance(); //Get reference to inventory
        playerCrafting = SCR_player_crafting.returnInstance(); //Get reference to crafting
        master = SCR_master.returnInstance();
    }
    private void Update() {
        playerInput();
    }

    private void OnMouseOver() {
        mouseOver = true;

        if(Input.GetMouseButtonDown(1)) {
            useItemLogic();
        }
    }
    private void OnMouseExit() {
        mouseOver = false;
    }

    #region Item Logic
    private void useItemLogic() {
        switch (pieceItem) {
            case SCO_ABS_item_useable_on_entity:
                Debug.Log("Using " + pieceItem.name);

                SCO_ABS_item_useable_on_entity casted = pieceItem as SCO_ABS_item_useable_on_entity;
                casted.useOnEntity(SCR_player_main.returnInstance().returnAttributes());
                if (casted.returnBreakOnUse()) destroyPiece();

                break;

            default:

                Debug.Log("This is just a resource, you may not eat it!");

                break;
        }
    }
    #endregion
    #region Input
    private void playerInput() { //Move piece via mouse input
        if (active) {
            transform.position = IzzetMain.getMousePos(Camera.main);
            if (!Input.GetMouseButton(0)) {
                active = false;
                if (playerInventory.tryPlaceGrid(this)) {
                    //Debug.Log("I've fallen, and I can't get up");
                    adjustSortingOrder(1);
                    playerCrafting.remove(this);
                }
                else if (master.returnPlayerCrafting() && playerCrafting.tryPlace(this)) {
                    playerInventory.removePiece(this);
                    adjustSortingOrder(1);
                }
                else {
                    drop();
                }
            }
        }
        else {
            if(Input.GetMouseButtonDown(0) && mouseOver) {
                active=true;
                pickUp();
            }
        }
    }
    public void drop() {
        playerInventory.removePiece(this);
        playerCrafting.remove(this);
        transform.parent = null;
        adjustSize(.55f);
        adjustSortingOrder(0, "Default"); //In future 0 should be replaced with an oppropriate order, relative to the world.
    }
    private void pickUp() {
        adjustSize(1);
        adjustSortingOrder(2);
        transform.parent = playerInventory.returnCellParent();
        playerInventory.removePiece(this);
    }
    #endregion
    #region Grid Authentication
    public Vector2Int[] returnChildren(Vector2Int parentPos) { //Return all positions the piece takes up
        List<Vector2Int> vecs = new List<Vector2Int>();
        foreach (Vector2Int item in slots) {
            vecs.Add(item + parentPos);
        }
        return vecs.ToArray();
    }
    private void destroyPiece() {
        playerInventory.removePiece(this);
        Destroy(gameObject);
    }
    #endregion
    #region Adjust Sprite Renderers
    private void adjustSortingOrder(int i, string sortingLayer = "Inventory Piece") {
        foreach (SpriteRenderer sr in srs) {
            sr.sortingOrder = i;
            sr.sortingLayerName = sortingLayer;
        }
    }
    
    private void adjustSize(float i) {
        transform.localScale = new Vector2(i,i);
    }
    #endregion
    public SCO_item returnItem() {
        return pieceItem;
    }
}
