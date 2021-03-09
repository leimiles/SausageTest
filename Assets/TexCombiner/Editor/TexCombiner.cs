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

    private void Update() {
        Repaint();
    }

    private void OnEnable() {
        //Debug.Log("...tttxxx");
        //GUIDraggableObject yo = new GUIDraggableObject(new Vector2(0, 0));

    }

    public void Init() {
        guiContent = new GUIContent("");
        sideFrame = new Rect(10, 10, 250, 500);
        topFrame = new Rect(sideFrame.width + 20, 10, 500, 150);
        mainFrame = new Rect(sideFrame.width + 20, 170, 500, 500);
        shaderInfos = ShaderUtil.GetAllShaderInfo();
        squareColor = new Color(0.75f, 0.75f, 0.75f);
        mainFramePositionCorrection = new Vector2(10, 55);
        rectButtonLeft = new Rect(0, 0, 10, 20);
        rectButtonRight = new Rect(0, 0, 10, 20);
        rectButtonTop = new Rect(0, 0, 20, 10);
        rectButtonButtom = new Rect(0, 0, 20, 10);
        rectRepeatButton = new Rect(0, 0, 50, 25);
        texItemScaleInCanvas = Mathf.Pow(2, 0);
        gridCanvas = new Rect(10, 55, 512, 512);
        toggleRect = new Rect(0, 0, 200, 200);
    }

    private void OnGUI() {
        DrawSideFrame(GetSideFrameStyle());
        DrawTopFrame(GetSideFrameStyle());
        DrawMainFrame(GetSideFrameStyle());
        this.maxSize = new Vector2(812, 750);
        //Debug.Log(this.position.width);
        //Debug.Log(this.position.height);
        this.minSize = new Vector2(812, 750);
        //DropTextureGUI();
    }
    /*
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
    */

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
        outputTextureSize = (int)GUILayout.HorizontalSlider((float)outputTextureSize, (int)1.0f, (int)4.0f, GUILayout.Width(130));
        //outputTextureSize = GetTextureSizeLevel(outputTextureSize);
        GUILayout.Label("Final Texture Size: " + GetTextureSizeLevel(outputTextureSize) + " x " + GetTextureSizeLevel(outputTextureSize), GUILayout.Width(200));
        if(GUILayout.Button("Combine", GUILayout.Width(170))) {
            // Combine only one texture
            TexItemList.CombineTheTexItem(GetTextureSizeLevel(outputTextureSize), gridCanvas, textureNames[selectedTexChannelIndex]);
            SetOutput(TexItemList._combinerResults);
            

        }
        GUILayout.EndHorizontal();

        //DrawRectGrid(16);
        DrawRectGridCanvas(gridCanvas);
        DrawTextureBlock();

        GUILayout.EndArea();

    }

    
    private void SetOutput(CombinerResults combinerResults) {
        if(combinerResults != null && combinerResults.IsReady()) {
            GameObject outputRoot = GameObject.Find("TexCombiner_output");
            if(outputRoot == null) {
                outputRoot = new GameObject("TexCombiner_output");
            } else {
                GameObject.DestroyImmediate(outputRoot);
                outputRoot = new GameObject("TexCombiner_output");
            }
            foreach(TexItem ti in combinerResults.combinedItems) {
                GameObject newItem = new GameObject(ti.GetObjName());
                newItem.AddComponent<MeshRenderer>().sharedMaterial = combinerResults.GetMaterial();
                newItem.transform.position = ti.GetTransform().position;
                newItem.transform.rotation = ti.GetTransform().rotation;
                newItem.transform.localScale = ti.GetTransform().localScale;
                newItem.AddComponent<MeshFilter>().sharedMesh = ti.GetNewBorn();
                newItem.transform.SetParent(outputRoot.transform);

                //newItem.transform = ti.GetTransform();
            }
        }
    }
    

    Rect gridCanvas;
    private void DrawRectGridCanvas(Rect gridCanvas) {
        EditorGUI.DrawRect(gridCanvas, squareColor);
        DrawGridBlock();
    }

    private void DrawGridBlock() {

    }


    public void DrawTextureBlock() {
        if(texItems == null) {
            return;
        }
        //Debug.Log(texItems.Count);
        foreach(TexItem ti in texItems) {
            if(ti.isPressed && ti.IsSelected() && ti.theTexture != null) {
                

                Rect rect = new Rect(ti.position + mainFramePositionCorrection, ti.SetSizeForCanvas(GetTextureSizeLevel(outputTextureSize), gridCanvas, texItemScaleInCanvas));
                //Debug.Log(ti.GetName());
                //ti.SetPositionAndSize(GetTextureSizeLevel(outputTextureSize));
                //EditorGUI.DrawPreviewTexture(rect, new Texture2D(30, 30));
                //GUILayout.BeginArea(rect, ti.theTexture);
                GUILayout.BeginArea(rect, "");
                GUI.DrawTexture(new Rect(0, 0, rect.width, rect.height), ti.theTexture);
                //DrawMoveButtons(ti);
                DrawMoveButtonForKeyboard(ti);
                //DrawScaleButton(ti);
                GUILayout.EndArea();
            }
        }
        //Debug.Log(texItems.Count);
    }

    

    Rect toggleRect;
    private void DrawMoveButtonForKeyboard(TexItem ti) {
        ti.isMoving = EditorGUI.Toggle(toggleRect, ti.isMoving);
        if(ti.isMoving) {
            TexItemList.SetMovingFalseExcept(ti);
            //Debug.Log(EventType.ContextClick.ToString());
            //float timeCount = 0;
            if(Event.current.keyCode == KeyCode.RightArrow && Event.current.type == EventType.KeyDown) {
                if(ti.position.x < (gridCanvas.width - ti.sizeForCanvas.x)) {
                    ti.position.x += 32;
                }
                if(ti.position.x >= (gridCanvas.width - ti.sizeForCanvas.x)) {
                    ti.position.x = (gridCanvas.width - ti.sizeForCanvas.x);
                }
                ti.cph.position.x = ti.position.x;
            }

            if(Event.current.keyCode == KeyCode.LeftArrow && Event.current.type == EventType.KeyDown) {
                if(ti.position.x > 0) {
                    ti.position.x -= 32;
                }
                if(ti.position.x <= 0) {
                    ti.position.x = 0;
                }
                ti.cph.position.x = ti.position.x;
            }

            if(Event.current.keyCode == KeyCode.UpArrow && Event.current.type == EventType.KeyDown) {
                if(ti.position.y > 0) {
                    ti.position.y -= 32;
                }
                if(ti.position.y <= 0) {
                    ti.position.y = 0;
                }
                ti.cph.position.y = ti.position.y;
            }

            if(Event.current.keyCode == KeyCode.DownArrow && Event.current.type == EventType.KeyDown) {
                if(ti.position.y < (gridCanvas.height - ti.sizeForCanvas.y)) {
                    ti.position.y += 32;
                }
                if(ti.position.y >= (gridCanvas.height - ti.sizeForCanvas.y)) {
                    ti.position.y = (gridCanvas.height - ti.sizeForCanvas.y);
                }
                ti.cph.position.y = ti.position.y;
            }

        }
    }



    Rect rectButtonLeft;
    Rect rectButtonRight;
    Rect rectButtonTop;
    Rect rectButtonButtom;
    private void DrawMoveButtons(TexItem ti) {
        rectButtonLeft.y = (int)((ti.sizeForCanvas.y / 2) - (rectButtonLeft.height / 2));
        if(GUI.Button(rectButtonLeft, "")) {
            if(ti.position.x > 0) {
                ti.position.x -= 32;
            }
            if(ti.position.x <= 0) {
                ti.position.x = 0;
            }
            ti.cph.position.x = ti.position.x;
        }

        rectButtonRight.x = (int)(ti.sizeForCanvas.x - rectButtonRight.width);
        rectButtonRight.y = (int)((ti.sizeForCanvas.y / 2) - (rectButtonRight.height / 2));
        if(GUI.Button(rectButtonRight, "")) {
            if(ti.position.x < (gridCanvas.width - ti.sizeForCanvas.x)) {
                ti.position.x += 32;
            }
            if(ti.position.x >= (gridCanvas.width - ti.sizeForCanvas.x)) {
                ti.position.x = (gridCanvas.width - ti.sizeForCanvas.x);
            }
            ti.cph.position.x = ti.position.x;
        }

        rectButtonTop.x = (int)((ti.sizeForCanvas.x / 2) - (rectButtonTop.width / 2));
        if(GUI.Button(rectButtonTop, "")) {
            if(ti.position.y > 0) {
                ti.position.y -= 32;
            }
            if(ti.position.y <= 0) {
                ti.position.y = 0;
            }
            ti.cph.position.y = ti.position.y;
        }

        rectButtonButtom.x = (int)((ti.sizeForCanvas.x / 2) - (rectButtonButtom.width / 2));
        rectButtonButtom.y = (int)((ti.sizeForCanvas.y - rectButtonButtom.height));
        if(GUI.Button(rectButtonButtom, "")) {
            if(ti.position.y < (gridCanvas.height - ti.sizeForCanvas.y)) {
                ti.position.y += 32;
            }
            if(ti.position.y >= (gridCanvas.height - ti.sizeForCanvas.y)) {
                ti.position.y = (gridCanvas.height - ti.sizeForCanvas.y);
            }
            ti.cph.position.y = ti.position.y;
        }
    }

    Rect rectRepeatButton;
    float texItemScaleInCanvas;

    private void DrawScaleButton(TexItem ti) {
        rectRepeatButton.x = (int)((ti.sizeForCanvas.x / 2) - (rectRepeatButton.width / 2));
        rectRepeatButton.y = (int)((ti.sizeForCanvas.y / 2) + (rectRepeatButton.height / 2));
        /*
        if(GUI.RepeatButton(rectRepeatButton, ti.theTexture.width.ToString())) {
            // change texItem scale

            //ti.sizeForCanvas *= outputTextureSize;
        }
        */
    }


    Vector2 mainFramePositionCorrection;
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
            EditorGUI.DrawRect(new Rect((squareSize + squareborder) * i + xFix, 0 + yFix, squareSize, squareSize), squareColor);
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

            GUILayoutOption[] layouts = new GUILayoutOption[] {
                GUILayout.Width(70),
                GUILayout.Height(70),
            };
            int offset = 0;
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
                            //EditorGUI.DrawPreviewTexture(new Rect(offsetX, 0, 75, 75), requiredTexture);
                            if(ti.isPressed) {
                                EditorGUI.DrawRect(new Rect(6 + offset, 76, 65, 6), Color.green);
                            } else {
                                EditorGUI.DrawRect(new Rect(6 + offset, 76, 65, 6), Color.red);
                            }
                            if(GUILayout.Button(requiredTexture, layouts)) {
                                //ProjectWindowUtil.ShowCreatedAsset(requiredTexture);
                                //EditorGUIUtility.AddCursorRect(new Rect(0, 0, 40, 40), MouseCursor.MoveArrow);
                                if(ti.isPressed) {
                                    ti.isPressed = false;
                                } else {
                                    if(!requiredTexture.isReadable) {
                                        Debug.Log(requiredTexture.name + " is not readable");
                                        ti.isPressed = false;
                                    } else {

                                        ti.isPressed = true;
                                    }

                                }
                                //Debug.Log(ti.GetTheTextureName());
                            }
                            offset += 75;
                            //GUILayout.Box(requiredTexture);
                        } else {
                            //GUILayout.Box(ti.GetName() + "  has no the " + textureNames[selectedTexChannelIndex] + " texture");
                        }

                    } else {
                        //GUILayout.Box(ti.GetName() + "  has no textures");
                    }
                }
                //offset += 70;
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
