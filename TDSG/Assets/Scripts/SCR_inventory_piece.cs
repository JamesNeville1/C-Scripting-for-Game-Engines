using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using IzzetUtils;
using IzzetUtils.IzzetAttributes;

public class SCR_inventory_piece : MonoBehaviour {
    [SerializeField] [Tooltip("IsPressed")] [MyReadOnly] private bool pressed = false;

    private SCR_player_inventory playerInventory; //Reference to inventory
    private List<SpriteRenderer> srs = new List<SpriteRenderer>(); //All sprite renderers of children

    //Create brand new instance from anywhere with no reference required
    public static GameObject createInstance(SCO_item item, Vector2 spawnPos) {
        GameObject newPiece = new GameObject(item.name + " Piece", typeof(SCR_inventory_piece));
        newPiece.transform.position = spawnPos;

        SCR_player_inventory instance = SCR_player_inventory.returnInstance();
        newPiece.transform.parent = instance.returnCellParent();

        SCR_inventory_piece newScript = newPiece.GetComponent<SCR_inventory_piece>();
        newScript.setup(item, instance.returnItemSprite());

        return newPiece;
    }

    private void Awake() {
        playerInventory = SCR_player_inventory.returnInstance();
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) { 
            pressed = true;
            playerInventory.removePiece(this);
        }
        //Debug.Log("This is a " + itemName);
    }
    private void Update() {
        move();
    }

    private void move() { //Move piece via mouse input
        if (pressed) {
            if (Input.GetMouseButton(0)) {
                transform.position = IzzetMain.getMousePos(Camera.main);
            }
            else if (Input.GetMouseButtonUp(0)) {
                pressed = false;
                if (!playerInventory.tryPlaceGrid(this)) {
                    if (!playerInventory.tryPlaceTempSlot(this)) {
                        Debug.Log(gameObject.name + " has been destroyed");
                        Instantiate(playerInventory.returnDestroyVFX(), transform.position, Quaternion.identity);
                        Destroy(this.gameObject);
                    }
                }
            }
        }
    }

    private void setup(SCO_item source, Sprite blockSprite) { //Called from create instance. It creates children acording to the source (item)
        Vector2[] blocks = source.returnSpaces();

        Color blockColour = source.returnColor();

        foreach (Vector2 blockPos in blocks) {
            GameObject newBlock = new GameObject("Block:" + blockPos.x + ", " + blockPos.y, typeof(SpriteRenderer));
            newBlock.transform.parent = transform;
            newBlock.transform.localPosition = blockPos;

            srs.Add(newBlock.GetComponent<SpriteRenderer>());
            int arrayPos = srs.Count - 1;

            srs[arrayPos].sprite = blockSprite;
            srs[arrayPos].color = blockColour;
            srs[arrayPos].sortingOrder = 2;
            srs[arrayPos].sortingLayerName = "Inventory Piece";

            newBlock.AddComponent<BoxCollider2D>().usedByComposite = true;
        }

        
        gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        CompositeCollider2D compCol = gameObject.AddComponent<CompositeCollider2D>();
        compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;
        compCol.isTrigger = true;
    }

    public List<Vector2Int> returnChildren(Vector2Int parentPos) { //Return all positions the piece takes up
        List<Vector2Int> children = new List<Vector2Int>();
        for (int i = 0; i < transform.childCount; i++) {
            Vector2 beforeCast = gameObject.transform.GetChild(i).transform.localPosition + (Vector3Int)parentPos;
            children.Add(IzzetMain.castVector2(beforeCast));
        }
        return children.ToList();
    }
}
