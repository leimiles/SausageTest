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
        guiContent = new GUIContent("");
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
        GUI.Box(new Rect(5, 20, mainFrame.width - 10, mainFrame.height - 25), "");
        GUILayout.EndArea();
    }

    Vector2 pos = new Vector2(10, 10);
    private void DrawSideFrameList() {
        List<TexItem> texItems = TexItemList.GetTexItems();
        if(texItems == null || texItems.Count < 1) {
            GUILayout.Label("Nothing to show.");
            return;
        }

        using(var scrollView =  new GUILayout.ScrollViewScope(this.pos)) {
            this.pos = scrollView.scrollPosition;
            foreach(TexItem ti in texItems) {
                GUILayout.BeginHorizontal();
                ti.Draw();
                GUILayout.EndHorizontal();
            }
        }
    }

    private GUIStyle GetSideFrameStyle() {
        GUIStyle style = GUI.skin.GetStyle("Window");
        return style;
    }

    Shader shader;
    private void DrawTopFrame(GUIStyle style) {
        topFrame.width = this.position.width - 230;
        GUILayout.BeginArea(topFrame, guiContent, style);
        GUI.Box(new Rect(5, 20, topFrame.width - 10, topFrame.height - 25), "");
        shader = (Shader)EditorGUILayout.ObjectField(shader, typeof(Shader), true);
        GUILayout.EndArea();
    }

    private void DrawSideFrame(GUIStyle style) {
        sideFrame.height = this.position.height - 20;
        GUILayout.BeginArea(sideFrame, guiContent, style);
        GUI.Box(new Rect(5, 40, 190, sideFrame.height - 45), "");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Load")) {
            TexItemList.SetData();
        }
        if(GUILayout.Button("Add")) {
            TexItemList.AddData();
        }
        if(GUILayout.Button("Remove")) {
            TexItemList.RemoveFromList(1);
        }
        GUILayout.EndHorizontal();
        DrawSideFrameList();
        GUILayout.EndArea();
    }
}
