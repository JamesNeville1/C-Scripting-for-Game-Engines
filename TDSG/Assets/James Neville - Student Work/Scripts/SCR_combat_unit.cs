using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SCR_combat_unit : MonoBehaviour {

    [SerializeField] [MyReadOnly] private List<SCO_item> additionalItems;

    [SerializeField] [MyReadOnly] private SCR_ABS_attributes attributes;
    [SerializeField] [MyReadOnly] private SCR_unit_animation myAnimator;

    public static SCR_combat_unit createInstance(SCO_enemy data) {
        GameObject unit = new GameObject(data.returnName() + " - Obj", 
            typeof(SCR_combat_unit), typeof(SCR_unit_attributes), typeof(SCR_unit_animation), typeof(SpriteRenderer));

        SCR_combat_unit combatUnit = unit.GetComponent<SCR_combat_unit>();
        SCR_unit_attributes attributes = unit.GetComponent<SCR_unit_attributes>();
        SCR_unit_animation myAnimator = unit.GetComponent<SCR_unit_animation>();
        unit.AddComponent<Animator>();

        myAnimator.setup(data.returnAnimatioPrefix(), data.returnAnimator());
        myAnimator.play(SCR_unit_animation.AnimationType.IDLE);

        combatUnit.attributes = attributes;
        combatUnit.myAnimator = myAnimator;


        attributes.setupUniversal(data.returnAthletics(), data.returnDexterity(), data.returnEndurance(), 0);

        SCR_master_main.returnInstance().moveToScene(unit, SCR_master_main.sceneKey.SCE_COMBAT);

        return combatUnit;
    }

    private void Update() {
        myAnimator.play(SCR_unit_animation.AnimationType.IDLE);
    }
    public void move(List<Vector2> steps = null) {
        transform.position = Vector2.zero;
    }
}
