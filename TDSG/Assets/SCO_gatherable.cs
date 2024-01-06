using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_", menuName = "ScriptableObjects/Gatherables")]
public class SCO_gatherable : ScriptableObject {
    [SerializeField] private Sprite sprite;
    [SerializeField] private float interactableRadius = 1;
    [SerializeField] private int health = 1;
    [SerializeField] private int maxGive = 1;
    [SerializeField] private int minGive = 1;
    [SerializeField] private SCO_item item;

    public GameObject gatherableSetup(Vector2 pos, Transform parent) {
        
        GameObject obj = new GameObject(pos.ToString());

        obj.AddComponent<gatherableHook>().hookConstructor(health, maxGive, minGive, item);

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
        [System.Serializable]
        private struct itemData {
            public int health;
            public int maxGive;
            public int minGive;
            public SCO_item item;
        }
        [SerializeField] private itemData data;
        public void hookConstructor(int health, int maxGive, int minGive, SCO_item item = null) {
            data.health = health;
            data.maxGive = maxGive;
            data.minGive = minGive;
            data.item = item;
        }
        public SCO_item returnItem() { //Ask how to make override?
            Destroy(gameObject);
            return data.item;
        }
    }
}
