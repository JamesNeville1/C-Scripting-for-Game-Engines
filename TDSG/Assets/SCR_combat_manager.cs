using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_combat_manager : MonoBehaviour {
    
    [SerializeField]
    private GameObject tile;

    [SerializeField]
    private Vector2 gridPos; //TEMP

    static public Vector2? pressed;

    [SerializeField]
    private SCR_unit currentUnit;

    private void Start() {
        gridSetup();
        currentUnit = GameObject.Find("PRE_unit").GetComponent<SCR_unit>();
        comatMain();
    }

    private void Update() {
    }

    private List<List<GameObject>> gridSetup(int boundsX=16, int boundsY=9) {
        GameObject parent = new GameObject("Grid Parent");
        List<List<GameObject>> grid = new List<List<GameObject>>();
        for (int x = 0; x < boundsX; x++) {
            grid.Add(new List<GameObject>());
            for (int y = 0; y < boundsY; y++) {
                GameObject currentTile = Instantiate(tile, new Vector2(x, y), Quaternion.identity);
                grid[grid.Count - 1].Add(currentTile);
                currentTile.transform.parent = parent.transform;
                currentTile.name = "Tile: " + transform.position.ToString();
            }
        }
        parent.transform.position = gridPos;
        return grid;
    }
    private void comatMain() {
        if(pressed != null) {
            List<Vector2> temp = new List<Vector2> {
                (Vector2)pressed,
                (Vector2)pressed + Vector2.left,
                (Vector2)pressed + Vector2.left + Vector2.left
            };
            StartCoroutine(currentUnit.move(temp));
        }
    }
}
