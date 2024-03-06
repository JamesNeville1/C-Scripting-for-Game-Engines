using IzzetUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using IzzetUtils.IzzetAttributes;

public class SCR_master_inventory_main : MonoBehaviour {
    [Header("Require Dev Input")]
    [SerializeField] [Tooltip("What sprite should the grid cells use")] private Sprite gridCellSprite;
    [SerializeField] [Tooltip("What sprite should the item use")] private Sprite itemBlockSprite;
    [SerializeField] private RectTransform cellParent;

    //
    public enum cellState {
        EMPTY,
        OCCUPIED
    }
    
    private Dictionary<Vector2Int, cellState> gridData = new Dictionary<Vector2Int, cellState>(); //Holds grid and if it is occupied or not
    private Dictionary<SCR_inventory_piece, Vector2Int[]> pieceData = new Dictionary<SCR_inventory_piece, Vector2Int[]>(); //Hold data of pieces

    #region Set Instance
    private static SCR_master_inventory_main inventoryInstance; //To be returned to others
    public static SCR_master_inventory_main returnInstance() {
        return inventoryInstance;
    }

    private void Awake() {
        inventoryInstance = this;

    }
    #endregion

    #region Setup
    public void setup(int sizeX, int sizeY) { //Base setup of grid
        for (int y = 0; y <= sizeY - 1; y++) {
            for (int x = 0; x <= sizeX - 1; x++) {
                Vector2Int pos = new Vector2Int(x, y);

                createSlotDisplay("Inventory Grid Cell: ", cellParent, new Vector3(pos.x, pos.y, 1)); //Made Z = 1, this was because the pieces and cells were intersecting, causing errors
                inventoryInstance.gridData.Add(pos, cellState.EMPTY);
            }
        }
    }
    #endregion
    #region Piece Placement
    public void removePiece(SCR_inventory_piece toCheck) { //Remove piece from dictionaries
        if (pieceData.ContainsKey(toCheck)) {
            
            //Remove from pieceData
            pieceData.Remove(toCheck);

            //Remove from gridData
            Vector2Int roundedPos = IzzetMain.castToVector2Int(toCheck.transform.localPosition);
            adjustGridState(toCheck.returnChildren(roundedPos), cellState.EMPTY);
        }
    }

    public bool tryPlaceGrid(SCR_inventory_piece toManipulate) { //Try to place in grid, return false if fail
        toManipulate.transform.parent = cellParent;

        //Check if it can even be placed?
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(toManipulate.transform.localPosition.x), Mathf.RoundToInt(toManipulate.transform.localPosition.y));
        if(!checkPiece(toManipulate,pos)) {
            return false;
        }

        //If True
        toManipulate.transform.localPosition = (Vector2)pos;

        pieceData.Add(toManipulate, toManipulate.returnChildren(pos));

        return true;
    }
    #endregion
    #region Grid Authentication
    private bool checkPiece(SCR_inventory_piece piece, Vector2Int pos) { 
        
        //Loop through children, check all slots are free and valid
        Vector2Int[] children = piece.returnChildren(pos);
        foreach(Vector2Int vec in children) {
            bool invalid = !gridData.ContainsKey(vec) || gridData[vec] == cellState.OCCUPIED;
            if (invalid) return false;
        }

        //If passes the validation, set ass occupied
        adjustGridState(children, cellState.OCCUPIED);
        return true;
    }

    private void adjustGridState(Vector2Int[] toAdjust, cellState state) { //Change grid data
        foreach (Vector2Int vec in toAdjust) {
            gridData[vec] = state;
        }
    }
    #endregion
    #region Returns & Publics
    public Sprite returnItemSprite() {
        return itemBlockSprite;
    }

    public Transform returnCellParent() {
        return cellParent;
    }
    public GameObject createSlotDisplay(string prefix, Transform parent, Vector3 localPos) { //Used by Inventory and Crafting
        SpriteRenderer sr;
        GameObject obj = new GameObject($"{prefix} {localPos.x} , {localPos.y}", typeof(SpriteRenderer));
        obj.transform.parent = parent.transform;
        obj.transform.localPosition = localPos;

        sr = obj.GetComponent<SpriteRenderer>();
        sr.sprite = gridCellSprite;
        sr.sortingLayerName = "Inventory";

        obj.AddComponent<BoxCollider2D>().isTrigger = true;

        return obj;
    }
    public void destroyAll() {
        foreach (var piece in pieceData.Keys) {
            Destroy(piece.gameObject);
        }
    }
    public bool contains(SCR_inventory_piece toCheck) {
        return pieceData.ContainsKey(toCheck);
    }
    public float returnZOfParent() {
        return cellParent.transform.position.z;
    }
    #endregion
}
