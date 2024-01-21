using System;
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
        public static void animate(Animator animator, string prefix, bool moving = false) { //Simple animation hookup
            if (moving) animator.Play(prefix + "move");
            else animator.Play(prefix + "idle");
        }
        public static Vector2 getMousePos(Camera cam) { //Get mouse position in world
            Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            return worldPosition;
        }
        public static Vector2Int castVector2(Vector2 vec) {
            return new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
        }
        public static Sprite mergeSprite(Sprite baseSprite, Sprite pasted) {
            for (int x = 0; x < baseSprite.texture.width; x++) {
                for (int y = 0; y < baseSprite.texture.height; y++) {
                    Color32 color = pasted.texture.GetPixel(x, y);
                    if(color.a != 0) {
                        baseSprite.texture.SetPixel(x, y, color);
                    }
                }
            }
            return baseSprite;
        }
    }
}
