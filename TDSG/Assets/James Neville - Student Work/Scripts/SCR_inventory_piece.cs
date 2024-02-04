using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IzzetUtils;
using IzzetUtils.IzzetAttributes;

public class SCR_inventory_piece : MonoBehaviour {
    [Header("Input Related")]
    [SerializeField] [Tooltip("Is the piece currently moving?")] [MyReadOnly] private bool active = false;
    [SerializeField] [Tooltip("Is the mouse currently over the piece?")] [MyReadOnly] private bool mouseOver = false;

    [Header("Mechanical Implementations")]
    [SerializeField] [Tooltip("Slots the piece takes up")] [MyReadOnly] private Vector2Int[] slots;
    [SerializeField] [Tooltip("The Item this piece represents")] [MyReadOnly] private SCO_item pieceItem;

    #region Won't be Seen in Inspector
    private SCR_master_inventory_main playerInventory; //Reference to inventory
    private SCR_master_crafting playerCrafting;
    private SCR_master_audio audioManager;
    private SCR_master_main master;
    private List<SpriteRenderer> srs = new List<SpriteRenderer>(); //All sprite renderers of children
    #endregion

    #region Create Instance
    //Create brand new instance from anywhere with no reference required
    public static SCR_inventory_piece createInstance(SCO_item item, Vector2 spawnPos, Transform parent, bool startActive = true) {
        GameObject newPiece = new GameObject(item.name + " Piece", typeof(SCR_inventory_piece));
        newPiece.transform.position = spawnPos;

        newPiece.transform.parent = parent;

        SCR_inventory_piece newScript = newPiece.GetComponent<SCR_inventory_piece>();
        newScript.setup(item, SCR_master_inventory_main.returnInstance().returnItemSprite(), startActive);

        return newScript;
    }
    private void setup(SCO_item source, Sprite blockSprite, bool startActive) { //Called from create instance. It creates children acording to the source (item)
        active = startActive; mouseOver = startActive;
        
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

    #region Unity
    private void Awake() {
        playerInventory = SCR_master_inventory_main.returnInstance(); //Get reference to inventory
        playerCrafting = SCR_master_crafting.returnInstance(); //Get reference to crafting
        audioManager = SCR_master_audio.returnInstance();
        master = SCR_master_main.returnInstance(); //Get reference to master
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
    #endregion
    #region Item Logic
    private void useItemLogic() {
        switch (pieceItem) {
            case SCO_ABS_item_useable_on_entity:
                Debug.Log("Using " + pieceItem.name);

                SCO_ABS_item_useable_on_entity casted = pieceItem as SCO_ABS_item_useable_on_entity;
                casted.useOnEntity(SCR_player_main.returnInstance().returnAttributes());

                audioManager.playRandomEffect(casted.returnOnUse());

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
    #region Returns
    public SCO_item returnItem() {
        return pieceItem;
    }
    #endregion
}
