using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_character_", menuName = "ScriptableObjects/Character")]
public class SCO_character_preset : ScriptableObject {
    [SerializeField] string characterName;
    [SerializeField] RuntimeAnimatorController animations;
    [SerializeField] private string animationPrefix;
    [SerializeField] private SCR_player_attributes.entStats startingStats;
    [SerializeField] private Sprite characterSelectionSprite;
    [SerializeField] [TextArea(20, 20)] private string flavourText;

    public Sprite returnCharacterSelectionSprite() {
        return characterSelectionSprite;
    }
    public string returnAnimationPrefix() {
        return animationPrefix;
    }
    public RuntimeAnimatorController returnAnimations() {
        return animations;
    }
    public SCR_player_attributes.entStats returnStartingStats() {
        return startingStats;
    }
    public string returnName() {
        return characterName;
    }
    public string returnFlavourText() {
        return flavourText;
    }
}