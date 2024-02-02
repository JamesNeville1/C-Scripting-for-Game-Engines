using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace IzzetUtils {
    namespace IzzetAttributes {
        public class MyReadOnlyAttribute : PropertyAttribute { }
    }
    public class IzzetMain : MonoBehaviour {
        //Note to marker: I use this to trim the input of the string input by the user for the seed
        public static int validateIntFromString(string invalid, int max = 10000) {
            int valid = 0;
            string validString = "";

            for (int i = 0; i < invalid.Length; i++) {
                if (char.IsNumber(invalid[i])) { 
                    validString += invalid[i];
                }
            }

            if (validString.Length > 0) {
                if(!int.TryParse(validString, out valid)) {
                    valid = max;
                }
            }
            else valid = 0;

            return valid;
        }
        //Simple animation hookup
        public static void animate(Animator animator, string prefix = "", bool moving = false) {
            if (moving) animator.Play(prefix + "move");
            else animator.Play(prefix + "idle");
        }
        //Get mouse position in world
        public static Vector2 getMousePos(Camera cam) {
            Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            return worldPosition;
        }
        //Cast Vector2 to Vector2Int
        public static Vector2Int castVector2(Vector2 vec) {
            return new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
        }
        public static int getRandomWeight(int[] weights, System.Random randomStart = null) {
            int totalWeight = weights.Sum();

            int step = 0;
            if(randomStart == null) randomStart = new System.Random();
            int rand = randomStart.Next(0, totalWeight);

            for (int i = 0; i < weights.Length; i++) {
                step += weights[i];
                if (step > rand) { 
                    return i; //return pos in array
                }
            }
            return -1; //Create deliberate error, something has to very wrong for this to happen.
        }
    }
}
