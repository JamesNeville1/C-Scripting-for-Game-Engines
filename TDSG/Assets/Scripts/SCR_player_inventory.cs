using IzzetUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_player_inventory : MonoBehaviour {
    [Header("Main")]
    [SerializeField] [Tooltip("What sprite should the grid cells use")] private Sprite gridCellSprite;

    [SerializeField] [Tooltip("What sprite should the item use")] private Sprite itemBlockSprite;

    [SerializeField] private Transform cellParent;

    [SerializeField] private Transform tempSlotKey;

    [Header("Other")]
    [SerializeField] private GameObject destroyVFX;

    private KeyValuePair<Transform, SCR_inventory_piece> tempSlot; //Holds the location and the piece (if any) of a temp slot
    
    private static SCR_player_inventory inventoryInstance; //To be returned to others


    private enum cellState {
        EMPTY,
        OCCUPIED
    }
    private Dictionary<Vector2Int, cellState> gridData = new Dictionary<Vector2Int, cellState>(); //Holds grid and where it is occupied or not
    private Dictionary<SCR_inventory_piece, List<Vector2Int>> pieceData = new Dictionary<SCR_inventory_piece, List<Vector2Int>>(); //Hold data of pieces

    public static SCR_player_inventory returnInstance() {
        return inventoryInstance;
    }

    private void Awake() {
        inventoryInstance = this;

        tempSlot = new KeyValuePair<Transform, SCR_inventory_piece>(tempSlotKey, null);
        tempSlotKey = null; //Removing since I don't want to use it after it is used here
    }
    public void setup(int sizeX, int sizeY) { //Base setup of grid
        SpriteRenderer sr;
        for (int y = 0; y <= sizeY - 1; y++) {
            for (int x = 0; x <= sizeX - 1; x++) {
                Vector2Int pos = new Vector2Int(x, y);

                GameObject cell = new GameObject("Inventory Grid Cell: " + x.ToString() + ", " + y.ToString(), typeof(SpriteRenderer));
                cell.transform.parent = cellParent.transform;
                cell.transform.localPosition = (Vector3Int)pos;

                sr = cell.GetComponent<SpriteRenderer>();
                sr.sprite = gridCellSprite;
                sr.sortingLayerName = "Inventory";
                inventoryInstance.gridData.Add(pos, cellState.EMPTY);
            }
        }

        sr = tempSlot.Key.AddComponent<SpriteRenderer>();
        sr.sprite = gridCellSprite;
        sr.sortingLayerName = "Inventory";
    }
    
    public void removePiece(SCR_inventory_piece toCheck) { //Remove piece from dictionaries
        if (pieceData.ContainsKey(toCheck)) {
            pieceData.Remove(toCheck);
            Vector2Int roundedPos = IzzetMain.castVector2(toCheck.transform.localPosition);
            adjustGridState(toCheck.returnChildren(roundedPos), cellState.EMPTY);
        }
        else if (tempSlot.Value == toCheck) {
            tempSlot = new KeyValuePair<Transform, SCR_inventory_piece>(tempSlot.Key, null);
        }
    }

    public bool tryPlaceGrid(SCR_inventory_piece toManipulate) { //Try to place in grid
        //Check if it can even be placed?
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(toManipulate.transform.localPosition.x), Mathf.RoundToInt(toManipulate.transform.localPosition.y));
        if (!gridData.ContainsKey(pos)) {
            Debug.Log("Grid Data Doesnt Contain: " + pos);
            return false;
        }
        else if(!checkPiece(toManipulate,pos)) {
            return false;
        }

        toManipulate.transform.localPosition = (Vector2)pos;

        pieceData.Add(toManipulate, toManipulate.returnChildren(pos));

        return true;
    }

    public bool tryPlaceTempSlot(SCR_inventory_piece toManipulate) { //Try place in temp slot
        if(tempSlot.Value != null) {
            return false;
        }
        toManipulate.transform.localPosition = tempSlot.Key.localPosition;
        tempSlot = new KeyValuePair<Transform, SCR_inventory_piece>(tempSlot.Key, toManipulate);
        return true;
    }

    public bool checkPiece(SCR_inventory_piece piece, Vector2Int pos) { //Loop through children //LOOK AT THIS, THIS IS BAD ;-;
        List<Vector2Int> children = piece.returnChildren(pos);
        foreach(Vector2Int vec in children) {
            if (!gridData.ContainsKey(vec)) {
                Debug.Log("Grid Data Doesnt Contain: " + vec);
                return false;
            }
            else if (gridData[vec] == cellState.OCCUPIED) {
                Debug.Log("Grid Data: " + vec + " is ocupied");
                return false;
            }
        }
        adjustGridState(children, cellState.OCCUPIED);
        return true;
    }

    private void adjustGridState(List<Vector2Int> toAdjust, cellState state) { //Change grid data
        foreach (Vector2Int vec in toAdjust) {
            gridData[vec] = state;
        }
    }

    #region All Public Functions to return variables
    public Sprite returnItemSprite() {
        return itemBlockSprite;
    }

    public Transform returnCellParent() {
        return cellParent;
    }

    public KeyValuePair<Transform, SCR_inventory_piece> returnTempSlot() {
        return tempSlot;
    }

    public GameObject returnDestroyVFX() {
        return destroyVFX;
    }
    #endregion
}
