using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_enemy_", menuName = "ScriptableObjects/Enemy")]
public class SCO_enemy : ScriptableObject {
    [SerializeField] private string enemyName;
    [SerializeField] private RuntimeAnimatorController animator;
    [SerializeField] private string animationUnitPrefix;
    [SerializeField] private SCO_item[] items;

    [SerializeField] private int athletics;
    [SerializeField] private int dexterity;
    [SerializeField] private int endurance;

    public string returnName() {
        return enemyName;
    }
    public RuntimeAnimatorController returnAnimator() {
        return animator;
    }
    public string returnAnimatioPrefix() {
        return animationUnitPrefix;
    }
    public SCO_item[] returnItems() {
        return items;
    }
    public int returnAthletics() {
        return athletics;
    }
    public int returnEndurance() {
        return endurance;
    }
    public int returnDexterity() {
        return dexterity;
    }
}
