using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SCR_unit : MonoBehaviour {

    [SerializeField]
    private SCO_ABS_item_weapon mainHand;
    [SerializeField]
    private SCO_ABS_item_weapon offHand;
    [SerializeField]
    private List<SCO_item> additionalItems;

    public IEnumerator move(List<Vector2> pos) {
        int i = 0;
        while (i < pos.Count) {
            if (Vector2.Distance(transform.position, pos[i]) < .1f) {
                transform.position = pos[i];
                i++;
                yield return new WaitForSeconds(2);
            }
            else {
                transform.position = Vector2.MoveTowards(transform.position, pos[i], 2 * Time.deltaTime);
            }
        }
    }
}
