using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IzzetUtils;
using Unity.VisualScripting;

public class SCR_player_crafting : MonoBehaviour {

    [SerializeField] private RectTransform slotsParent;
    [SerializeField] private Transform[] slotPos;

    private static SCR_player_crafting instance;

    private struct slotData {
        public Vector2Int vec;
        public SCR_inventory_piece piece;

        public slotData(Vector2Int vec, SCR_inventory_piece item) {
            this.vec = vec;
            this.piece = item;
        }
    }
    private slotData[] craftingSlots;

    private SCR_player_inventory invRef;

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
        invRef = SCR_player_inventory.returnInstance();

        craftingSlots = new slotData[3];
        //Create two slots for input
        createSingleSlot("Crafting Slot 1: ", craftingArrayPosName.SLOT1);
        createSingleSlot("Crafting Slot 2: ", craftingArrayPosName.SLOT2);
        createSingleSlot("Output Slot: ", craftingArrayPosName.OUTPUT);

        slotsParent.gameObject.SetActive(false);
    }
    private void createSingleSlot(string name, craftingArrayPosName arrayPos) {
        GameObject slot = invRef.createSlotDisplay(name, slotsParent, slotPos[(int)arrayPos].localPosition);
        craftingSlots[(int)arrayPos] =
            new slotData(IzzetMain.castVector2((Vector2)slot.transform.localPosition), null);
    }
    public bool tryPlace(SCR_inventory_piece toPlace) {

        toPlace.transform.parent = slotsParent;
        bool isSlotOneClose = IzzetMain.castVector2(toPlace.transform.localPosition) == craftingSlots[(int)craftingArrayPosName.SLOT1].vec;
        bool isSlotTwoClose = IzzetMain.castVector2(toPlace.transform.localPosition) == craftingSlots[(int)craftingArrayPosName.SLOT2].vec;

        if (isSlotOneClose) {
            place(toPlace, craftingArrayPosName.SLOT1);

        }
        else if (isSlotTwoClose) {
            place(toPlace, craftingArrayPosName.SLOT2);
        }
        else {
            toPlace.drop();
            return false;
        }

        return true;
    }
    private void place(SCR_inventory_piece toPlace, craftingArrayPosName name) {
        toPlace.transform.localPosition = (Vector2)craftingSlots[(int)name].vec;
        craftingSlots[(int)name].piece = toPlace;
    }
    public void remove(SCR_inventory_piece toRemove) {
        if (craftingSlots[(int)craftingArrayPosName.SLOT1].piece == toRemove) {
            craftingSlots[(int)craftingArrayPosName.SLOT1].piece = null;
            return;
        }

        else if (craftingSlots[(int)craftingArrayPosName.SLOT2].piece == toRemove) {
            craftingSlots[(int)craftingArrayPosName.SLOT2].piece = null;
            return;
        }

    }
    public void toggle() {
        bool currentState = slotsParent.gameObject.activeInHierarchy;
        SCR_master.returnInstance().setGatheringLocked(!currentState);
        slotsParent.gameObject.SetActive(!currentState);

        if (craftingSlots[(int)craftingArrayPosName.SLOT1].piece != null) { craftingSlots[(int)craftingArrayPosName.SLOT1].piece.drop(); }
        if (craftingSlots[(int)craftingArrayPosName.SLOT2].piece != null) { craftingSlots[(int)craftingArrayPosName.SLOT2].piece.drop(); }
    }
    public void craftButton() {
        if (craftingSlots[(int)craftingArrayPosName.SLOT1].piece != null && craftingSlots[(int)craftingArrayPosName.SLOT2].piece != null) {
            craft(craftingSlots[(int)craftingArrayPosName.SLOT1].piece.returnItem(), craftingSlots[(int)craftingArrayPosName.SLOT2].piece.returnItem());
        }
    }

    private void craft(SCO_item a, SCO_item b) {
        if(a.returnName() == "Wood" && b.returnName() == "Wood") {
            print("DOUBLE WOOD");
        }
    }
}
