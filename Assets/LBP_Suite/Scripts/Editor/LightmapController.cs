using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LightmapController : EditorWindow{
    private float rangeMax = 50f;
    private float rangeMin = 10f;
    private float scale = 1f;
    private bool isSearchable = false;
    [MenuItem("Lightmap/ScaleController")]
    public static void ShowWindow() {
        GetWindow<LightmapController>("ScaleController");
    }

    private void OnGUI() {
        GUILayout.Label("根据体积调整 LightmapScale 参数: ", EditorStyles.boldLabel);
        GUILayout.Space(2);
        //GUILayout.BeginHorizontal();
        
        GUILayout.Label("先设置一个范围，单位(米), 从", EditorStyles.boldLabel);
  
        rangeMin = EditorGUILayout.FloatField("最小", rangeMin);
       
        GUILayout.Label("到", EditorStyles.boldLabel);
        rangeMax = EditorGUILayout.FloatField("最大", rangeMax);

        //GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("LightmapScale，默认为 1", EditorStyles.boldLabel);

        scale = EditorGUILayout.FloatField("", scale);

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("重置")) {
            rangeMax = 50f;
            rangeMin = 10f;
            scale = 1f;
        }
        
        if (GUILayout.Button("获取")) {
            GetDesiredObjs(rangeMin, rangeMax);
        }
        if (GUILayout.Button("设置")) {
            if(Selection.objects.Length < 1) {
                Debug.Log("没有");
            
            }else {
                SetDesiredScale(scale);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.Label("-------------------------------------------------");
        GUILayout.Label("选中一个物体，获得与其体积相同的所有物体: ", EditorStyles.boldLabel);
        
        if (GUILayout.Button("搜索") && isSearchable == true) {
            GetBySize(CurrentObj());
        }
        GUILayout.Space(3);
        GUILayout.Label("当前物体: ");
        CurrentObj();
        //GUILayout.Label(Selection.activeGameObject.name);
        
    }

    void GetBySize(GameObject obj) {
        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        if(mr != null) {
            float temp = Mathf.Max(mr.bounds.size.x, mr.bounds.size.y);
            temp = Mathf.Max(mr.bounds.size.z, temp);
            GetDesiredObjs(temp, temp);
        } else {
            Debug.Log("并非可渲染对象");
        }
    }

    GameObject CurrentObj() {
        GameObject obj = null;
        if(Selection.activeGameObject != null) {
            if(Selection.gameObjects.Length > 1) {
                GUILayout.Label("多个");
                isSearchable = false;
            }else {
                obj = Selection.activeGameObject;
                GUILayout.Label(obj.name);
                isSearchable = true;
            }
        }else {
            GUILayout.Label("没有");
            isSearchable = false;
        }
        return obj;
    }


    void SetDesiredScale(float scale) {
        if(scale > 1f) {
            scale = 1f;
        }
        Debug.Log(Selection.gameObjects.Length);
        foreach(GameObject go in Selection.gameObjects) {
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            SerializedObject so = new SerializedObject(mr);
            so.FindProperty("m_ScaleInLightmap").floatValue = scale;
            so.ApplyModifiedProperties();
            Debug.Log(so.FindProperty("m_ScaleInLightmap").floatValue);
        }
    }

    void GetDesiredObjs(float rangeMin, float rangeMax) {
        MeshRenderer[] obj;
        int count = 0;
        List<GameObject> gameObjs = new List<GameObject>();
        obj = GameObject.FindObjectsOfType<MeshRenderer>();
        foreach(MeshRenderer mr in obj) {
            //Debug.Log(mr.bounds.size);
            float temp = Mathf.Max(mr.bounds.size.x, mr.bounds.size.y);
            temp = Mathf.Max(mr.bounds.size.z, temp);
            if(temp >= rangeMin && temp <= rangeMax) {
                //Debug.Log(mr.name);
                if(mr.transform.gameObject.isStatic) {
                    count++;
                    gameObjs.Add(mr.transform.gameObject);
                }
                
            }
        }
        Selection.objects = gameObjs.ToArray();
        Debug.Log("找到了 " + count + " 个范围在 " + rangeMin + " 到 " + rangeMax + " 之间的静态物体");
    }
}