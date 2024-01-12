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
    private HashSet<Vector2> avaliable = new HashSet<Vector2>();

    [SerializeField]
    private HashSet<SCR_inventory_piece> used = new HashSet<SCR_inventory_piece>();

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

                avaliable.Add(pos);
            }
        }
        Camera.main.transform.position = new Vector3(3.5f, 3.5f, -10); //TEMP
    }

    public Vector2 closest(Vector2 toCheck, SCR_inventory_piece piece) {
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(toCheck.x), Mathf.RoundToInt(toCheck.y));
        if (avaliable.Contains(pos)) {
            used.Add(piece);
            return pos;
        }
        else {
            return toCheck;
        }
    }

    public void remove(SCR_inventory_piece piece) {
        used.Remove(piece);
    }

    private void Update() {
        print(pack());
    }

    public bool pack() {
        foreach (SCR_inventory_piece piece in used) {
            List<Vector2> vecs = piece.returnPositions();
            foreach (Vector2 vect in vecs) {
                //print(vect);
                if(!avaliable.Contains(vect)) {
                   return false;
                }
            }
        }
        return true;
    }
}
