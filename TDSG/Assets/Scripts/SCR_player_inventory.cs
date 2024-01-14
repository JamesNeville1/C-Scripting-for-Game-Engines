using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_player_inventory : MonoBehaviour {

    private static SCR_player_inventory instance;

    public static SCR_player_inventory returnInstance() {
        return instance;
    }

    public List<SCO_item> inventory = new List<SCO_item>();

    [SerializeField]
    private int startingX;

    [SerializeField]
    private int startingY;

    [SerializeField]
    private Sprite cellSprite;

    enum cellState {
        EMPTY,
        OCCUPIED
    }

    private Dictionary<Vector2, cellState> gridData = new Dictionary<Vector2, cellState>();
    private Dictionary<SCR_inventory_piece, List<Vector2>> pieceData = new Dictionary<SCR_inventory_piece, List<Vector2>>();

    private void Awake() {
        instance = this;
    }
    private void Start() {
        for (int y = 0; y <= startingY - 1; y++) {
            for (int x = 0; x <= startingX - 1; x++) {
                Vector2 pos = new Vector2(x, y);

                GameObject cell = new GameObject("Inventory Grid Cell: " + x.ToString() + ", " + y.ToString(), typeof(SpriteRenderer));
                cell.transform.position = pos;
                cell.transform.parent = transform;
                cell.GetComponent<SpriteRenderer>().sprite = cellSprite;

                gridData.Add(pos, cellState.EMPTY);
            }
        }
        Camera.main.transform.position = new Vector3(3.5f, 3.5f, -10); //TEMP
    }
    
    public void removePiece(SCR_inventory_piece toCheck) {
        if (pieceData.ContainsKey(toCheck)) {
            pieceData.Remove(toCheck);
            Vector2 roundedPos = roundVect(toCheck.transform.position);
            adjustGridState(toCheck.returnChildren(roundedPos), cellState.EMPTY);
        }
    }

    public bool tryPlace(SCR_inventory_piece toManipulate) {        
        //Check if it can even be placed?
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(toManipulate.transform.position.x), Mathf.RoundToInt(toManipulate.transform.position.y));
        if (!gridData.ContainsKey(pos)) {
            return false;
        }
        else if(!checkPiece(toManipulate,pos)) {
            return false;
        }

        toManipulate.transform.position = (Vector2)pos;

        pieceData.Add(toManipulate, toManipulate.returnChildren(pos));

        return true;
    }

    private void Update() {
        
    }

    public void packButton() {
        
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
