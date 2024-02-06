using IzzetUtils;
using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Animations;
using UnityEngine;

public class SCR_overworld_enemy : MonoBehaviour {
    #region Structs & Enums
    [System.Serializable] private enum enemyState { WANDERING, PLAYER_SEEN, PLAYER_CAUGHT }
    [System.Serializable] private struct PASS_directionWeighingStruct { public Vector2 key; public int weight; }
    #endregion

    [Header("Require Dev Input")]
    [SerializeField] private PASS_directionWeighingStruct[] PASS_directionWeighing;
    [SerializeField] private float caughtAtMaxDist = 1f;

    [SerializeField] private float beforeGiveup;
    [SerializeField] private float speedModif;

    [Header("Read Only")]
    [SerializeField] [MyReadOnly] private SCO_enemy data;
    [SerializeField] [MyReadOnly] private enemyState currentState = enemyState.WANDERING;
    [SerializeField] [MyReadOnly] private Vector2 targetPos;
    [SerializeField] [MyReadOnly] private Vector2 dir = Vector2.zero;
    [SerializeField] [MyReadOnly] private float overworldSpeed = 1f;

    //
    private SpriteRenderer sr;
    private SCR_unit_animation enemyAnimator;
    Dictionary<Vector2, int> directionWeighing = new Dictionary<Vector2, int>(); //Where the enemy will randomly decide to go, and the likely hood
    private SCR_master_main masterRef;

    #region Unity
    private void Update() {
        main();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        getNewTarget();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<SCR_player_main>() && currentState != enemyState.PLAYER_SEEN) {
            currentState = enemyState.PLAYER_SEEN;
            SCR_master_audio.returnInstance().playRandomMusic(SCR_master_audio.sfx.MUSIC_BATTLE);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.GetComponent<SCR_player_main>() && currentState == enemyState.PLAYER_SEEN) {
            SCR_master_timers.returnInstance().subscribe("Enemy", () => waitBeforeGiveup(), beforeGiveup);
        }
    }
    #endregion
    #region Setup
    public void setup(SCO_enemy data) {
        foreach (PASS_directionWeighingStruct toPass in PASS_directionWeighing) {
            directionWeighing.Add(toPass.key, toPass.weight);
        }
        PASS_directionWeighing = null;

        targetPos = transform.position;

        enemyAnimator = GetComponent<SCR_unit_animation>();
        sr = enemyAnimator.GetComponent<SpriteRenderer>();
        masterRef = SCR_master_main.returnInstance();

        this.data = data;

        overworldSpeed = data.returnDexterity() * speedModif;
        print(overworldSpeed);
    }
    #endregion
    #region Functionality
    private void main() {
        switch (currentState) {
            case enemyState.WANDERING:
                wander();
                break;
            case enemyState.PLAYER_SEEN:
                chase();
                break;
            case enemyState.PLAYER_CAUGHT:
                caught();
                break;
        }

        transform.position = (Vector3)Vector2.MoveTowards((Vector2)transform.position, targetPos, overworldSpeed * Time.deltaTime);
        enemyAnimator.play(SCR_unit_animation.AnimationType.WALK);
    }

    private void wander() {
        if (Vector2.Distance((Vector2)transform.position, targetPos) < .1f) {
            getNewTarget();
        }

        flip(dir.x, sr);

        sr.color = Color.blue;
    }
    private void chase() {
        Transform target = SCR_player_main.returnInstance().transform;
        flip(-target.InverseTransformPoint(transform.position).x, sr);
        targetPos = target.position;

        sr.color = Color.red;

        if(Vector2.Distance((Vector2)transform.position, targetPos) < caughtAtMaxDist){
            //Trigger other chasers

            //Trigger self
            currentState = enemyState.PLAYER_CAUGHT;
        }
    }
    private void caught() {
        //masterRef.loadCombat();
        //Destroy(gameObject);
    }
    private void getNewTarget() {
        dir = directionWeighing.ElementAt(IzzetMain.getRandomWeight(directionWeighing.Values.ToArray())).Key;
        targetPos = dir + (Vector2)transform.position;
    }

    private void waitBeforeGiveup() {
        currentState = enemyState.WANDERING;
        getNewTarget();
        masterRef.whatMusic();
        SCR_master_timers.returnInstance().removeAll("Enemy");
    }
    #endregion
    #region Display
    private void flip(float input, SpriteRenderer sr) {  //Make in TDSG utils
        if (input < 0) {
            sr.flipX = true;
        }
        else if (input > 0) {
            sr.flipX = false;
        }
    }
    #endregion
}
