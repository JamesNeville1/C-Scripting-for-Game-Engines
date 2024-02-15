using System;
using System.Collections;
using UnityEngine;
using IzzetUtils;
using IzzetUtils.IzzetAttributes;
using UnityEngine.Tilemaps;

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
    [SerializeField] [MyReadOnly] private SCR_overworld_player_attributes playerAttributes;
    [SerializeField] [MyReadOnly] private SCR_unit_animation playerAnimation;

    [Header("Will not change once built")]
    [SerializeField] private float timeBetweenWalkSFX;
    [SerializeField] private float swimSpeedModif;
    [SerializeField] private GameObject waterResponsibleRef;

    [Header("Other")]
    [SerializeField] [MyReadOnly] private bool footstepCoroutineRunning = false;
    [SerializeField] [MyReadOnly] private float speedOrigin;

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
        playerMovementMain();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject == SCR_master_generation.returnInstance().returnGroundTilemap().gameObject) {
            startWalking();
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject == SCR_master_generation.returnInstance().returnGroundTilemap().gameObject) {
            startSwimming();
        }
    }
    #endregion
    #region Main
    //All movement related stuff here
    private void playerMovementMain() {
        Vector2Int input = returnMovementInput(); //Get Input
        movePlayer(input); //Move Player
        flipSprite(input); //Check If Should Flip Sprite
        animate(input); //Do idle if still, and walk if moving
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10); //Move Camera to follow players
    }
    private void startSwimming() {
        speed = speedOrigin * swimSpeedModif;
        waterResponsibleRef.SetActive(true);
    }
    private void startWalking() {
        speed = speedOrigin;
        waterResponsibleRef.SetActive(false);
    }
    #endregion
    #region playerMovementMainFuncs
    private Vector2Int returnMovementInput() {
        Vector2Int movement = new Vector2Int((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));
        return movement;
    }
    private void flipSprite(Vector2Int input) {
        if (input.x == -1) {
            sr.flipX = true;
        }
        else if (input.x == 1) {
            sr.flipX = false;
        }
    }
    private void movePlayer(Vector2 input) {
        if (input.x == 0 && input.y == 0) { //If we aren't moving
            if (footstepCoroutineRunning == true) {
                rb.velocity = Vector2.zero;
                StopAllCoroutines();
                footstepCoroutineRunning = false;
            }
        }
        else { //If we are moving
            if(footstepCoroutineRunning == false) {
                StartCoroutine(Footstepsounds());
            }
            
            if (input.x == 0 || input.y == 0) {
                rb.velocity = input * speed;
            }
            else {
                rb.velocity = (input * speed) * 0.71f;
            }
        }
    }
    private void animate(Vector2Int input) {
        if (input.x != 0 || input.y != 0) {
            playerAnimation.play(SCR_unit_animation.AnimationType.WALK);
        }
        else {
            playerAnimation.play(SCR_unit_animation.AnimationType.IDLE);
        }
    }
    private IEnumerator Footstepsounds() {
        footstepCoroutineRunning = true;
        while (true) {
            SCR_master_audio.returnInstance().playRandomEffect(SCR_master_audio.sfx.WALK_STEP, .15f);
            yield return new WaitForSeconds(timeBetweenWalkSFX);
        }
    }
    #endregion
    #region Returns & Publics
    public SCR_overworld_player_attributes returnAttributes() {
        return playerAttributes;
    }
    public void changeOverworldSpeed(int modifBy = 1) {
        speedOrigin = (playerAttributes.returnSpeed() * modifOverworldSpeed) * modifBy;
    }
    public void readyToDie() {
        SCR_master_inventory_main.returnInstance().destroyAll();
        speed = 0;
        rb.velocity = Vector2.zero;
        SCR_master_main.returnInstance().setGatheringLocked(true);
        this.enabled = false;
    }
    #endregion
    #region Setup
    public void setup(SCO_character_preset preset) {
        //Get Components
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerAttributes = GetComponent<SCR_overworld_player_attributes>();
        playerAnimation = GetComponent<SCR_unit_animation>();

        //
        playerAnimation.setup(preset.returnAnimationPrefix(), preset.returnAnimations());
        playerAttributes.setupUniversal(preset.returnStartingStats());

        //Adjust Speed
        changeOverworldSpeed();

        speed = speedOrigin;
    }
    #endregion
}