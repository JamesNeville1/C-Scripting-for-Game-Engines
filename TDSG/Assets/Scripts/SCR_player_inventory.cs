using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField]
    private Dictionary<Vector2, GameObject> avaliable = new Dictionary<Vector2, GameObject>();

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

                avaliable.Add(pos, null);
            }
        }
        Camera.main.transform.position = new Vector3(3.5f, 3.5f, -10); //TEMP
    }

    public bool tryPlace(SCR_inventory_piece toManipulate) {
        //Check if it can even be placed?
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(toManipulate.transform.position.x), Mathf.RoundToInt(toManipulate.transform.position.y));
        if (!avaliable.ContainsKey(pos)) {
            return false;
        }

        toManipulate.transform.position = (Vector2)pos;

        return true;
    }

    private void Update() {

    }

    public void packButton() {
        SCR_inventory_piece[] pieces = FindObjectsOfType<SCR_inventory_piece>();

        List<SCR_inventory_piece> validPieces = new List<SCR_inventory_piece>();

        foreach (SCR_inventory_piece piece in pieces) {
            if (piece.isPlaced) {
                validPieces.Add(piece);
            }
        }


    }
}
