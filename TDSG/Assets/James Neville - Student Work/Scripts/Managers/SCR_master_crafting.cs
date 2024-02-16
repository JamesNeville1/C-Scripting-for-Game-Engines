using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IzzetUtils;
using IzzetUtils.IzzetAttributes;

public class SCR_master_crafting : MonoBehaviour {

    [SerializeField] private RectTransform slotsParent;
    [SerializeField] private Transform[] slotPos;
    [SerializeField] private recipeData[] recipes;
    [SerializeField] [MyReadOnly] private slotData[] craftingSlots;

    #region Structures & Enums
    private enum craftingArrayPosName { SLOT1, SLOT2, OUTPUT } //Cast to int to cleanly access array, saw no use to justify using a dictionairy
    [System.Serializable] private struct slotData {
        public Vector2Int vec;
        public SCR_inventory_piece piece;

        public slotData(Vector2Int vec, SCR_inventory_piece piece) {
            this.vec = vec;
            this.piece = piece;
        }
    }
    [System.Serializable] private struct recipeData {
        public SCO_item itemA;
        public SCO_item itemB;
        public SCO_item outputItem;
    }
    #endregion
    #region Set Instance
    private static SCR_master_crafting instance;
    public static SCR_master_crafting returnInstance() {
        return instance;
    }
    private void Awake() {
        instance = this;
    }
    #endregion

    #region Setup
    public void setup() {

        craftingSlots = new slotData[3]; //Set array to fit all slots

        //Create two slots for input
        createSingleSlot("Crafting Slot 1: ", craftingArrayPosName.SLOT1);
        createSingleSlot("Crafting Slot 2: ", craftingArrayPosName.SLOT2);
        createSingleSlot("Output Slot: ", craftingArrayPosName.OUTPUT);

        slotsParent.gameObject.SetActive(false); //Hide since we don't want it open at beginning
    }
    private void createSingleSlot(string name, craftingArrayPosName arrayPos) {
        GameObject slot = SCR_master_inventory_main.returnInstance().createSlotDisplay(name, slotsParent, slotPos[(int)arrayPos].localPosition);
        craftingSlots[(int)arrayPos] =
            new slotData(IzzetMain.castToVector2Int((Vector2)slot.transform.localPosition), null);
    }
    #endregion
    #region Authentication & Input
    public bool tryPlace(SCR_inventory_piece toPlace) { //If 
        toPlace.transform.parent = slotsParent;

        //
        bool isSlotOneValid = IzzetMain.castToVector2Int(toPlace.transform.localPosition) == craftingSlots[(int)craftingArrayPosName.SLOT1].vec && checkIfEmpty(craftingArrayPosName.SLOT1);
        bool isSlotTwoValid = IzzetMain.castToVector2Int(toPlace.transform.localPosition) == craftingSlots[(int)craftingArrayPosName.SLOT2].vec && checkIfEmpty(craftingArrayPosName.SLOT2);

        if (isSlotOneValid) {
            place(toPlace, craftingArrayPosName.SLOT1);

        }
        else if (isSlotTwoValid) {
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
    private bool checkIfEmpty(craftingArrayPosName slot) {
        bool isEmpty = craftingSlots[(int)slot].piece == null;

        return isEmpty;
    }
    #endregion
    #region Buttons
    public void toggle() {
        bool currentState = slotsParent.gameObject.activeInHierarchy;
        SCR_master_main.returnInstance().setGatheringLocked(!currentState);
        slotsParent.gameObject.SetActive(!currentState);

        if (craftingSlots[(int)craftingArrayPosName.SLOT1].piece != null) { craftingSlots[(int)craftingArrayPosName.SLOT1].piece.drop(); }
        if (craftingSlots[(int)craftingArrayPosName.SLOT2].piece != null) { craftingSlots[(int)craftingArrayPosName.SLOT2].piece.drop(); }
    }
    public void craftButton() {
        if (craftingSlots[(int)craftingArrayPosName.SLOT1].piece != null && craftingSlots[(int)craftingArrayPosName.SLOT2].piece != null) {
            craft(craftingSlots[(int)craftingArrayPosName.SLOT1].piece, craftingSlots[(int)craftingArrayPosName.SLOT2].piece);
        }
    }
    #endregion
    #region Craft
    private void craft(SCR_inventory_piece a, SCR_inventory_piece b) {
        foreach (recipeData recipe in recipes) {
            if (a.returnItem() == recipe.itemA && b.returnItem() == recipe.itemB || b.returnItem() == recipe.itemA && a.returnItem() == recipe.itemB) {
                remove(a); remove(b);
                Destroy(a.gameObject); Destroy(b.gameObject);

                SCR_inventory_piece createdItem = SCR_inventory_piece.createInstance(recipe.outputItem, craftingSlots[(int)craftingArrayPosName.OUTPUT].vec, slotsParent, false);
                place(createdItem, craftingArrayPosName.OUTPUT);
                return;
            }
        }
    }
    #endregion
}
