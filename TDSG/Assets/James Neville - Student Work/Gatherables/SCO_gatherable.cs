using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "SCO_gatherable_", menuName = "ScriptableObjects/Gatherables")]
public class SCO_gatherable : ScriptableObject {
    [SerializeField] [Tooltip("Sprite is shown in overworld, ensure pivot is in correct location")] private Sprite sprite;
    [SerializeField] [Tooltip("Size of colider")] private float interactableRadius = .1f;
    [SerializeField] [Tooltip("What item does it give?")] private SCO_item item;
    [SerializeField] private SCR_master_audio.sfx onPickUp;
    public GameObject gatherableSetup(Vector2 pos, Transform parent) {
        
        GameObject obj = new GameObject(pos.ToString());

        obj.AddComponent<gatherableHook>().hookConstructor(item, onPickUp);

        obj.transform.position = pos;
        obj.transform.parent = parent;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 10000 - (int)obj.transform.position.y;

        PolygonCollider2D col = obj.AddComponent<PolygonCollider2D>();
        col.isTrigger = true;
        //col.radius = interactableRadius;

        return obj;
    }

    public class gatherableHook : MonoBehaviour {
        private SpriteRenderer sr;
        private Color32 originalColor;
        private KeyCode keyToGather = KeyCode.Mouse0;

        [SerializeField] [MyReadOnly] private SCO_item item;
        [SerializeField] [MyReadOnly] private SCR_master_audio.sfx onPickUp;
        public void hookConstructor(SCO_item item, SCR_master_audio.sfx onPickUp) {
            this.item = item;
            this.onPickUp = onPickUp;
        }
        private void Start() {
            sr = GetComponent<SpriteRenderer>();
            originalColor = sr.color;
        }
        private void OnMouseExit() { //Reset colour
            sr.color = originalColor;
        }
        private void OnMouseOver() {
            if (!SCR_master_main.returnInstance().returnPlayerCrafting() && !EventSystem.current.IsPointerOverGameObject()) {
                sr.color = new Color32(originalColor.r, originalColor.g, originalColor.b, 100); //Display that the player is hovering over this object
                if (Input.GetKeyDown(keyToGather)) { //Create instance of puzzle piece, then destroy
                    SCR_inventory_piece.createInstance(item, transform.position, SCR_master_inventory_main.returnInstance().returnCellParent());
                    SCR_master_audio.returnInstance().playRandomEffect(onPickUp);
                    Destroy(gameObject);
                }
            }
        }
    }
}
