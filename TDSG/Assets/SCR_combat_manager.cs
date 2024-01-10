using System;
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
    private KeyValuePair<SCR_unit, unitType> currentUnit;

    [SerializeField]
    private GameObject unitPrefab;

    public enum unitType {
        PLAYER,
        Enemy
    }

    private List<KeyValuePair<SCR_unit, unitType>> units = new List<KeyValuePair<SCR_unit, unitType>>();
    private List<List<GameObject>> grid = new List<List<GameObject>>();

    private void Start() {
        setup();
        currentUnit = units[0];
        print(currentUnit);
    }

    private void Update() {
        comatMain();
    }

    public void setup() {
        grid = gridSetup();
        units = unitSetup();
    }
    private void comatMain() {
        int i = 0;
        if (units[i].Value == unitType.PLAYER) {
            if(pressed != null) {
                units[i].Key.move(null);
                i++;
            }
        }
    }
    private List<List<GameObject>> gridSetup(int boundsX = 16, int boundsY = 9) {
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
    private List<KeyValuePair<SCR_unit, unitType>> unitSetup() {
        GameObject parent = new GameObject("Units");
        List<KeyValuePair<SCR_unit, unitType>> toPass = new List<KeyValuePair<SCR_unit, unitType>>();  //TEMP
        
        toPass.Add(new KeyValuePair<SCR_unit, unitType>
            (Instantiate(unitPrefab, grid[4][5].transform.position, Quaternion.identity, parent.transform).GetComponent<SCR_unit>(), unitType.PLAYER));
        toPass.Add(new KeyValuePair<SCR_unit, unitType>
            (Instantiate(unitPrefab, grid[8][5].transform.position, Quaternion.identity, parent.transform).GetComponent<SCR_unit>(), unitType.Enemy));
        return toPass;
    }
}
