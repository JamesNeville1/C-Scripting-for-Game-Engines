using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_destroy_after : MonoBehaviour {
    [SerializeField] private float destroyAfter = 1;

    private void Awake() {
        StartCoroutine(destroyAfterFunc(destroyAfter));
    }

    private IEnumerator destroyAfterFunc(float destroyAfter) {
        yield return new WaitForSeconds(destroyAfter);
        Destroy(this.gameObject);
    }
}
