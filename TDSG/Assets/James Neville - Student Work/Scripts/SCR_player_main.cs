using System;
using System.Collections;
using UnityEngine;
using IzzetUtils;
using IzzetUtils.IzzetAttributes;
using UnityEngine.Rendering;

public class SCR_player_main : MonoBehaviour {

    [Header("Require Dev Input")]
    [SerializeField] [Tooltip("Multiply this by the speed attribute")] private float modifOverworldSpeed;

    [Header("Main")]
    [SerializeField] [MyReadOnly] private float overworldSpeed;

    [Header("Components")]
    [SerializeField] [MyReadOnly] private Rigidbody2D rb;
    [SerializeField] [MyReadOnly] private SpriteRenderer sr;
    [SerializeField] [MyReadOnly] private SCR_unit_attributes playerAttributes;
    [SerializeField] [MyReadOnly] private SCR_unit_animation playerAnimation;

    [Header("Will not change once built")]
    [SerializeField] private float timeBetweenWalkSFX;

    [Header("Other")]
    [SerializeField] [MyReadOnly] private bool courtineRunning = false;

    [Header("Attribute Vars")]
    [SerializeField] private float timeBetweenHungerTicks;
    [SerializeField] private float timeBetweenHungerDamageTicks;

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
    #endregion
    #region Main
    //All movement related stuff here
    private void playerMovementMain() {
        Vector2Int input = returnMovementInput(); //Get Input
        movePlayer(input); //Move Player
        flipSprite(input); //Check If Should Flip Sprite
        animate(input); //Do idle if still, and walk if moving
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10); //Move Camera to follow player
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
            if (courtineRunning == true) {
                rb.velocity = Vector2.zero;
                StopAllCoroutines();
                courtineRunning = false;
            }
        }
        else { //If we are moving
            if(courtineRunning == false) {
                StartCoroutine(Footstepsounds());
            }
            
            if (input.x == 0 || input.y == 0) {
                rb.velocity = input * overworldSpeed;
            }
            else {
                rb.velocity = (input * overworldSpeed) * 0.71f;
            }
        }
    }
    private void animate(Vector2Int input) {
        if (playerAnimation.enabled) {
            if(input.x != 0 || input.y != 0) {
                playerAnimation.play(SCR_unit_animation.AnimationType.WALK);
            }
            else {
                playerAnimation.play(SCR_unit_animation.AnimationType.IDLE);
            }
        }
    }
    private IEnumerator Footstepsounds() {
        while (true) {
            courtineRunning = true;
            SCR_master_audio.returnInstance().playRandomEffect(SCR_master_audio.sfx.WALK_STEP, .15f);
            yield return new WaitForSeconds(timeBetweenWalkSFX);
        }
    }
    #endregion
    #region Returns & Publics
    public SCR_unit_attributes returnAttributes() {
        return playerAttributes;
    }
    public void changeOverworldSpeed(int modifBy = 0) {
        overworldSpeed = playerAttributes.attributes.speed * modifOverworldSpeed;
    }
    public void readyToDie() {
        overworldSpeed = 0;
        rb.velocity = Vector2.zero;
        this.enabled = false;
    }
    public float returnTimeBetweenHungerTicks() {
        return timeBetweenHungerTicks;
    }
    public float returnTimeBetweenHungerDamageTicks() {
        return timeBetweenHungerDamageTicks;
    }
    #endregion
    #region Setup
    public void setup() {
        //Get Components
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerAttributes = GetComponent<SCR_unit_attributes>();
        playerAnimation = GetComponent<SCR_unit_animation>();

        //
        playerAnimation.setup();
        playerAttributes.setup();

        //Adjust Speed
        changeOverworldSpeed();
    }
    #endregion
}