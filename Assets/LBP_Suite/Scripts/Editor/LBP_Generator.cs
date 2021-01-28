using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using MileCode;
using System.Collections.Generic;

namespace MileCode {
    public class LBP_Generator : EditorWindow {
        static string LBPFolder = "Assets/LBP_Suite/Prefabs/LBP/";
        static string LBPMaterialsFolder = "Assets/LBP_Suite/Materials/LBP_Materials/";
        static Shader LBPShader;
        [MenuItem("Lightmap/LBP/Generate")]
        public static void GenerateLBP() {
            LBPShader = Shader.Find("MileShader/LightBakedPrefabIntensity");
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
            FindPotentialLBP2(gameObjects);
        }

        static void FindPotentialLBP2(GameObject[] gameObjects) {
            foreach(GameObject go in gameObjects) {
                //List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
                /*
                MeshRenderer rootRender = go.GetComponent<MeshRenderer>();
                if(rootRender != null) {
                    meshRenderers.Add(rootRender);
                }
                */
                MeshRenderer[] childrenRenders = go.GetComponentsInChildren<MeshRenderer>();
                //meshRenderers.AddRange(childrenRenders);
                if(CanUseLBPSettings2(childrenRenders)) {
                    MakeLBP(go);
                } else {
                    Debug.Log("Mesh renderer is not available for LBP Settings.");
                }
            }
        }

        static bool CanUseLBPSettings2(MeshRenderer[] meshRenderers) {
            bool enableLBP = false;
            foreach(MeshRenderer mr in meshRenderers) {
                if(mr.lightmapIndex >= 0) {
                    LBP_Settings lbpSettings = mr.GetComponent<LBP_Settings>();
                    if(lbpSettings == null) {
                        lbpSettings = mr.gameObject.AddComponent<LBP_Settings>();
                    }
                    lbpSettings.lightmapScaleAndOffset = mr.lightmapScaleOffset;
                    lbpSettings.lightmapIndex = mr.lightmapIndex;
                    Texture2D sceneLightmap = FetchCurrentLightmap(mr.lightmapIndex);
                    lbpSettings.lightmap = sceneLightmap;
                    PrepareLBPMaterial(mr);
                    enableLBP = true;
                } else {
                    Debug.LogWarning(mr.gameObject.name + "(static) doesn't have lightmap enabled, LBP can't be generated for this part.");
                }
            }
            return enableLBP;
        }

        static void PrepareLBPMaterial(MeshRenderer mr) {
            foreach(Material mat in mr.sharedMaterials) {
                //Texture mainTexture = mat.GetTexture("_MainTex");
                if(mat.name.EndsWith("_LBP")) {
                    mat.name = mat.name.Replace("_LBP", "");
                }
                string LBPMatName = mat.name + "_LBP";
                Material lbpMat = AssetDatabase.LoadAssetAtPath<Material>(LBPMaterialsFolder + LBPMatName + ".mat");
                if(lbpMat == null) {
                    lbpMat = new Material(LBPShader);
                    AssetDatabase.CreateAsset(lbpMat, LBPMaterialsFolder + LBPMatName + ".mat");
                }
                mr.sharedMaterial = lbpMat;
            }
        }

        static Texture2D FetchCurrentLightmap(int lightmapIndex) {
            LightmapData lightmapData = LightmapSettings.lightmaps[lightmapIndex];
            return lightmapData.lightmapColor;
        }

        static void MakeLBP(GameObject go) {
            string lbpPath = LBPFolder + go.name + ".prefab";
            //PrefabUtility.CreatePrefab(lbpPath, go);
            GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(go, lbpPath, UnityEditor.InteractionMode.AutomatedAction);
        }
    }
}
