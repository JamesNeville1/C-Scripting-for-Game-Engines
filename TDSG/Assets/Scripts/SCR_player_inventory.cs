using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SCR_player_inventory : MonoBehaviour {

    private static SCR_player_inventory instance;

    [SerializeField]
    private int startingX;

    [SerializeField]
    private int startingY;

    [SerializeField]
    private Sprite cellSprite;

    [SerializeField]
    private Sprite itemBlockSprite;

    [SerializeField]
    private Transform cellParent;

    [SerializeField]
    private KeyValuePair<Transform, SCR_inventory_piece> tempSlot;

    [SerializeField]
    private Transform tempSlotKey;

    [SerializeField]
    private GameObject destroyVFX;

    private enum cellState { //Why has to be public?
        EMPTY,
        OCCUPIED
    }

    private Dictionary<Vector2, cellState> gridData = new Dictionary<Vector2, cellState>();
    private Dictionary<SCR_inventory_piece, List<Vector2>> pieceData = new Dictionary<SCR_inventory_piece, List<Vector2>>();

    public static SCR_player_inventory returnInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;

        tempSlot = new KeyValuePair<Transform, SCR_inventory_piece>(tempSlotKey, null);
        tempSlotKey = null; //Removing since I don't want to use it after it is used here
    }
    public void setup(int sizeX, int sizeY) {
        for (int y = 0; y <= sizeY - 1; y++) {
            for (int x = 0; x <= sizeX - 1; x++) {
                Vector2 pos = new Vector2(x, y);

                GameObject cell = new GameObject("Inventory Grid Cell: " + x.ToString() + ", " + y.ToString(), typeof(SpriteRenderer));
                cell.transform.parent = cellParent.transform;
                cell.transform.localPosition = pos;

                SpriteRenderer sr = cell.GetComponent<SpriteRenderer>();
                sr.sprite = cellSprite;
                sr.sortingLayerName = "Inventory";
                instance.gridData.Add(pos, cellState.EMPTY);
            }
        }

        tempSlot.Key.AddComponent<SpriteRenderer>().sprite = cellSprite;
    }
    
    public void removePiece(SCR_inventory_piece toCheck) {
        if (pieceData.ContainsKey(toCheck)) {
            pieceData.Remove(toCheck);
            Vector2 roundedPos = roundVect(toCheck.transform.localPosition);
            adjustGridState(toCheck.returnChildren(roundedPos), cellState.EMPTY);
        }
        else if (tempSlot.Value == toCheck) {
            tempSlot = new KeyValuePair<Transform, SCR_inventory_piece>(tempSlot.Key, null);
        }
    }

    public bool tryPlace(SCR_inventory_piece toManipulate) {        
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

    public bool tryPlaceTempSlot(SCR_inventory_piece toManipulate) {
        if(tempSlot.Value != null) {
            return false;
        }
        toManipulate.transform.localPosition = tempSlot.Key.localPosition;
        tempSlot = new KeyValuePair<Transform, SCR_inventory_piece>(tempSlot.Key, toManipulate);
        return true;
    }

    public bool checkPiece(SCR_inventory_piece piece, Vector2 pos) {
        List<Vector2> children = piece.returnChildren(pos);
        foreach(Vector2 vec in children) {
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

    private void adjustGridState(List<Vector2> toAdjust, cellState state) {
        foreach (Vector2 vec in toAdjust) {
            gridData[vec] = state;
        }
    }

    private Vector2 roundVect(Vector2 vect) {
        vect = new Vector2(Mathf.RoundToInt(vect.x), Mathf.RoundToInt(vect.y));
        return vect;
    }

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
}
