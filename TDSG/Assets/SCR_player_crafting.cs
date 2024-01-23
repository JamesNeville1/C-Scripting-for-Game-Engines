using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IzzetUtils;
using static UnityEditor.PlayerSettings;

public class SCR_player_crafting : MonoBehaviour {

    [SerializeField] private Transform slotsParent;
    [SerializeField] private Transform[] slotPos;

    private static SCR_player_crafting instance;
    private Dictionary<Vector2Int, SCR_player_inventory.cellState> craftingSlots = new Dictionary<Vector2Int, SCR_player_inventory.cellState>();
    public static SCR_player_crafting returnInstance() {
        return instance;
    }
    private void Awake() {
        instance = this;
    }
    public void setup() {
        //Create two slots for input
        GameObject slot = SCR_player_inventory.returnInstance().createSlotDisplay("Crafting Slot 1: ", slotsParent, slotPos[0].localPosition);
        craftingSlots.Add(IzzetMain.castVector2((Vector2)slot.transform.localPosition), SCR_player_inventory.cellState.EMPTY);
        slot = SCR_player_inventory.returnInstance().createSlotDisplay("Crafting Slot 2: ", slotsParent, slotPos[1].localPosition);
        craftingSlots.Add(IzzetMain.castVector2((Vector2)slot.transform.localPosition), SCR_player_inventory.cellState.EMPTY);

        //Create one slot for output
        SCR_player_inventory.returnInstance().createSlotDisplay("Output Slot: ", slotsParent, slotPos[2].localPosition);
    }
    public bool tryPlace(SCR_inventory_piece toPlace) {
        foreach (Vector2Int slot in craftingSlots.Keys) {
            if (IzzetMain.castVector2(toPlace.transform.position) != slot)
            {
                return false;
            }
            else {
                place(toPlace, slot);
                return true;
            }
        }
        return false;
    }
    private void place(SCR_inventory_piece toPlace, Vector2Int pos) {
        craftingSlots[pos] = SCR_player_inventory.cellState.OCCUPIED;
        toPlace.transform.position = (Vector2)pos;
    }
    private void remove(Vector2Int pos) {
        craftingSlots[pos] = SCR_player_inventory.cellState.EMPTY;
    }
}
