using System;
using System.Collections;
using UnityEngine;
using IzzetUtils;
using IzzetUtils.IzzetAttributes;

public class SCR_player_main : MonoBehaviour {

    [Header("Require Dev Input")]
    [SerializeField] [Tooltip("Multiply this by the speed attribute")] private float modifOverworldSpeed;
    [Tooltip("Used to determine what animations we will use, THIS IS TEMPORARY")] [SerializeField] private string animationPrefix;
    [SerializeField] private AudioClip[] walkClips;

    [Header("Main")]
    [SerializeField] [MyReadOnly] private float overworldSpeed;

    [Header("Components")]
    [SerializeField] [MyReadOnly] private Rigidbody2D rb;
    [SerializeField] [MyReadOnly] private Animator animator;
    [SerializeField] [MyReadOnly] private SpriteRenderer sr;
    [SerializeField] [MyReadOnly] private SCR_entity_attributes playerAttributes;

    [Header("Will not change once built")]

    //Can't / Won't be serialised
    private static SCR_player_main instance;

    private void Awake() {
        setup();
        instance = this;
    }
    private void Update() {
        playerMovementMain();
    }

    //All movement related stuff here
    private void playerMovementMain() {
        Vector2Int input = returnMovementInput(); //Get Input
        movePlayer(input); //Move Player
        IzzetMain.animate(animator, animationPrefix, rb.velocity != Vector2.zero); //Animate Player
        flipSprite(input); //Check If Should Flip Sprite
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10); //Move Camera to follow player
    }

    #region playerMovementMainFuncs
    private Vector2Int returnMovementInput() {
        Vector2Int movement = new Vector2Int((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));
        return movement;
    }
    private void flipSprite(Vector2Int movement) {
        if (movement.x == -1) {
            sr.flipX = true;
        }
        else if (movement.x == 1) {
            sr.flipX = false;
        }
    }
    private void movePlayer(Vector2 input) {
        if(input.x == 0 || input.y == 0) {
            rb.velocity = input * overworldSpeed;
        }
        else {
            rb.velocity = (input * overworldSpeed) * 0.71f;
        }
    }
    #endregion
    #region Returns & Publics
    public static SCR_player_main returnInstance() {
        return instance;
    }
    public SCR_entity_attributes returnAttributes() {
        return playerAttributes;
    }
    public void changeOverworldSpeed(int modifBy = 0) {
        overworldSpeed = (playerAttributes.speed.returnCurrent() * modifOverworldSpeed) - modifBy;
    }
    public void die(string reason = "") {
        Debug.Log($"You dead fool, you died to {reason}");
    }
    #endregion
    #region Other
    private void setup() {
        //Get Components
        rb = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        playerAttributes = GetComponent<SCR_entity_attributes>();

        //Define Animation Prefix
        animationPrefix = "ANI_" + animationPrefix + "_";

        //Adjust Speed
        changeOverworldSpeed();
    }
    #endregion
}