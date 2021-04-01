using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TexItemList {
    private static List<TexItem> _texItems;
    public static CombinerResults _combinerResults;
    public static List<TexItem> GetTexItems() {
        return _texItems;
    }

    public static void SetMovingFalseExcept(TexItem ti) {
        if(_texItems == null) {
            return;
        }
        foreach(TexItem temp in _texItems) {
            if(temp != ti) {
                temp.isMoving = false;
            }
        }

    }

    public static string _finalTextureSavePath = "";
    public static string _finalMaterialSavePath = "";
    public static string _finalMeshSavePath = "";


    public static string GetFinalMeshSavePath() {
        return _finalMeshSavePath;
    }
    public static void SetFinalMeshSavePath(string value) {
        int index = value.IndexOf("Assets/");
        if(index >= 0) {
            _finalMeshSavePath = value.Substring(index);
        }

        //Debug.Log(_finalMeshSavePath);
    }

    public static string GetFinalTextureSavePath() {
        return _finalTextureSavePath;
    }
    public static void SetFinalTextureSavePath(string value) {
        int index = value.IndexOf("Assets/");
        if(index >= 0) {
            _finalTextureSavePath = value.Substring(index);
        }
    }

    public static string GetFinalMaterialSavePath() {
        return _finalMaterialSavePath;
    }
    public static void SetFinalMaterialSavePath(string value) {
        int index = value.IndexOf("Assets/");
        if(index >= 0) {
            _finalMaterialSavePath = value.Substring(index);
        }
    }


    public static void CombineTheTexItem(int finalTextureSize, Rect canvasSize, string textureChannelName) {
        if(_texItems == null) {
            return;
        }

        _combinerResults = new CombinerResults();
        // generate combined texture
        Texture2D combinedTexture = new Texture2D(finalTextureSize, finalTextureSize, TextureFormat.RGBA32, true);
        List<TexItem> toBeCombinedTexItems = new List<TexItem>();
        for(int i = 0; i < _texItems.Count; i++) {
            if(_texItems[i].theTexture != null && _texItems[i].isPressed && _texItems[i].IsSelected()) {
                //Debug.Log(_texItems[i].theTexture.name);
                Vector2 finalPos = _texItems[i].GetPositionForFinalTexture(finalTextureSize, canvasSize);
                finalPos = TransformFromCanvasSpaceToTextureSpace(finalPos, finalTextureSize, canvasSize);
                finalPos.y -= (int)_texItems[i].theTexture.height;
                _texItems[i].SetUVOffset(finalPos);
                //Debug.Log(finalPos);
                combinedTexture.SetPixels((int)finalPos.x, (int)finalPos.y, (int)_texItems[i].theTexture.width, (int)_texItems[i].theTexture.height, _texItems[i].theTexture.GetPixels());
                toBeCombinedTexItems.Add(_texItems[i]);
            }
        }

        combinedTexture.Apply();
        byte[] bytes = combinedTexture.EncodeToPNG();
        /*
        string combinedTexturePath = EditorUtility.SaveFilePanel("选择贴图保存目录", "Assets", "combinedTexture", "png");
        int index = combinedTexturePath.IndexOf("Assets/");
        if(index >= 0) {
            combinedTexturePath = combinedTexturePath.Substring(index);
            //Debug.Log(combinedTexturePath);
            File.WriteAllBytes(combinedTexturePath, bytes);
        }
        */

        if(_finalMaterialSavePath == "" || !_finalMaterialSavePath.EndsWith(".mat")) {
            EditorUtility.DisplayDialog("警告", "非法的材质保存目录", "Try Again");
            return;
        } else {
            //Debug.Log(_finalMaterialSavePath);
        }

        if(_finalTextureSavePath == "" || !_finalTextureSavePath.EndsWith(".png")) {
            EditorUtility.DisplayDialog("警告", "非法的贴图保存目录", "Try Again");
            return;
        } else {
            //Debug.Log(_finalTextureSavePath);
        }

        if(_finalMeshSavePath.EndsWith("/")) {
            int lastIndex = _finalMeshSavePath.LastIndexOf('/');
            _finalMeshSavePath = _finalMeshSavePath.Remove(lastIndex);
        }

        if(_finalMeshSavePath == "" || !AssetDatabase.IsValidFolder(_finalMeshSavePath)) {
            EditorUtility.DisplayDialog("警告", "非法的网格保存目录", "Try Again");
            Debug.Log("wrong: " + _finalMeshSavePath);
            return;
        } else {
            //Debug.Log(_finalMeshSavePath);
        }

        // save texture
        File.WriteAllBytes(_finalTextureSavePath, bytes);



        // generate new meshes
        // List<Mesh> newMeshes = null;
        if(toBeCombinedTexItems.Count <= 0) {
            return;
        } else {
            _combinerResults.SetItems(toBeCombinedTexItems);
            //int meshFolderIndex = combinedTexturePath.LastIndexOf("/");

            if(!_finalMeshSavePath.EndsWith("/")) {
                _finalMeshSavePath += "/";
            }
            //Debug.Log(meshFolderPath);
            List<Mesh> newMeshes = GenerateFitMeshes(toBeCombinedTexItems, finalTextureSize, canvasSize, _finalMeshSavePath);
            _combinerResults.SetMeshes(newMeshes);


        }

        // generate new material
        string matPath = _finalMaterialSavePath;
        matPath = matPath.Replace("png", "mat");
        if(matPath != "") {
            Material combinedMat = new Material(toBeCombinedTexItems[0].GetShader());
            //Debug.Log(textureChannelName);
            // texture is not ready
            //Texture2D newBornTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(combinedTexturePath);
            //combinedMat.SetTexture(textureChannelName, newBornTexture);
            AssetDatabase.CreateAsset(combinedMat, matPath);
            _combinerResults.SetMaterial(combinedMat);
        }


    }

    public static void CombineTheTexItemV2(int finalTextureSize, Rect canvasSize, string textureChannelName) {
        if(_texItems == null) {
            return;
        }

        _combinerResults = new CombinerResults();
        // generate combined texture
        Texture2D combinedTexture = new Texture2D(finalTextureSize, finalTextureSize, TextureFormat.RGBA32, true);
        List<TexItem> toBeCombinedTexItems = new List<TexItem>();
        for(int i = 0; i < _texItems.Count; i++) {
            if(_texItems[i].theTexture != null && _texItems[i].isPressed && _texItems[i].IsSelected()) {
                //Debug.Log(_texItems[i].theTexture.name);
                Vector2 finalPos = _texItems[i].GetPositionForFinalTexture(finalTextureSize, canvasSize);
                finalPos = TransformFromCanvasSpaceToTextureSpace(finalPos, finalTextureSize, canvasSize);
                finalPos.y -= (int)_texItems[i].theTexture.height;
                _texItems[i].SetUVOffset(finalPos);
                //Debug.Log(finalPos);
                combinedTexture.SetPixels((int)finalPos.x, (int)finalPos.y, (int)_texItems[i].theTexture.width, (int)_texItems[i].theTexture.height, _texItems[i].theTexture.GetPixels());
                toBeCombinedTexItems.Add(_texItems[i]);
            }
        }

        combinedTexture.Apply();
        byte[] bytes = combinedTexture.EncodeToPNG();
        string combinedTexturePath = EditorUtility.SaveFilePanel("选择贴图保存目录", "Assets", "combinedTexture", "png");
        int index = combinedTexturePath.IndexOf("Assets/");
        if(index >= 0) {
            combinedTexturePath = combinedTexturePath.Substring(index);
            //Debug.Log(combinedTexturePath);
            File.WriteAllBytes(combinedTexturePath, bytes);
        }


        // generate new meshes
        // List<Mesh> newMeshes = null;
        if(toBeCombinedTexItems.Count <= 0) {
            return;
        } else {
            _combinerResults.SetItems(toBeCombinedTexItems);
            int meshFolderIndex = combinedTexturePath.LastIndexOf("/");
            if(meshFolderIndex >= 0) {
                string meshFolderPath = combinedTexturePath;
                meshFolderPath = meshFolderPath.Replace(meshFolderPath.Substring(meshFolderIndex), "") + "/";
                //Debug.Log(meshFolderPath);
                List<Mesh> newMeshes = GenerateFitMeshes(toBeCombinedTexItems, finalTextureSize, canvasSize, meshFolderPath);
                _combinerResults.SetMeshes(newMeshes);
            }

        }

        // generate new material
        string matPath = combinedTexturePath;
        matPath = matPath.Replace("png", "mat");
        if(matPath != "") {
            Material combinedMat = new Material(toBeCombinedTexItems[0].GetShader());
            //Debug.Log(textureChannelName);
            // texture is not ready
            //Texture2D newBornTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(combinedTexturePath);
            //combinedMat.SetTexture(textureChannelName, newBornTexture);
            AssetDatabase.CreateAsset(combinedMat, matPath);
            _combinerResults.SetMaterial(combinedMat);
        }


    }


    private static Vector2 TransformFromCanvasSpaceToTextureSpace(Vector2 posInCanvas, int finalTextureSize, Rect canvasSize) {
        posInCanvas.y -= finalTextureSize;
        posInCanvas.y = Mathf.Abs(posInCanvas.y);
        return posInCanvas;
    }

    /*
    private static void CreateMaterialForCombinedTexItem(TexItem item, string[] texturePaths) {
        Shader shader = item.GetShader();
        if(shader == null) {
            return;
        }
        

        Material material = new Material(item.GetMaterial());
        string matPath = texturePaths[0].Replace(".png", ".mat");
        AssetDatabase.CreateAsset(material, matPath);
        
    }

    private static void SetTextureForMaterial(Material mat, Texture2D texture) {
        string[] textureChannels = mat.GetTexturePropertyNames();
        for(int i = 0; i < textureChannels.Length; i++) {

        }
    }
    */
    private static List<Mesh> GenerateFitMeshes(List<TexItem> texItems, int finalTextureSize, Rect canvasSize, string savePath = "Assets/") {
        /*
        if(texItems == null) {
            return ;
        }
        */

        List<Mesh> meshes = new List<Mesh>();
        foreach(TexItem ti in texItems) {
            Vector4 scaleOffset = new Vector4(ti.sizeForCanvas.x, ti.sizeForCanvas.y, ti.positionForFinalTexture.x, ti.positionForFinalTexture.y);
            scaleOffset.x = scaleOffset.x / (float)canvasSize.width;
            scaleOffset.y = scaleOffset.y / (float)canvasSize.height;

            scaleOffset.z = ti.uvOffset.x / (float)finalTextureSize;
            scaleOffset.w = ti.uvOffset.y / (float)finalTextureSize;

            //Debug.Log("scaleoffset: " + scaleOffset.x + " | " + scaleOffset.y + " | " + scaleOffset.z + " | " + scaleOffset.w);
            //string meshPath = AssetDatabase.GetAssetPath(ti.GetSharedMesh());
            Mesh newMesh = CopyMeshWithNewUVFromOldMesh(ti.GetName() + "_temp", ti.GetSharedMesh(), scaleOffset);
            ti.SetNewBorn(newMesh);
            AssetDatabase.CreateAsset(newMesh, savePath + newMesh.name + ".mesh");
            meshes.Add(newMesh);
        }
        return meshes;

    }

    static Mesh CopyMeshWithNewUVFromOldMesh(string meshName, Mesh oldMesh, Vector4 scaleOffset) {
        Vector3[] vertices = oldMesh.vertices;
        int[] tris = oldMesh.triangles;
        Vector3[] normals = oldMesh.normals;
        Vector2[] uv1 = GetNewUV1(oldMesh.uv, scaleOffset);
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        //mesh.uv = uv1;
        mesh.uv = uv1;
        mesh.uv2 = oldMesh.uv2;
        mesh.name = meshName;
        return mesh;
    }

    static Vector2[] GetNewUV1(Vector2[] oldUV, Vector4 scaleOffset) {
        Matrix4x4 uvMatrix = new Matrix4x4(
        new Vector4(scaleOffset.x, 0, 0, 0),
        new Vector4(0, scaleOffset.y, 0, 0),
        new Vector4(0, 0, 1, 0),
        new Vector4(scaleOffset.z, scaleOffset.w, 0, 1)
        );
        Vector2[] newUV = new Vector2[oldUV.Length];
        for(int i = 0; i < newUV.Length; i++) {
            Vector3 newPoint = uvMatrix.MultiplyPoint(new Vector3(oldUV[i].x, oldUV[i].y, 0));
            newUV[i].x = newPoint.x;
            newUV[i].y = newPoint.y;
        }
        return newUV;
        /*
        Matrix4x4 uvMatrix = new Matrix4x4(
                new Vector4(scaleOffset.x, 0, 0, 0),
                new Vector4(0, scaleOffset.y, 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(scaleOffset.z, scaleOffset.w, 0, 1)
            );
        Vector2[] newUV2 = new Vector2[oldUV.Length];
        for(int i = 0; i < newUV2.Length; i++) {
            newUV2[i].x = uvMatrix.MultiplyPoint(new Vector3(oldUV[i].x, oldUV[i].y, 0)).x;
            newUV2[i].y = uvMatrix.MultiplyPoint(new Vector3(oldUV[i].x, oldUV[i].y, 0)).y;
        }
        return newUV2;
        */
    }

    private static void CheckPackTexturesRect(Rect[] rects) {
        if(rects.Length > 0) {
            foreach(Rect rect in rects) {
                Debug.Log(rect.position.ToString());
            }
        }
    }

    public static void RemoveFromList() {

        if(_texItems == null) {
            return;
        }

        List<TexItem> temp = new List<TexItem>();



        int num = _texItems.Count;
        for(int i = 0; i < num; i++) {
            if(!_texItems[i].IsSelected()) {
                temp.Add(_texItems[i]);
            }
        }
        _texItems.Clear();
        _texItems = temp;
        //temp.Clear();
    }

    public static void AddData() {
        if(_texItems == null) {
            _texItems = new List<TexItem>();
        }
        foreach(var obj in Selection.objects) {
            GameObject gameObject = (GameObject)obj;
            if(gameObject != null) {
                MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                if(mr != null) {
                    TexItem ti = new TexItem(mr.gameObject.name, mr);
                    if(!TexItemList.Contains(ti)) {
                        if(mr.gameObject.GetComponent<CanvasPositionHolder>() == null) {
                            mr.gameObject.AddComponent<CanvasPositionHolder>();
                        }
                        _texItems.Add(ti);

                    }
                }
            }
        }
    }

    public static void AddDataV2() {
        if(_texItems == null) {
            _texItems = new List<TexItem>();
        }
        foreach(var obj in Selection.objects) {
            GameObject gameObject = (GameObject)obj;
            if(gameObject != null) {
                MeshRenderer[] mrs = gameObject.GetComponentsInChildren<MeshRenderer>();
                if(mrs.Length > 0) {
                    foreach(MeshRenderer mr in mrs) {
                        TexItem ti = new TexItem(mr.gameObject.name, mr);
                        if(!TexItemList.Contains(ti)) {
                            if(mr.gameObject.GetComponent<CanvasPositionHolder>() == null) {
                                mr.gameObject.AddComponent<CanvasPositionHolder>();
                            }
                            _texItems.Add(ti);
                        }
                    }
                }
            }
        }
    }

    public static bool Contains(TexItem ti) {
        if(_texItems == null) {
            _texItems = new List<TexItem>();
            return false;
        } else {
            bool status = false;
            foreach(TexItem temp in _texItems) {
                if(temp == ti) {
                    status = true;
                }
            }
            return status;
        }
    }

    public static void SetData() {
        if(_texItems != null) {
            _texItems.Clear();
        }
        _texItems = new List<TexItem>();
        if(Selection.objects.Length == 0) {
            return;
        }

        for(int i = 0; i < Selection.objects.Length; i++) {
            GameObject gameObject = (GameObject)Selection.objects[i];
            if(gameObject != null) {
                MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                if(mr != null) {
                    TexItem ti = new TexItem(mr.gameObject.name, mr);
                    if(!TexItemList.Contains(ti)) {
                        if(mr.gameObject.GetComponent<CanvasPositionHolder>() == null) {
                            mr.gameObject.AddComponent<CanvasPositionHolder>();
                        }
                        _texItems.Add(ti);
                    }

                }
            }
        }

    }
}


/*
public class TexBlock {
    Texture2D texture2D;
    public TexBlock(Texture2D texture2D) {
        this.texture2D = texture2D;
    }

    public void Draw(Vector2 position, Vector2 size) {
    }

}
*/
