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
        //public static Sprite mergeSprite(Sprite baseSprite, Sprite pasted) {
        //    Texture2D toPass = baseSprite.texture;

        //    Texture2D toPaste = pasted.texture;
        //    toPaste.Reinitialize((int)pasted.rect.x, (int)pasted.rect.y);
            
        //    toPaste.l

        //    for (int x = 0; x <toPass.width; x++) {
        //        for (int y = 0; y < toPass.height; y++) {
        //            if(toPaste.GetPixel(x, y).a != 0) {
                        
        //            }
        //        }
        //    }

        //    return Sprite.Create(toPaste, new Rect(0,0, toPass.width, toPass.height), new Vector2(.5f, .5f), 16);
        //}
    }
}
