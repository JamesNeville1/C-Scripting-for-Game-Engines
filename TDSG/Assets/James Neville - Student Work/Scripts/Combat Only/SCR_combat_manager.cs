using IzzetUtils.IzzetAttributes;
using IzzetUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Burst.CompilerServices;

public class SCR_combat_manager : MonoBehaviour {

    private static SCR_combat_manager instance;
    public static SCR_combat_manager returnInstance() {
        return instance;
    }
    private void Awake() {
        instance = this;
    }

    [SerializeField] private Vector2Int size;
    [SerializeField] private RuleTile tile;
    [SerializeField] private GameObject boardParent;

    [SerializeField] [MyReadOnly] private Tilemap tilemap;

    private Dictionary<Vector2Int, bool> boardData = new Dictionary<Vector2Int, bool>(); //Use unit in future

    public void setup() {
        tilemap = GetComponentInChildren<Tilemap>();
        tilemap.ClearAllTiles();

        bool offset = false;
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                Vector3Int currentPos = new Vector3Int(x, y, 0);
                tilemap.SetTile(currentPos, tile);

                if (offset) tilemap.SetColor(currentPos, Color.black);
                offset = !offset;

                bool isBound = currentPos.x == 0 || currentPos.y == size.x - 1 || currentPos.y == 0 || currentPos.x == size.x - 1;
                
                if(!isBound) {
                    boardData.Add((Vector2Int)currentPos, true);    
                }
            }
        }

        //boardParent.SetActive(false);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(IzzetMain.getMousePos(Camera.main), Vector2.down);

            if (hit.collider != null) {
                if (boardData.ContainsKey(IzzetMain.castToVector2Int(hit.point))) {
                    Debug.Log("You can move here");
                    return;
                }
            }

            Debug.Log("You can't move here");
        }
    }
}
