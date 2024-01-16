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
    private GameObject piecePerfab;

    [SerializeField]
    private SCO_item[] tempItems;

    enum cellState {
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
    }
    public void setup(List<SCO_item> items, int sizeX, int sizeY) {
        GameObject cellParent = new GameObject("Inventory Cells");
        cellParent.transform.parent = Camera.main.transform;
        cellParent.transform.localPosition = new Vector3(-7, -3,10);
        for (int y = 0; y <= sizeY - 1; y++) {
            for (int x = 0; x <= sizeX - 1; x++) {
                Vector2 pos = new Vector2(x, y);

                GameObject cell = new GameObject("Inventory Grid Cell: " + x.ToString() + ", " + y.ToString(), typeof(SpriteRenderer));
                cell.transform.parent = cellParent.transform;
                cell.transform.localPosition = pos;

                SpriteRenderer sr = cell.GetComponent<SpriteRenderer>();
                sr.sprite = instance.cellSprite;
                sr.sortingLayerName = "Inventory";
                instance.gridData.Add(pos, cellState.EMPTY);
            }
        }
        
        foreach (SCO_item item in items) {
            GameObject piece = Instantiate(piecePerfab, cellParent.transform);
            piece.transform.localPosition = new Vector3(0, 0, 10);
            piece.GetComponent<SCR_inventory_piece>().setup(item);
        }

        foreach(Vector2 vec in gridData.Keys) {
            Debug.Log(vec + " " + gridData[vec].ToString());
        }
    }
    
    public void removePiece(SCR_inventory_piece toCheck) {
        if (pieceData.ContainsKey(toCheck)) {
            pieceData.Remove(toCheck);
            Vector2 roundedPos = roundVect(toCheck.transform.localPosition);
            adjustGridState(toCheck.returnChildren(roundedPos), cellState.EMPTY);
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
}
