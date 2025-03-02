using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IzzetUtils;
using IzzetUtils.IzzetAttributes;
using UnityEngine.EventSystems;

public class SCR_inventory_piece : MonoBehaviour {
    [Header("Input Related")]
    [SerializeField] [Tooltip("Is the piece currently moving?")] [MyReadOnly] private bool active = false;
    [SerializeField] [Tooltip("Is the mouse currently over the piece?")] [MyReadOnly] private bool mouseOver = false;

    [Header("Mechanical Implementations")]
    [SerializeField] [Tooltip("Slots the piece takes up")] [MyReadOnly] private Vector2Int[] slots;
    [SerializeField] [Tooltip("The Item this piece represents")] [MyReadOnly] private SCO_item pieceItem;

    [Header("Other")]
    [SerializeField] [MyReadOnly] private string myName;

    //References
    private SCR_master_inventory_main playerInventory;
    private SCR_master_crafting playerCrafting;
    private SCR_master_audio audioManager;
    private SCR_master_main master;

    private List<SpriteRenderer> srs = new List<SpriteRenderer>(); //All sprite renderers of children

    #region Create Instance
    //Create brand new instance from anywhere with no reference required
    public static SCR_inventory_piece CreateInstance(SCO_item item, Vector2 spawnPos, Transform parent, bool startActive = true) {
        GameObject newPiece = new GameObject(item.name + " Piece", typeof(SCR_inventory_piece));
        newPiece.transform.position = spawnPos;

        newPiece.transform.parent = parent;

        SCR_inventory_piece newScript = newPiece.GetComponent<SCR_inventory_piece>();
        newScript.Setup(item, startActive);

        return newScript;
    }
    private void Setup(SCO_item source, bool startActive) { //Called from create instance. It creates children acording to the source (item)
        active = startActive; mouseOver = startActive;
        
        Vector2Int[] blocks = source.ReturnSpaces();

        Sprite itemSprite = source.ReturnItemSprite();

        foreach (Vector2 blockPos in blocks) {
            GameObject newBlock = new GameObject("Block:" + blockPos.x + ", " + blockPos.y, typeof(SpriteRenderer));
            newBlock.transform.parent = transform;
            newBlock.transform.localPosition = blockPos;

            srs.Add(newBlock.GetComponent<SpriteRenderer>());
            int arrayPos = srs.Count - 1;

            myName = source.ReturnName();

            srs[arrayPos].sprite = itemSprite;
            srs[arrayPos].sortingOrder = 2;
            srs[arrayPos].sortingLayerName = "Inventory Piece";

            //newBlock.AddComponent<BoxCollider2D>().usedByComposite = true;
            newBlock.AddComponent<BoxCollider2D>().compositeOperation = Collider2D.CompositeOperation.Merge;
        }

        
        gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        CompositeCollider2D compCol = gameObject.AddComponent<CompositeCollider2D>();
        compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;
        compCol.isTrigger = true;

        slots = source.ReturnSpaces();

        pieceItem = source;
    }
    #endregion

    #region Unity
    private void Awake() {
        playerInventory = SCR_master_inventory_main.instance; //Get reference to inventory
        playerCrafting = SCR_master_crafting.instance; //Get reference to crafting
        audioManager = SCR_master_audio.instance; //Get reference to audio
        master = SCR_master_main.instance; //Get reference to master
    }
    private void Update() {
        PlayerInput();
    }

    private void OnMouseEnter() {
        mouseOver = true;
    }
    private void OnMouseExit() {
        mouseOver = false;
        master.SetInfoText("", Color.clear);
    }
    private void OnMouseOver() {
        if(!active) {
            master.SetInfoText(myName, Color.yellow);
        }

        bool canUse = SCR_master_inventory_main.instance.Contains(this) || active;
        if (Input.GetMouseButtonDown(1) && canUse) {
            UseItemLogic(); //Use item if useable
        }
    }
    #endregion
    #region Item Logic
    private void UseItemLogic() {
        switch (pieceItem) {
            case SCO_ABS_item_useable_on_entity: //Use on player if can be used
                Debug.Log("Using " + pieceItem.name);

                SCO_ABS_item_useable_on_entity casted = pieceItem as SCO_ABS_item_useable_on_entity;
                casted.UseOnEntity(SCR_player_main.returnInstance().ReturnAttributes());

                if (casted.ReturnShouldSFX()) {
                    audioManager.PlayRandomEffect(casted.ReturnOnUse());
                }

                if (casted.ReturnBreakOnUse()) DestroyPiece();
                
                break;

            default:

                Debug.Log("This is just a resource, you may not eat it!");

                break;
        }
    }
    #endregion
    #region Input
    private void PlayerInput() { //Move piece via mouse input
        if (active) {
            Vector2 mousePos = IzzetMain.GetMousePos(Camera.main);
            transform.position = new Vector3(mousePos.x, mousePos.y, playerInventory.ReturnZOfParent() - 1); //Set Z to this to make sure mouse over is still triggered
            
            if (Input.GetMouseButtonUp(0)) { //If let go of left mouse button
                active = false;

                if (playerInventory.TryPlaceGrid(this)) { //Try to place in inventory
                    AdjustSortingOrder(1);
                    playerCrafting.Remove(this);
                }
                else if (master.IsPlayerCraftingActive() && playerCrafting.TryPlace(this)) { //Try to place in crafting slots
                    playerInventory.RemovePiece(this);
                    AdjustSortingOrder(1);
                }
                else { //If all fails, drop the item
                    Drop();

                }
            }
        }
        else {
            if(Input.GetMouseButtonDown(0) && mouseOver && !EventSystem.current.IsPointerOverGameObject()) { //If pressed while mouse isn't over UI, pickup
                master.SetInfoText("", Color.clear);
                PickUp();
            }
        }
    }
    public void Drop() {

        //Remove from crafting and inventory incase, and remove parent
        RemoveFromAll();
        transform.parent = null;

        //Purely Design
        AdjustSize(.55f);
        AdjustSortingOrder(0, "Default");
    }
    private void PickUp() {

        //Make active, Remove from crafting and inventory incase
        active = true;
        RemoveFromAll();

        //Purely Design
        AdjustSize(1);
        AdjustSortingOrder(2);
    }
    private void RemoveFromAll() {
        playerInventory.RemovePiece(this);
        playerCrafting.Remove(this);
    }
    #endregion
    #region Authentication
    public Vector2Int[] ReturnChildren(Vector2Int parentPos) { //Return all positions the piece takes up
        List<Vector2Int> vecs = new List<Vector2Int>();
        foreach (Vector2Int item in slots) {
            vecs.Add(item + parentPos);
        }
        return vecs.ToArray();
    }
    private void DestroyPiece() {
        playerInventory.RemovePiece(this);
        Destroy(gameObject);
    }
    #endregion
    #region Adjust Display
    private void AdjustSortingOrder(int i, string sortingLayer = "Inventory Piece") {
        foreach (SpriteRenderer sr in srs) {
            sr.sortingOrder = i;
            sr.sortingLayerName = sortingLayer;
        }
    }
    
    private void AdjustSize(float i) {
        transform.localScale = new Vector2(i,i);
    }
    #endregion
    #region Returns
    public SCO_item ReturnItem() {
        return pieceItem;
    }
    #endregion
}
