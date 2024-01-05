using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CreateAssetMenu(fileName = "SCO_", menuName = "ScriptableObjects/Gatherables")]
public class SCO_gatherable : ScriptableObject {
    public Sprite sprite;
    public float interactableRadius;
    [SerializeField] private int health;

    public GameObject gatherableSetup(Vector2 pos, Transform parent) {
        
        GameObject obj = new GameObject(pos.ToString());

        obj.AddComponent<hook>().hookConstructor(health);

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

    public class hook : MonoBehaviour {
        [SerializeField] private int health;
        public void hookConstructor(int health) {
            this.health = health;
        }
    }
}
