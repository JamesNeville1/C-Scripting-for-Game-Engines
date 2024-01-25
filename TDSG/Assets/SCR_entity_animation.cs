using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_entity_animation : MonoBehaviour {
    [SerializeField] [MyReadOnly] private Animator animator;
    public enum AnimationType { IDLE, WALK, HURT, DEATH }

    private Dictionary<AnimationType, string> animations = new Dictionary<AnimationType, string>();

    private AnimationType? current;

    private const string prefix = "ANI_";

    [System.Serializable]
    private struct animationPasser {
        [SerializeField] public AnimationType type;
        [SerializeField] public string name;
    }

    [SerializeField] private animationPasser[] passer;

    private void Awake() {
        animator = GetComponent<Animator>();

        foreach (animationPasser toPass in passer) {
            animations.Add(toPass.type, prefix + toPass.name);
        }
    }
    public void play(AnimationType type) {
        if(animations.ContainsKey(type)) {
            if (current == type) return;
            
            animator.Play(animations[type]);
            current = type;
        }
        else {
            Debug.Log("The animation of type: " + type.ToString() + " does not exist in this context");
        }
    }
}
