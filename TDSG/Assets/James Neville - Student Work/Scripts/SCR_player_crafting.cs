using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IzzetUtils;

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

        //
        slotsParent.gameObject.SetActive(false);
    }
    public bool tryPlace(SCR_inventory_piece toPlace) {
        float dist = float.PositiveInfinity;
        Vector2Int vec = new Vector2Int();
        foreach (Vector2Int slot in craftingSlots.Keys) {
            if (Vector2.Distance(slot, toPlace.transform.position) < dist){
                vec = slot;
            }
        }

        toPlace.transform.localPosition = (Vector2)vec;

        return true;
    }
    private void place(SCR_inventory_piece toPlace, Vector2Int pos) {
        craftingSlots[pos] = SCR_player_inventory.cellState.OCCUPIED;
        toPlace.transform.position = (Vector2)pos;
    }
    private void remove(Vector2Int pos) {
        craftingSlots[pos] = SCR_player_inventory.cellState.EMPTY;
    }
    public void enable() {
        bool currentState = slotsParent.gameObject.activeInHierarchy;
        SCR_master.returnInstance().setGatheringLocked(!currentState);
        slotsParent.gameObject.SetActive(!currentState);
    }
}
