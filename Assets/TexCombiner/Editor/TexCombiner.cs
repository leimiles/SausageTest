using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TexCombiner : EditorWindow {

    Rect sideFrame;
    Rect topFrame;
    Rect mainFrame;
    GUIContent guiContent;


    [MenuItem("Window/Tex Combiner")]
    public static void ShowWindow() {
        TexCombiner tc = (TexCombiner)EditorWindow.GetWindow(typeof(TexCombiner));
        tc.Show();
        tc.Init();  
    }

    public void Init() {
        guiContent = new GUIContent("Scene Objects:");
        sideFrame = new Rect(10, 10, 200, 500);
        topFrame = new Rect(220, 10, 500, 150);
        mainFrame = new Rect(220, 170, 500, 500);
    }

    private void OnGUI() {
        DrawSideFrame(GetSideFrameStyle());
        DrawTopFrame(GetSideFrameStyle());
        DrawMainFrame(GetSideFrameStyle());
    }

    private void DrawMainFrame(GUIStyle style) {
        mainFrame.height = this.position.height - 180;
        mainFrame.width = this.position.width - 230;
        GUILayout.BeginArea(mainFrame, guiContent, style);
        GUILayout.EndArea();
    }

    private GUIStyle GetSideFrameStyle() {
        GUIStyle style = GUI.skin.GetStyle("Window");
        return style;
    }

    private void DrawTopFrame(GUIStyle style) {
        topFrame.width = this.position.width - 230;
        GUILayout.BeginArea(topFrame, guiContent, style);
        GUILayout.EndArea();
    }

    private void DrawSideFrame(GUIStyle style) {
        sideFrame.height = this.position.height - 20;
        GUILayout.BeginArea(sideFrame, guiContent, style);
        if(GUILayout.Button("Load Objects")) {

        }
        GUILayout.EndArea();
    }
}
