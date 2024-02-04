using IzzetUtils.IzzetAttributes;
using IzzetUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SCR_master_combat : MonoBehaviour {

    #region Set Instance
    private static SCR_master_combat instance;
    public static SCR_master_combat returnInstance() {
        return instance;
    }
    private void Awake() {
        instance = this;
    }
    #endregion

    [Header("Require Dev Input")]
    [SerializeField] private RuleTile tile;
    [SerializeField] private GameObject boardParent;

    [Header("Read Only")]
    [SerializeField] [MyReadOnly] private Tilemap tilemap;
    [SerializeField] [MyReadOnly] private bool playerTurn = false;

    //
    private Dictionary<Vector2Int, SCR_combat_unit> boardData = new Dictionary<Vector2Int, SCR_combat_unit>(); //Use unit in future

    #region Unity
    private void Update() {
        if (playerTurn) {
            takeInput();
        }
    }
    #endregion
    #region Setup
    public void setup(Vector2Int size) {
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
                    boardData.Add((Vector2Int)currentPos, null);    
                }
            }
        }

        //boardParent.SetActive(false);
    }
    #endregion
    #region Take Player Input
    private void takeInput() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(IzzetMain.getMousePos(Camera.main), Vector2.down);

            if (hit.collider != null) {
                if (boardData.ContainsKey(IzzetMain.castToVector2Int(hit.point))) {
                    Debug.Log("You can move here");
                    return;
                }
            }

            //Debug.Log("You can't move here");
        }
    }
    #endregion
}
