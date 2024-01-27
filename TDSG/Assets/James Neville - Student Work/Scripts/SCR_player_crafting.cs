using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IzzetUtils;

public class SCR_player_crafting : MonoBehaviour {

    [SerializeField] private Transform slotsParent;
    [SerializeField] private Transform[] slotPos;

    private static SCR_player_crafting instance;
    private KeyValuePair<Vector2Int, SCR_player_inventory.cellState>[] craftingSlots;

    private enum craftingArrayPosName {
        SLOT1,
        SLOT2,
        OUTPUT
    }
    public static SCR_player_crafting returnInstance() {
        return instance;
    }
    private void Awake() {
        instance = this;
    }
    public void setup() {
        SCR_player_inventory invRef = SCR_player_inventory.returnInstance();

        craftingSlots = new KeyValuePair<Vector2Int, SCR_player_inventory.cellState>[3];
        //Create two slots for input
        GameObject slot = invRef.createSlotDisplay("Crafting Slot 1: ", slotsParent, slotPos[(int)craftingArrayPosName.SLOT1].localPosition);
        craftingSlots[(int)craftingArrayPosName.SLOT1] =
            new KeyValuePair<Vector2Int, SCR_player_inventory.cellState>(IzzetMain.castVector2((Vector2)slot.transform.localPosition), SCR_player_inventory.cellState.EMPTY);
        
        slot = invRef.createSlotDisplay("Crafting Slot 2: ", slotsParent, slotPos[(int)craftingArrayPosName.SLOT2].localPosition);
        craftingSlots[(int)craftingArrayPosName.SLOT2] =
            new KeyValuePair<Vector2Int, SCR_player_inventory.cellState>(IzzetMain.castVector2((Vector2)slot.transform.localPosition), SCR_player_inventory.cellState.EMPTY);

        //Create one slot for output
        slot = invRef.createSlotDisplay("Output Slot: ", slotsParent, slotPos[(int)craftingArrayPosName.OUTPUT].localPosition);
        craftingSlots[(int)craftingArrayPosName.OUTPUT] =
            new KeyValuePair<Vector2Int, SCR_player_inventory.cellState>(IzzetMain.castVector2((Vector2)slot.transform.localPosition), SCR_player_inventory.cellState.EMPTY);

        slotsParent.gameObject.SetActive(false);
    }
    public bool tryPlace(SCR_inventory_piece toPlace) {
        bool isSlotOneClose = Vector2.Distance(toPlace.transform.position, craftingSlots[(int)craftingArrayPosName.SLOT1].Key)
            < Vector2.Distance(toPlace.transform.position, craftingSlots[(int)craftingArrayPosName.SLOT2].Key);

        toPlace.transform.parent = slotsParent;

        if (isSlotOneClose) {
            toPlace.transform.localPosition = (Vector2)craftingSlots[(int)craftingArrayPosName.SLOT1].Key;
        }
        else {
            toPlace.transform.localPosition = (Vector2)craftingSlots[(int)craftingArrayPosName.SLOT2].Key;
        }

        return true;
    }
    private void place(SCR_inventory_piece toPlace, Vector2Int pos) {
        //craftingSlots[pos] = SCR_player_inventory.cellState.OCCUPIED;
        //toPlace.transform.position = (Vector2)pos;
    }
    private void remove(Vector2Int pos) {
        //craftingSlots[pos] = SCR_player_inventory.cellState.EMPTY;
    }
    public void enable() {
        bool currentState = slotsParent.gameObject.activeInHierarchy;
        SCR_master.returnInstance().setGatheringLocked(!currentState);
        slotsParent.gameObject.SetActive(!currentState);
    }
}
