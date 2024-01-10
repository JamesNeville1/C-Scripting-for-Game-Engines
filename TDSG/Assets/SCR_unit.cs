using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SCR_unit : MonoBehaviour {

    [SerializeField]
    private SCO_ABS_item_weapon mainHand;
    [SerializeField]
    private SCO_ABS_item_weapon offHand;
    [SerializeField]
    private List<SCO_item> additionalItems;

    SCR_entity_attributes characterAttributes;

    private void Awake() {
        characterAttributes = GetComponent<SCR_entity_attributes>();
    }

    public void move(List<Vector2> steps = null) {
        transform.position = Vector2.zero;
    }
}
