using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TexCombinerWindow : EditorWindow {

    static GUIContent titleGui = new GUIContent("Texture Combine");
    string str = "test";
    bool groupGo;
    bool mybool = true;
    float myFloat = 12.3f;

    [MenuItem("Window/Tex Combiner")]
    static void Init() {
        TexCombinerWindow tcWindow = (TexCombinerWindow)EditorWindow.GetWindow(typeof(TexCombinerWindow));
        tcWindow.titleContent = titleGui;
        tcWindow.Show();
    }


    void OnGUI() {
        GUILayout.Label("Settings: ", EditorStyles.boldLabel);
        str = EditorGUILayout.TextField("Input Texture Name", GUILayout.Width(130.0f)) ;
        groupGo = EditorGUILayout.BeginToggleGroup("Optional Settings: ", groupGo);
        mybool = EditorGUILayout.Toggle("Toggle", mybool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3.0f, 15.0f);
        EditorGUILayout.EndToggleGroup();
        GUILayout.BeginArea(new Rect(0, 300, 50, 50), new GUIContent("boxStyle"));
    }
}
