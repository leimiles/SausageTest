using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace MileCode {
    public class LBP_DeepCombine : Editor {
        static string LBDPFolder = "Assets/LBP_Suite/Prefabs/LBDP/";
        static string LBDPMeshesFolder = "Assets/LBP_Suite/Meshes/";
        static string LBDPMaterialsFolder = "Assets/LBP_Suite/Materials/LBDP_Materials/";
        static Shader LBDPShader;

        [MenuItem("Lightmap/LBP/Generate Deep LBP")]
        public static void DeepCombine() {
            if(!CheckedContext()) {
                return;
            }
            FetchGameObjects();
        }

        static void FetchGameObjects() {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("LBP Deep");
            if(gameObjects.Length <= 0) {
                Debug.Log("Can't find gameObjects with the tag(LBP)");
                return;
            } else {

                Debug.LogWarning(gameObjects.Length + " LBP Tags.");
            }
            FindPotentialLBDP(gameObjects);
        }

        static void FindPotentialLBDP(GameObject[] gameObjects) {
            foreach(GameObject go in gameObjects) {
                MeshRenderer[] childrenRenders = go.GetComponentsInChildren<MeshRenderer>();
                GenerateLBDPMesh(childrenRenders);    
            }
        }

        static void GenerateLBDPMesh(MeshRenderer[] meshRenderers) {
            for(int i = 0; i < LightmapSettings.lightmaps.Length; i++) {
                foreach(MeshRenderer mr in meshRenderers) {
                    if(mr.lightmapIndex == i) {
                        //Debug.Log("Index: " + i);
                        //Debug.Log("Name: " + mr.gameObject.transform.parent == null ? mr.gameObject.name : mr.gameObject.transform.parent.name);
                        string prefabName;
                        if(mr.gameObject.transform.parent == null) {
                            prefabName = mr.gameObject.name;
                        } else {
                            prefabName = mr.gameObject.transform.parent.name;
                        }
                        Debug.Log(prefabName);
                        Mesh lbdpMesh = CopyMeshWithNewUV2FromOldMesh(mr.gameObject.name, mr.GetComponent<MeshFilter>().sharedMesh, mr.lightmapScaleOffset);
                        string savePath = LBDPMeshesFolder + lbdpMesh.name + ".asset";
                        AssetDatabase.CreateAsset(lbdpMesh, savePath);
                        PrepareLBDPMaterialForLightmap(EditorSceneManager.GetActiveScene().name + "_lightmap_" + i , LightmapSettings.lightmaps[i].lightmapColor);

                        //AssetDatabase.CreateAsset(new GameObject(prefabName),  LBDPFolder + prefabName + ".prefab");
                        //PrefabUtility.SaveAsPrefabAsset( (LBDPFolder + prefabName + ".prefab");
                    }
                }
            }
        }

        static Material PrepareLBDPMaterialForLightmap(string materialName, Texture2D lightmap) {
            Material material = new Material(LBDPShader);
            material.SetTexture("_Lightmap", lightmap);
            if(material != null) {
                AssetDatabase.CreateAsset(material, LBDPMaterialsFolder + materialName + ".mat");
            }
            return material;
        }



        static Mesh CopyMeshWithNewUV2FromOldMesh(string meshName, Mesh oldMesh, Vector4 scaleOffset) {
            Vector3[] vertices = oldMesh.vertices;
            int[] tris = oldMesh.triangles;
            Vector3[] normals = oldMesh.normals;
            Vector2[] uv1 = oldMesh.uv;
            Vector2[] uv2 = GetNewUV2(oldMesh.uv2, scaleOffset);
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = tris;
            mesh.normals = normals;
            mesh.uv = uv1;
            mesh.uv2 = uv2;
            mesh.name = meshName;
            return mesh;
        }


        static Vector2[] GetNewUV2(Vector2[] oldUV2, Vector4 scaleOffset) {
            Matrix4x4 uvMatrix = new Matrix4x4(
                    new Vector4(scaleOffset.x, 0, 0, 0),
                    new Vector4(0, scaleOffset.y, 0, 0),
                    new Vector4(0, 0, 1, 0),
                    new Vector4(scaleOffset.z, scaleOffset.w, 0, 1)
                );
            Vector2[] newUV2 = new Vector2[oldUV2.Length];
            for(int i = 0; i < newUV2.Length; i++) {
                newUV2[i].x = uvMatrix.MultiplyPoint(new Vector3(oldUV2[i].x, oldUV2[i].y, 0)).x;
                newUV2[i].y = uvMatrix.MultiplyPoint(new Vector3(oldUV2[i].x, oldUV2[i].y, 0)).y;
            }
            return newUV2;
        }

        private static bool CheckedContext() {
            bool isChecked = true;
            LBDPShader = Shader.Find("MileShader/LightBakedDeepPrefabIntensity");
            if(LBDPShader == null) {
                isChecked = false;
                Debug.Log("LBDP Shader can't be found!");
            }
            return isChecked;
        }

    }
}
