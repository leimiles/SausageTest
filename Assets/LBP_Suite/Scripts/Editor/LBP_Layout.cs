using UnityEngine;
using UnityEditor;
using UnityEngine.XR.WSA.Input;
using System.Collections.Generic;
using System.Linq;

namespace MileCode {
    public class LBP_Layout : EditorWindow {
        //static string LBPFolder = "Assets/MilesWorkshop/Prefabs/LBP/";
        static Shader LBPShader;
        [MenuItem("Lightmap/LBP/Layout")]
        public static void LayOutGameObject() {
            LBPShader = Shader.Find("MileShader/LightBakedPrefab");
            if(LBPShader == null) {
                Debug.LogError("LBP Shader can't be found!");
                return;
            }
            FetchGameObjects();
        }

        static void FetchGameObjects() {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("LBP");
            if(gameObjects.Length <= 0) {
                Debug.Log("Can't find gameObjects with the tag(LBP)");
                return;
            } else {
                Debug.LogWarning(gameObjects.Length + " LBP Tags.");
            }

            int LayoutNumber = GetLayoutBaseNumber(gameObjects);
            AllInPosition(gameObjects, LayoutNumber);
        }

        static void AllInPosition(GameObject[] gos, int LayoutNumber) {
            Queue<GameObject> GoQueue = new Queue<GameObject>(gos);
            if(LayoutNumber == 1) {
                // don't have to layout if there's only one object
                return;
            } else {
                int row = 40;
                int column = 40;
                for(int i = 0; i < LayoutNumber; i++) {
                    for(int j = 0; j < LayoutNumber; j++) {
                        if(GoQueue.Count >= 1) {
                            GameObject go = GoQueue.Dequeue();
                            go.transform.position = new Vector3(i * column, 0, j * row);
                        } else {
                            return;
                        }
                    }
                }
            }
        }

        static int GetLayoutBaseNumber(GameObject[] gos) {
            float num = Mathf.Sqrt(gos.Length);
            int layoutBaseNumber;
            if(num == (int)num) {
                layoutBaseNumber = (int)num;
            } else {
                layoutBaseNumber = (int)Mathf.Ceil(num);
            }

            //Debug.Log(layoutBaseNumber);

            return layoutBaseNumber;
        }
    }
}
