using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SCR_combat_unit : MonoBehaviour {

    [SerializeField] [MyReadOnly] private List<SCO_item> additionalItems;

    [SerializeField] [MyReadOnly] private SCR_ABS_attributes attributes;
    [SerializeField] [MyReadOnly] private SCR_unit_animation myAnimator;

    public static SCR_combat_unit createInstance(string objName, int athletics, int dexterity, int endurance) {
        GameObject unit = new GameObject(objName, typeof(SCR_combat_unit), typeof(SCR_unit_attributes), typeof(SCR_unit_animation), typeof(SpriteRenderer));

        SCR_combat_unit combatUnit = unit.GetComponent<SCR_combat_unit>();
        SCR_unit_attributes attributes = unit.GetComponent<SCR_unit_attributes>();
        SCR_unit_animation myAnimator = unit.GetComponent<SCR_unit_animation>();
        
        combatUnit.attributes = attributes;
        combatUnit.myAnimator = myAnimator;

        attributes.setupUniversal(athletics, dexterity, endurance, 0);

        return combatUnit;
    }

    public void move(List<Vector2> steps = null) {
        transform.position = Vector2.zero;
    }
}
