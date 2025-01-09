using System;
using System.Collections;
using UnityEngine;
using IzzetUtils;
using IzzetUtils.IzzetAttributes;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

public class SCR_player_main : MonoBehaviour {

    [Header("Require Dev Input")]
    [SerializeField] [Tooltip("Multiply this by the speed attribute")] private float modifOverworldSpeed;

    [Header("Animation - Require Dev Input")]
    [SerializeField] private string unitPrefix;
    [SerializeField] private RuntimeAnimatorController controller;

    [Header("Main")]
    [SerializeField] [MyReadOnly] private float speed;

    [Header("Components")]
    [SerializeField] [MyReadOnly] private Rigidbody2D rb;
    [SerializeField] [MyReadOnly] private SpriteRenderer sr;
    [SerializeField] [MyReadOnly] private SCR_player_attributes playerAttributes;
    [SerializeField] [MyReadOnly] private SCR_unit_animation playerAnimation;

    [Header("Will not change once built")]
    [SerializeField] [MyReadOnly] private float timeBetweenStepSFX;
    [SerializeField] private float timeBetweenWalkSFX;
    [SerializeField] private float timeBetweenSwimSFX;
    [SerializeField] private float swimSpeedModif;
    [SerializeField] private GameObject waterResponsibleRef;

    [Header("Other")]
    [SerializeField] [MyReadOnly] private bool footstepCoroutineRunning = false;
    [SerializeField] [MyReadOnly] private float speedOrigin;
    [SerializeField] [MyReadOnly] private SCR_master_audio.sfx stepID = SCR_master_audio.sfx.WALK_STEP;
    Coroutine stepCoroutine;

    #region Set Instance
    private static SCR_player_main instance;
    public static SCR_player_main returnInstance() {
        return instance;
    }
    private void Awake() {
        instance = this;
    }
    #endregion
    #region Unity
    private void Update() {
        PlayerMain();
    }
    #region Swimming / Walking Checks
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject == SCR_master_map.instance.ReturnGroundTilemap().gameObject) {
            StartWalking();
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject == SCR_master_map.instance.ReturnGroundTilemap().gameObject) {
            StartSwimming();
        }
    }
    #endregion
    #endregion
    #region Main
    //All movement related stuff here
    private void PlayerMain() {
        Vector2Int input = ReturnMovementInput(); //Get Input
        MovePlayer(input); //Move Player
        FlipSprite(input); //Check If Should Flip Sprite
        Animate(input); //Do idle if still, and walk if moving
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10); //Move Camera to follow players
    }
    private void StartSwimming() {
        timeBetweenStepSFX = timeBetweenSwimSFX;
        stepID = SCR_master_audio.sfx.SWIM_STEP;
        speed = speedOrigin * swimSpeedModif;
        waterResponsibleRef.SetActive(true);
    }
    private void StartWalking() {
        timeBetweenStepSFX = timeBetweenWalkSFX;
        stepID = SCR_master_audio.sfx.WALK_STEP;
        speed = speedOrigin;
        waterResponsibleRef.SetActive(false);
    }
    #endregion
    #region playerMainFuncs
    private Vector2Int ReturnMovementInput() { //Get input with Input.GetAxisRaw
        Vector2Int movement = new Vector2Int((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));
        return movement;
    }
    private void FlipSprite(Vector2Int input) { 
        if (input.x == -1) {
            sr.flipX = true;
        }
        else if (input.x == 1) {
            sr.flipX = false;
        }
    }
    //Note: Would use Enhanced Input in future
    private void MovePlayer(Vector2 input) {
        
        //If we aren't moving
        if (input.x == 0 && input.y == 0) {
            if (footstepCoroutineRunning == true) {
                rb.linearVelocity = Vector2.zero;
                StopAllCoroutines();
                footstepCoroutineRunning = false;
            }
        }

        //If we are moving
        else {
            if(footstepCoroutineRunning == false) {
                stepCoroutine = StartCoroutine(Footstepsounds());
            }
            
            if (input.x == 0 || input.y == 0) {
                rb.linearVelocity = input * speed;
            }
            else { //If both input keys are down, modify speed
                rb.linearVelocity = (input * speed) * 0.75f;
            }
        }
    }
    private void Animate(Vector2Int input) {
        if (input.x != 0 || input.y != 0) { //If moving, show walk animation, if not do idle
            playerAnimation.play(SCR_unit_animation.AnimationType.WALK);
        }
        else {
            playerAnimation.play(SCR_unit_animation.AnimationType.IDLE);
        }
    }
    private IEnumerator Footstepsounds() {
        footstepCoroutineRunning = true;
        while (true) {
            SCR_master_audio.instance.PlayRandomEffect(stepID, .25f);
            yield return new WaitForSeconds(timeBetweenStepSFX);
        }
    }
    #endregion
    #region Returns & Publics
    public SCR_player_attributes ReturnAttributes() {
        return playerAttributes;
    }
    public void ChangeOverworldSpeed(int modifBy = 1) {
        speedOrigin = (playerAttributes.ReturnSpeed() * modifOverworldSpeed) * modifBy;
    }
    public void ReadyToDie() {
        if(stepCoroutine != null) StopCoroutine(stepCoroutine);

        SCR_master_inventory_main.instance.DestroyAll();
        speed = 0;
        rb.linearVelocity = Vector2.zero;
        SCR_master_main.instance.SetGatheringLocked(true);
        this.enabled = false;
    }
    #endregion
    #region Setup
    public void Setup(SCO_character_preset preset) {
        //Get Components
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerAttributes = GetComponent<SCR_player_attributes>();
        playerAnimation = GetComponent<SCR_unit_animation>();

        //
        playerAnimation.setup(preset.returnAnimationPrefix(), preset.returnAnimations());
        playerAttributes.SetupUniversal(preset.returnStartingStats());

        //Adjust Speed
        ChangeOverworldSpeed();

        speed = speedOrigin;
    }
    #endregion
}