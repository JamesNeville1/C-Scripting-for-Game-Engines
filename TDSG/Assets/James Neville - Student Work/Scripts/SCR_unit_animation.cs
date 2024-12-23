using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE TO MARKER: This is used to bypas the unity trasitions of the animator, I am still using the built in system.
//I confirmed this with Luke, and he said this was still enough to say that I am using the engines tools still.
public class SCR_unit_animation : MonoBehaviour {
    #region Structs & Enums
    [System.Serializable] public enum AnimationType { IDLE, WALK, DEATH }
    [System.Serializable] public struct PASS_animationStruct { public AnimationType type; public animationDataStruct animationData; }
    [System.Serializable] public struct animationDataStruct { public string name; public int priority; 
        public animationDataStruct(string name, int priority) {
            this.name = name;
            this.priority = priority;
        }
    }
    #endregion

    private const string globalPrefix = "ANI_"; //Starting naming convention
    private const string idle = "_idle";
    private const string walk = "_walk";
    private const string death = "_death";

    [Header("Main")]

    [Header("Read Only")]
    [SerializeField] [MyReadOnly] private Animator animator;
    [SerializeField] [MyReadOnly] private AnimationType current;

    //
    private Dictionary<AnimationType, animationDataStruct> animations = new Dictionary<AnimationType, animationDataStruct>();

    #region Publics
    public void play(AnimationType type) {
        bool isAlreadyPlaying = current == type;
        bool isPriorityLessThanCurrent = animations[type].priority < animations[current].priority;

        if (isAlreadyPlaying || isPriorityLessThanCurrent) return; //If running, don't play again

        animator.Play(animations[type].name);
        current = type;
    }
    public void setAnimationController(RuntimeAnimatorController controller) {
        animator.runtimeAnimatorController = controller;
    }
    #endregion
    #region Setup
    public void setup(string unitPrefix, RuntimeAnimatorController animController) {
        //Setup unity's animator
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animController;

        //Add all animations with strings to dictionairy
        animations.Add(AnimationType.IDLE, new animationDataStruct (globalPrefix + unitPrefix + idle, 0));
        animations.Add(AnimationType.WALK, new animationDataStruct (globalPrefix + unitPrefix + walk, 0));
        animations.Add(AnimationType.DEATH, new animationDataStruct (globalPrefix + unitPrefix + death, 0));
    }
    #endregion
}
