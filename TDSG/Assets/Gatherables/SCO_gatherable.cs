using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "SCO_", menuName = "ScriptableObjects/Gatherables")]
public class SCO_gatherable : ScriptableObject {
    [SerializeField] private Sprite sprite;
    [SerializeField] private float interactableRadius = .1f;
    [SerializeField] private SCO_item item;

    public GameObject gatherableSetup(Vector2 pos, Transform parent) {
        
        GameObject obj = new GameObject(pos.ToString());

        obj.AddComponent<gatherableHook>().hookConstructor(item);

        obj.transform.position = pos;
        obj.transform.parent = parent;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 10000 - (int)obj.transform.position.y;

        CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = interactableRadius;

        return obj;
    }

    public class gatherableHook : MonoBehaviour {
        private SpriteRenderer sr;
        private Color32 originalColor;
        private KeyCode keyToGather = KeyCode.Mouse0;

        [SerializeField] private SCO_item item;
        public void hookConstructor(SCO_item item) {
            this.item = item;
        }
        public SCO_item returnItem() { //Ask how to make override?
            Destroy(gameObject);
            return item;
        }
        private void Start() {
            sr = GetComponent<SpriteRenderer>();
            originalColor = sr.color;
        }
        private void OnMouseEnter() {
            sr.color = new Color32(originalColor.r, originalColor.g, originalColor.b, 100);
        }
        private void OnMouseExit() {
            sr.color = originalColor;
        }
        private void OnMouseOver() {
            if(Input.GetKeyDown(keyToGather)) {
                //SCR_player_inventory.returnInstance().inventory.Add(returnItem());
            }
        }
    }
}
