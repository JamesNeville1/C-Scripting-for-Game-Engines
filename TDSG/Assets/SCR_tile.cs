using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_tile : MonoBehaviour {
    private Color noHover = Color.clear;
    private SpriteRenderer sr;

    [SerializeField]
    private Color onHover = Color.white;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        sr.color = noHover;
    }
    private void OnMouseOver() {
        sr.color = onHover;
        if(Input.GetMouseButton(0))
        SCR_combat_manager.pressed = transform.position;
    }
    private void OnMouseExit() {
        sr.color = noHover;
    }
}
