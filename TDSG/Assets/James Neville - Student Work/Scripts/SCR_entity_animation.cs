using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_entity_animation : MonoBehaviour {
    [SerializeField] [Tooltip("What unit is this? Ensure you use the correct naming convention")]private string unitPrefix = "";
    [SerializeField] private animationPasser[] passer;
    [SerializeField] [MyReadOnly] private Animator animator;

    private AnimationType current;
    [System.Serializable] public enum AnimationType { IDLE, WALK, HURT, DEATH }
    private Dictionary<AnimationType, string> animations = new Dictionary<AnimationType, string>();
    private const string globalPrefix = "ANI_";
    [System.Serializable] private struct animationPasser { [SerializeField] public AnimationType type; [SerializeField] public string name; }

    private void Awake() {
        animator = GetComponent<Animator>();

        foreach (animationPasser toPass in passer) {
            animations.Add(toPass.type, globalPrefix + unitPrefix + "_" + toPass.name);
        }
        passer = null; //(The inspector doesn't update but it is null) print(passer);
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
