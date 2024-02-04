using IzzetUtils;
using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Animations;
using UnityEngine;

public class SCR_overworld_enemy : MonoBehaviour {
    private SpriteRenderer sr;
    private SCR_unit_animation enemyAnimator;
    [SerializeField] private float caughtAtMaxDist = 1f;
    [SerializeField] [MyReadOnly] private SCO_enemy data;

    private enum enemyState {
        WANDERING,
        PLAYER_SEEN,
        PLAYER_CAUGHT
    }

    private enemyState currentState = enemyState.WANDERING;

    [System.Serializable] private struct PASS_directionWeighingStruct { public Vector2 key; public int weight; }
    [SerializeField] private PASS_directionWeighingStruct[] PASS_directionWeighing;
    Dictionary<Vector2, int> directionWeighing = new Dictionary<Vector2, int>();

    [SerializeField] [MyReadOnly] private Vector2 targetPos;
    [SerializeField] [MyReadOnly] private Vector2 dir = Vector2.zero;

    [SerializeField] private float beforeGiveup;
    [SerializeField] private float speedModif;
    [SerializeField] [MyReadOnly] private float overworldSpeed = 1f;

    private SCR_master_main masterRef;

    private void Update() {
        main();
    }

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

        overworldSpeed = data.returnSpeed() * speedModif;
        print(overworldSpeed);
    }

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

    private void flip(float input, SpriteRenderer sr) {  //Make in TDSG utils
        if (input < 0) {
            sr.flipX = true;
        }
        else if (input > 0) {
            sr.flipX = false;
        }
    }

    private void wander() {
        if (Vector2.Distance((Vector2)transform.position, targetPos) < .1f) {
            getNewTarget();
        }

        flip(dir.x, sr);

        sr.color = Color.blue;
    }
    private void chase() {
        Transform target = masterRef.returnPlayer().transform;
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
            SCR_master_timers.returnTickSystem().subscribe(beforeGiveup, () => waitBeforeGiveup());
        }
    }
    private void waitBeforeGiveup() {
        currentState = enemyState.WANDERING;
        getNewTarget();
        masterRef.whatMusic();
        SCR_master_timers.returnTickSystem().unsubscribe(beforeGiveup, () => waitBeforeGiveup());
    }
}
