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
        squareColor = new Color(0.7f, 0.7f, 0.7f);
    }

    private void OnGUI() {
        DrawSideFrame(GetSideFrameStyle());
        DrawTopFrame(GetSideFrameStyle());
        DrawMainFrame(GetSideFrameStyle());
        this.maxSize = new Vector2(984, 909);
        //this.position.width = 984;
        //this.position.height = 909;
        this.minSize = new Vector2(984, 909);
    }

    public void DropTextureGUI() {
        Event evt = Event.current;
        if(evt.type == EventType.MouseDrag) {
            Debug.Log("ddd");
        }

        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        switch(evt.type) {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if(!dropArea.Contains(evt.mousePosition)) {
                    Debug.Log("noooooooooooooooooo");
                    return;
                } else {
                    Debug.Log("yeeeeeeeeeeeeeeeeeeeah");
                }
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                break;
        }
    }

    int outputTextureSize = 1;
    Rect textureArea;
    Texture2D bgTexture;
    private void DrawMainFrame(GUIStyle style) {
        mainFrame.height = this.position.height - 180;
        mainFrame.width = this.position.width - 280;
        GUILayout.BeginArea(mainFrame, guiContent, style);
        GUI.Box(new Rect(5, 20, mainFrame.width - 10, mainFrame.height - 25), "");
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        //GUILayout.Label("Final Texture Size: ", GUILayout.ExpandWidth(true));
        GUILayout.Space(5);
        outputTextureSize = (int)GUILayout.HorizontalSlider((float)outputTextureSize, (int)1.0f, (int)4.0f, GUILayout.Width(300));
        //outputTextureSize = GetTextureSizeLevel(outputTextureSize);
        GUILayout.Label("Final Texture Size: " + GetTextureSizeLevel(outputTextureSize) + " x " + GetTextureSizeLevel(outputTextureSize), GUILayout.Width(200));
        if(GUILayout.Button("Combine", GUILayout.Width(170))) {

        }
        GUILayout.EndHorizontal();
        DropTextureGUI();

        //EditorGUI.DrawPreviewTexture(new Rect(20, 50, 700, 700), new Texture2D(10, 10));
        // = new Rect(mainFrame.position.x - 250, mainFrame.position.y - 125, 4069 / 6, 4096 / 6);
        //GUILayout.BeginArea(textureArea);
        //GUILayout.Box(new Texture2D((int)(textureArea.width - 10), (int)(textureArea.height - 10)));        
        /*        Vector2 bgTextureSize = new Vector2((textureArea.width - 10), (textureArea.height - 10));
                if(bgTexture == null || bgTexture.width != bgTextureSize.x) {
                    bgTexture = new Texture2D((int)bgTextureSize.x, (int)bgTextureSize.y);
                } else {
                    bgTexture = new Texture2D((int)bgTextureSize.x, (int)bgTextureSize.y);
                }
                GUILayout.Box(bgTexture);*/
        /*
        Vector2 bgTextureSize = new Vector2((textureArea.width - 10), (textureArea.height - 10));
        if(bgTexture == null || bgTexture.width != bgTextureSize.x) {
            bgTexture = new Texture2D((int)bgTextureSize.x, (int)bgTextureSize.y);
        }
        */
        //bgTexture.SetPixels(GetCrossPixels(bgTextureSize));
        //bgTexture.wrapMode = TextureWrapMode.Repeat;
        //bgTexture.Apply();
        //GUILayout.Box(bgTexture);
        //GUI.Label(new Rect(0, 0, 50, 50), "fffffffffffffffffffffffffffffffffffffffffff");
        DrawRectGrid(16);
        //GUILayout.EndArea();
        GUILayout.EndArea();
        
    }

    Color squareColor;
    private void DrawRectGrid(int number) {

        //int count = size / 256;
        int count = number;

        //Debug.Log(count);
        int xFix = 17;
        int yFix = 55;
        int squareSize = 40;    // 80
        int squareborder = 2;
        for(int i = 0; i < count; i++) {
            EditorGUI.DrawRect(new Rect((squareSize + squareborder) * i + xFix , 0 + yFix, squareSize, squareSize), squareColor);
            for(int j = 1; j < count; j++) {
                EditorGUI.DrawRect(new Rect((squareSize + squareborder) * i + xFix, j * (squareSize + squareborder) + yFix, squareSize, squareSize), squareColor);
            }
        }
      
    }

    private int GetTextureSizeLevel(int number) {
        switch(number) {
            case 1:
                return 512;
            case 2:
                return 1024;
            case 3:
                return 2048;
            case 4:
                return 4096;
            default:
                return 512;

        }
    }

    Vector2 pos = new Vector2(10, 10);
    List<TexItem> texItems;
    private void DrawSideFrameList() {
        texItems = TexItemList.GetTexItems();
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
        if(tempMaterial.GetTexturePropertyNameIDs().Length >= 1) {
            DrawItemTextures(texItems);
        }
        GUILayout.EndArea();
    }

    Vector2 pos2 = new Vector2(10, 10);
    private void DrawItemTextures(List<TexItem> texItems) {
        if(texItems == null || texItems.Count < 1) {
            return;
        }
        GUILayout.BeginArea(new Rect(5, 60, topFrame.width - 12, topFrame.height - 65));
        using(var scrollView2 = new GUILayout.ScrollViewScope(this.pos2)) {
            this.pos2 = scrollView2.scrollPosition;
            GUILayout.BeginHorizontal();
            int offsetX = 5;
            GUILayoutOption[] layouts = new GUILayoutOption[] {
                GUILayout.Width(70),
                GUILayout.Height(70)
            };
            foreach(TexItem ti in texItems) {
                if(ti.IsSelected()) {
                    if(ti.ContainsTextures()) {
                        if(textureNames.Length < 1) {
                            return;
                        }
                        Texture2D requiredTexture = ti.GetTheTexture(tempMaterial, textureNames[selectedTexChannelIndex]);
                        if(requiredTexture != null) {
                            //GUI.Box(new Rect(offsetX, 5, 50, 50), "tt");

                            //GUI.DrawTexture(new Rect(0, 0, 50, 50), requiredTexture);
                            //EditorGUI.DrawPreviewTexture(new Rect(offsetX, 0, 80, 80), requiredTexture);
                            if(GUILayout.Button(requiredTexture, layouts)) {
                             
                            }
                            //GUILayout.Box(requiredTexture);
                        } else {
                            //GUILayout.Box(ti.GetName() + "  has no the " + textureNames[selectedTexChannelIndex] + " texture");
                        }

                    } else {
                        //GUILayout.Box(ti.GetName() + "  has no textures");
                    }
                }
                offsetX += 50;
            }
            GUILayout.EndHorizontal();

        }
        GUILayout.EndArea();
    }

    Material tempMaterial;
    int selectedTexChannelIndex;
    string[] textureNames;
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
        //string[] textureNames = mat.GetTexturePropertyNames();
        textureNames = mat.GetTexturePropertyNames();
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
