using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Rendering;

public class UselessShadow : Editor {
    //[MenuItem("Assets/Test")]
    public static void Test() {
        Texture2D t1 = Selection.objects[0] as Texture2D;
        Texture2D t2 = Selection.objects[1] as Texture2D;
        Debug.Log(CompareTwoTextures(t1, t2));

    }


    static Texture2D theBaseImage;
    static Texture2D theConditionImage;

    
    [MenuItem("Window/UselessShadow")]
    public static void GetUselessShadowCaster() {
        RenderTexture rt = AssetDatabase.LoadAssetAtPath<RenderTexture>("Assets/UselessShadow/Test.renderTexture");
        SceneView sceneView = SceneView.lastActiveSceneView;
        if(sceneView.camera != null) {
            theBaseImage = GetBaseImage(sceneView);

            MeshRenderer[] allRenders =  GameObject.FindObjectsOfType<MeshRenderer>();
            //Debug.Log(allRenders.Length);
            List<GameObject> finalSelection = new List<GameObject>();
            foreach(MeshRenderer mr in allRenders) {
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                theConditionImage = GetBaseImage(sceneView);
                float similarValue = CompareTwoTextures(theBaseImage, theConditionImage);
                if(similarValue == 1.0f) {
                    finalSelection.Add(mr.gameObject);
                    continue;
                } else {
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
            }
            Selection.objects = finalSelection.ToArray();
            // no need to save


        }
        
    }

    private static Texture2D GetBaseImage(SceneView sceneView) {
        sceneView.camera.targetTexture = new RenderTexture(sceneView.camera.activeTexture.width, sceneView.camera.activeTexture.height, 24, RenderTextureFormat.Default);
        sceneView.camera.Render();

        Texture2D texture = RtTo2D(sceneView.camera.targetTexture);

/*        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes("Assets/fff.png", bytes);*/
        return texture;
    }

    private static float CompareTwoTextures(Texture2D t1, Texture2D t2) {
        if(t1.width != t2.width || t1.height != t2.height) {
            Debug.Log("not the same size");
            return 0.0f;
        }
        Color[] colors_t1 = t1.GetPixels();
        Color[] colors_t2 = t2.GetPixels();

        int totalPixels = t1.width * t1.height;
        int diff = 0;
        for(int i = 0; i < colors_t1.Length; i++) {
            if(colors_t1[i] == colors_t2[i]) {
                diff++;
            }
        }
        return (float)diff / (float)totalPixels;

    }

    private static Texture2D RtTo2D(RenderTexture rt) {
        Texture2D texture2D = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

        RenderTexture.active = rt;
        
        texture2D.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0);
        texture2D.Apply();
        //RenderTexture.active = null;
        return texture2D;
    }
}
