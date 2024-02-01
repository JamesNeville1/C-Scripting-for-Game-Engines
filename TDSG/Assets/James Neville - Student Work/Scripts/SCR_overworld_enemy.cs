using IzzetUtils;
using IzzetUtils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_overworld_enemy : MonoBehaviour {
    private SpriteRenderer sr;
    private SCR_entity_animation enemyAnimator;

    private enum enemyState {
        WANDERING,
        PLAYER_SEEN,
        PLAYER_CAUGHT
    }

    private enemyState myState;

    [System.Serializable] private struct PASS_directionWeighingStruct { public Vector2 key; public int weight; }
    [SerializeField] private PASS_directionWeighingStruct[] PASS_directionWeighing;
    Dictionary<Vector2, int> directionWeighing = new Dictionary<Vector2, int>();

    [SerializeField] [MyReadOnly] private Vector2 targetPos;
    [SerializeField] [MyReadOnly] private Vector2 dir = Vector2.zero;

    [SerializeField] private float beforeGiveup;
    [SerializeField] private float speed = 1f;

    private SCR_master masterRef;

    private void Awake() {
        foreach (PASS_directionWeighingStruct toPass in PASS_directionWeighing) {
            directionWeighing.Add(toPass.key, toPass.weight);
        }
        PASS_directionWeighing = null;

        targetPos = transform.position;

        enemyAnimator = GetComponent<SCR_entity_animation>();
        sr = enemyAnimator.GetComponent<SpriteRenderer>();
        masterRef = SCR_master.returnInstance();
    }
    private void Start() {
    }
    private void Update() {
        main();
    }
    private void main() {
        switch (myState) {
            case enemyState.WANDERING:
                wander();
                break;
            case enemyState.PLAYER_SEEN:
                chase();
                break;
            case enemyState.PLAYER_CAUGHT:
                break;
        }

        transform.position = (Vector3)Vector2.MoveTowards((Vector2)transform.position, targetPos, speed * Time.deltaTime);
        enemyAnimator.play(SCR_entity_animation.AnimationType.WALK);
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
    }
    private void getNewTarget() {
        dir = directionWeighing.ElementAt(IzzetMain.getRandomWeight(directionWeighing.Values.ToArray())).Key;
        targetPos = dir + (Vector2)transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        getNewTarget();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<SCR_player_main>()) {
            myState = enemyState.PLAYER_SEEN;
            StopCoroutine(waitBeforeGiveup());
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.GetComponent<SCR_player_main>()) {
            StartCoroutine(waitBeforeGiveup());
            print("a");
        }
    }
    private IEnumerator waitBeforeGiveup() {
        yield return new WaitForSeconds(beforeGiveup);
        myState = enemyState.WANDERING;
        getNewTarget();
    }
}
