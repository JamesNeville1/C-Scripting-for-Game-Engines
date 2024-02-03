using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_enemy_", menuName = "ScriptableObjects/Enemy")]
public class SCO_enemy : ScriptableObject {
    [SerializeField] private string enemyName;
    [SerializeField] private AnimatorController animations;
    [SerializeField] private string animationUnitPrefix;
    [SerializeField] private SCO_item[] items;

    [SerializeField] private int maxHealth;
    [SerializeField] private int speed;

    private enum fightStyle {
        STUPID,
        SMART
    }

    [SerializeField] private fightStyle enemyFightStyle;

    public string returnName() {
        return enemyName;
    }
    public AnimatorController returnAnimator() {
        return animations;
    }
    public string returnAnimatioPrefix() {
        return animationUnitPrefix;
    }
    public SCO_item[] returnItems() {
        return items;
    }
    public int returnMaxHealth() {
        return maxHealth;
    }
    public int returnSpeed() {
        return speed;
    }
}
