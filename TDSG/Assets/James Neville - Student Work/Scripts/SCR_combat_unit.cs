using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SCR_combat_unit : MonoBehaviour {

    [SerializeField]
    private List<SCO_item> additionalItems;

    SCR_unit_attributes characterAttributes;

    private void Awake() {
        characterAttributes = GetComponent<SCR_unit_attributes>();
    }

    public void move(List<Vector2> steps = null) {
        transform.position = Vector2.zero;
    }
}
