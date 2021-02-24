using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TexCombiner : EditorWindow {

    Rect sideFrame;
    Rect topFrame;
    Rect mainFrame;
    GUIContent guiContent;
    int selectedShaderIndex;
    ShaderInfo[] shaderInfos;


    [MenuItem("Window/Tex Combiner")]
    public static void ShowWindow() {
        TexCombiner tc = (TexCombiner)EditorWindow.GetWindow(typeof(TexCombiner));
        tc.Show();
        tc.Init();
    }

    public void Init() {
        guiContent = new GUIContent("");
        sideFrame = new Rect(10, 10, 250, 500);
        topFrame = new Rect(sideFrame.width + 20, 10, 500, 150);
        mainFrame = new Rect(sideFrame.width + 20, 170, 500, 500);
        shaderInfos = ShaderUtil.GetAllShaderInfo();
    }

    private void OnGUI() {
        DrawSideFrame(GetSideFrameStyle());
        DrawTopFrame(GetSideFrameStyle());
        DrawMainFrame(GetSideFrameStyle());
    }

    private void DrawMainFrame(GUIStyle style) {
        mainFrame.height = this.position.height - 180;
        mainFrame.width = this.position.width - 280;
        GUILayout.BeginArea(mainFrame, guiContent, style);
        GUI.Box(new Rect(5, 20, mainFrame.width - 10, mainFrame.height - 25), "");
        GUILayout.EndArea();
    }

    Vector2 pos = new Vector2(10, 10);
    private void DrawSideFrameList() {
        List<TexItem> texItems = TexItemList.GetTexItems();
        if(texItems == null || texItems.Count < 1) {
            //GUILayout.Label("Nothing to show.");
            return;
        }
        GUILayout.Space(7);

        using(var scrollView = new GUILayout.ScrollViewScope(this.pos)) {
            this.pos = scrollView.scrollPosition;
            foreach(TexItem ti in texItems) {
                ti.Draw(shaderInfos[selectedShaderIndex].name);
               
            }
        }
    }

    private GUIStyle GetSideFrameStyle() {
        GUIStyle style = GUI.skin.GetStyle("Window");
        return style;
    }



    private void DrawTopFrame(GUIStyle style) {
        topFrame.width = this.position.width - 280;
        GUILayout.BeginArea(topFrame, guiContent, style);
        GUI.Box(new Rect(5, 20, topFrame.width - 10, topFrame.height - 25), "");
        //shader = (Shader)EditorGUILayout.ObjectField(shader, typeof(Shader), true);
        GUIContent[] shaderGuiContents;
        if(shaderInfos.Length > 1) {
            shaderGuiContents = GetShaderGuiContent(shaderInfos);
            selectedShaderIndex = EditorGUILayout.Popup(selectedShaderIndex, shaderGuiContents);
        }
        DrawTexTemplate(selectedShaderIndex);
        GUILayout.EndArea();
    }

    Material tempMaterial;
    int selectedTexChannelIndex;
    private void DrawTexTemplate(int index) {
        //Debug.Log(shaderInfos[index].name);
        Shader shader = Shader.Find(shaderInfos[index].name);
        if(shader == null) {
            Debug.Log("shader is not great.");
            return;
        }
        tempMaterial = new Material(shader);
        
        GUIContent[] textureGuis = GetTexturesGUI(tempMaterial);
        if(textureGuis.Length < 1) {
            return;
        }
        //GUILayout.Label("--------------------------------------------");
        Rect rect = new Rect(5, 40, topFrame.width - 11, 20);
        GUILayout.BeginArea(rect, "");
        selectedTexChannelIndex = GUILayout.SelectionGrid(selectedTexChannelIndex, textureGuis, textureGuis.Length, GUILayout.ExpandWidth(true));
        GUILayout.EndArea();

    }

    public GUIContent[] GetTexturesGUI(Material mat) {
        string[] textureNames = mat.GetTexturePropertyNames();
        GUIContent[] contents = new GUIContent[textureNames.Length];
        if(textureNames.Length >= 1) {
            for(int i = 0; i < textureNames.Length; i++) {
                contents[i] = new GUIContent(textureNames[i]);
            }
        }
        return contents;

    }

    private GUIContent[] GetShaderGuiContent(ShaderInfo[] shaderInfos) {
        GUIContent[] gUIContents = new GUIContent[shaderInfos.Length];
        for(int i = 0; i < shaderInfos.Length; i++) {
            gUIContents[i] = new GUIContent(shaderInfos[i].name);
        }
        return gUIContents;
    }

    private void DrawSideFrame(GUIStyle style) {
        sideFrame.height = this.position.height - 20;
        GUILayout.BeginArea(sideFrame, guiContent, style);
        GUI.Box(new Rect(5, 45, sideFrame.width - 10, sideFrame.height - 50), "");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Load")) {
            TexItemList.SetData();
        }
        if(GUILayout.Button("Add")) {
            TexItemList.AddData();
        }
        if(GUILayout.Button("Remove")) {
            TexItemList.RemoveFromList();
        }
        GUILayout.EndHorizontal();
        DrawSideFrameList();
        GUILayout.EndArea();
    }
}
