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
    private Camera inventoryCamera;

    [SerializeField]
    public Dictionary<Vector2, bool> avaliable = new Dictionary<Vector2, bool>();

    private void Awake() {
        instance = this;
    }
    private void Start() {
        for (int y = 1; y <= startingY; y++) {
            for (int x = 1; x <= startingX; x++) {
                Vector2 pos = new Vector2(x, y);

                GameObject cell = new GameObject("Inventory Grid Cell: " + x.ToString() + ", " + y.ToString(), typeof(SpriteRenderer));
                cell.transform.position = pos;
                cell.transform.parent = transform;
                cell.GetComponent<SpriteRenderer>().sprite = cellSprite;

                avaliable.Add(pos, false);
            }
        }
        inventoryCamera.transform.position = new Vector3(3.5f, 3.5f, -10); //TEMP
    }

    public Vector2 closest(Vector2 toCheck) {
        Vector2 toPass = avaliable.ElementAt(0).Key;
        float lastDist = Mathf.Infinity;
        foreach(Vector2 pos in avaliable.Keys) {
            float dist = Vector2.Distance(toCheck, pos);
            if (dist < lastDist) {
                toPass = pos;
                lastDist = dist;
            }
        }
        return toPass;
    }
}
