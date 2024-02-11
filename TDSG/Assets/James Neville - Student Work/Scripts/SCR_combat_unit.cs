using IzzetUtils;
using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SCR_combat_unit : MonoBehaviour {

    [SerializeField][MyReadOnly] private List<SCO_item> additionalItems;

    [SerializeField][MyReadOnly] private SCR_unit_attributes attributes;
    [SerializeField][MyReadOnly] private SCR_unit_animation myAnimator;
    [SerializeField][MyReadOnly] private SpriteRenderer sr;

    public static SCR_combat_unit createInstance(SCO_enemy data) {
        GameObject unit = new GameObject(data.returnName() + " Combat Unit",
            typeof(SCR_combat_unit), typeof(SCR_unit_attributes), typeof(SCR_unit_animation), typeof(SpriteRenderer));

        SCR_combat_unit combatUnit = unit.GetComponent<SCR_combat_unit>();
        combatUnit.attributes = unit.GetComponent<SCR_unit_attributes>();
        combatUnit.myAnimator = unit.GetComponent<SCR_unit_animation>();
        combatUnit.sr = unit.GetComponent<SpriteRenderer>();

        unit.AddComponent<Animator>();

        combatUnit.myAnimator.setup(data.returnAnimatioPrefix(), data.returnAnimator());
        combatUnit.myAnimator.play(SCR_unit_animation.AnimationType.IDLE);

        combatUnit.attributes.setupUniversal(data.returnAthletics(), data.returnDexterity(), data.returnEndurance(), 0);

        SCR_master_main.returnInstance().moveToScene(unit, SCR_master_main.sceneKey.SCE_COMBAT);

        return combatUnit;
    }

    public void move(Vector2Int goTo) {
        StartCoroutine(movementCorutine(goTo));
    }

    private IEnumerator movementCorutine(Vector2Int goTo) {
        int currentStep = 0;

        if(goTo.x < transform.position.x) sr.flipX = true;
        else sr.flipX = false;

        while (Vector2.Distance(transform.position, goTo) > .1f) {
            myAnimator.play(SCR_unit_animation.AnimationType.WALK);
            transform.position = Vector2.MoveTowards(transform.position, goTo, attributes.returnSpeed() * Time.deltaTime);
            currentStep++;
            yield return null;
        }
        myAnimator.play(SCR_unit_animation.AnimationType.IDLE);
        print("a");
        SCR_master_combat.returnInstance().changeData(IzzetMain.castToVector2Int(transform.position), this);
    }

    public SCR_unit_attributes returnAttributes() {
        return attributes;
    }
}
