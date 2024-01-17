using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;
using IzzetUtils;
using IzzetUtils.IzzetAttributes;

public class SCR_player_main : MonoBehaviour {

    [Header("Main")]
    [SerializeField] [MyReadOnly] private float overworldSpeed;

    [Header("Components")]
    [SerializeField] [MyReadOnly] private Rigidbody2D rb;
    [SerializeField] [MyReadOnly] private Animator animator;
    [SerializeField] [MyReadOnly] private SpriteRenderer sr;
    [SerializeField] [MyReadOnly] private SCR_entity_attributes att;

    [Header("Animation")]
    [Tooltip("Used to determine what animations we will use, THIS IS TEMPORARY")][SerializeField] private string animationPrefix;

    [Header("Will not change once built")]
    [SerializeField] [Tooltip("Multiply this by the speed attribute")] private float modifOverworldSpeed;
    [SerializeField] private static SCR_player_main instance;

    [Header("SFX")]
    [SerializeField] private AudioClip[] walkClips;

    private void Awake() {
        //Get Components
        rb = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        att = GetComponent<SCR_entity_attributes>();

        //Define Animation Prefix
        animationPrefix = "ANI_" + animationPrefix + "_";

        //Adjust Speed
        changeOverworldSpeed();

        instance = this;
    }
    private void Start() {
        SCR_audio_manager.playEffect(walkClips);
    }
    private void Update() {
        playerMovementMain();
    }
    public static SCR_player_main returnInstance() {
        return instance;
    }
    //All movement related stuff here
    private void playerMovementMain() {
        Vector2 input = returnMovementInput(); //Get Input
        movePlayer(input); //Move Player
        IzzetMain.animate(animator, animationPrefix, rb.velocity != Vector2.zero); //Animate Player
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10); //Move Camera to follow player
    }
    #region movement
    private Vector2 returnMovementInput() {
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (movement.x == -1) {
            sr.flipX = true;
        }
        else if (movement.x == 1){
            sr.flipX = false;
        }
        return movement;
    }
    private void movePlayer(Vector2 input) {
        if(input.x == 0 || input.y == 0) {
            rb.velocity = input * overworldSpeed;
        }
        else {
            rb.velocity = (input * overworldSpeed) * 0.71f;
        }
    }
    public void changeOverworldSpeed(int modifBy = 0) {
        overworldSpeed = (att.speed.current * modifOverworldSpeed) - modifBy;
    }
    #endregion
}