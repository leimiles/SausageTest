using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TexItemList {
    private static List<TexItem> _texItems;
    public static List<TexItem> GetTexItems() {
        return _texItems;
    }

    public static void RemoveFromList(int index) {

    }

    public static void AddData() {

    }



    public static void SetData() {
        if(_texItems != null) {
            _texItems.Clear();
        }
        _texItems = new List<TexItem>();

        foreach(var obj in Selection.objects) {
            GameObject gameObject = (GameObject)obj;
            if(gameObject != null) {
                MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                if(mr != null) {
                    TexItem ti = new TexItem(mr.gameObject.name, mr);
                    _texItems.Add(ti);
                }
            }
        }
    } 
}

public class TexItem {
    string name;
    Shader shader;
    MeshRenderer mr;
    MeshFilter mf;
    Material material;
    List<Texture2D> textures;
    public TexItem(string name, MeshRenderer mr) {
        this.name = name;
        this.mr = mr;
        this.mf = mr.GetComponent<MeshFilter>();
        this.shader = mr.sharedMaterial.shader;
        this.material = mr.sharedMaterial;
        textures = new List<Texture2D>();
    }

    public string GetName() {
        return name;
    }

    public List<Texture2D> GetTextures() {
        int[] texturesIds = this.material.GetTexturePropertyNameIDs();
        if(texturesIds.Length >= 1) {
            for(int i = 0; i < texturesIds.Length; i++) {
                this.textures.Add((Texture2D)this.material.GetTexture(texturesIds[i]));
            }
        }
        return this.textures;
    }

    public void Draw() {
        GUILayout.Box(this.name);
    }
}

