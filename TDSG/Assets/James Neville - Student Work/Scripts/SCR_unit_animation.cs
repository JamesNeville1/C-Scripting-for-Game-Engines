using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor.Animations;


//NOTE TO MARKER: This is used to bypas the unity trasitions of the animator, I am still using the built in system.
//I confirmed this with Luke, and he said this was still enough to say that I am using the engines tools still.
public class SCR_unit_animation : MonoBehaviour {
    #region Structs & Enums
    [System.Serializable] public enum AnimationType { IDLE, WALK, HURT, DEATH }
    [System.Serializable] private struct PASS_animationStruct { [SerializeField] public AnimationType type; [SerializeField] public animationDataStruct animationData; }
    [System.Serializable] private struct animationDataStruct { [SerializeField] public string name; [SerializeField] public int priority; 
        public animationDataStruct(string name, int priority) {
            this.name = name;
            this.priority = priority;
        }
    }
    #endregion

    private const string globalPrefix = "ANI_"; //Starting naming convention

    [Header("Main")]
    [SerializeField] private PASS_animationStruct[] PASS_animation; //Passed to dictionairy on awake
    [SerializeField] [Tooltip("What unit is this? Ensure you use the correct naming convention")] private string unitPrefix = "";

    [Header("Read Only")]
    [SerializeField] [MyReadOnly] private Animator animator;
    [SerializeField] [MyReadOnly] private AnimationType current;

    //
    private Dictionary<AnimationType, animationDataStruct> animations = new Dictionary<AnimationType, animationDataStruct>();

    #region Publics
    public void play(AnimationType type) {
        bool isAlreadyPlaying = current == type;
        bool isPriorityLessThanCurrent = animations[type].priority < animations[current].priority;

        if (isAlreadyPlaying || isPriorityLessThanCurrent) return;

        animator.Play(animations[type].name);
        current = type;
    }
    public void setPrefix(string prefix) {
        unitPrefix = prefix;
    }
    public void setAnimationController(AnimatorController controller) {
        animator.runtimeAnimatorController = controller;
    }
    #endregion
    #region Setup
    public void setup() {
        animator = GetComponent<Animator>();

        foreach (PASS_animationStruct toPass in PASS_animation) {
            animations.Add(toPass.type, new animationDataStruct (globalPrefix + unitPrefix + "_" + toPass.animationData.name, toPass.animationData.priority));
        }
        PASS_animation = null; //(The inspector doesn't update but it is null) print(passer);
    }
    #endregion
}
