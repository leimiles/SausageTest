using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TexItemList {
    private static List<TexItem> _texItems;
    public static List<TexItem> GetTexItems() {
        return _texItems;
    }

    public static void RemoveFromList() {

        if(_texItems == null) {
            return;
        }

        List<TexItem> temp = new List<TexItem>();

       

        int num = _texItems.Count;
        for(int i = 0; i < num; i++ ) {
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
                        _texItems.Add(ti);
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
                    _texItems.Add(ti);

                }
            }
        }
        /*
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
        */
    }
}

public class TexItem {
    string name;
    Shader shader;
    MeshRenderer mr;
    MeshFilter mf;
    Material material;
    List<Texture2D> textures;
    bool isSelected;
    public TexItem(string name, MeshRenderer mr) {
        this.name = name;
        this.mr = mr;
        this.mf = mr.GetComponent<MeshFilter>();
        this.shader = mr.sharedMaterial.shader;
        this.material = mr.sharedMaterial;
        isSelected = false;
        textures = new List<Texture2D>();
        this.textures = this.GetTextures();
    }

    public bool ContainsTextures() {
        if(this.textures != null && this.textures.Count >= 1) {
            return true;
        } else {
            return false;
        }
    }

    public Texture2D GetTheTexture(Material mat, string textureName) {

        if(this.material.shader.name != mat.shader.name) {
            return null;
        }

        Texture2D texture = (Texture2D)material.GetTexture(textureName);
        return texture;
    }

    public bool IsSelected() {
        return this.isSelected;
    }

    public string GetName() {
        return name;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override bool Equals(object obj) {
        return base.Equals(obj);
    }

    public static bool operator== (TexItem ti1, TexItem ti2) {
        bool status = true;
        if(ti1.mr.name != ti1.mr.name) {
            status = false;
        }
        if(ti1.material.name != ti2.material.name) {
            status = false;
        }
        if(ti1.name != ti2.name) {
            status = false;
        }
        return status;
    }

    public static bool operator !=(TexItem ti1, TexItem ti2) {
        bool status = true;
        if(ti1.mr.name == ti1.mr.name) {
            if(ti1.material.name == ti2.material.name) {
                if(ti1.name == ti2.name) {
                    status = false;
                }
            }
        }
        return status;
    }

    public List<Texture2D> GetTextures() {
        int[] texturesIds = this.material.GetTexturePropertyNameIDs();
        if(texturesIds.Length >= 1) {
            for(int i = 0; i < texturesIds.Length; i++) {
                //this.textures.Add((Texture2D)this.material.GetTexture(texturesIds[i]));
                Texture2D tex = (Texture2D)this.material.GetTexture(texturesIds[i]);
                if(tex != null) {
                    this.textures.Add(tex);
                }
            }
        }
        return this.textures;
    }



    public void Draw(string shaderName) {
        GUILayout.BeginHorizontal();
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        //this.isSelected = GUILayout.Toggle(this.isSelected, "", GUILayout.Width(15));
        if(this.shader.name == shaderName) {
            style.normal.textColor = Color.black;
            //this.isSelected = true;
        } else {
            style.normal.textColor = Color.red;
            //this.isSelected = false;
        }
        this.isSelected = GUILayout.Toggle(this.isSelected, "", GUILayout.Width(15));
        GUILayout.Box(this.name, style, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        /*
        if(this.isSelected == true) {
            Debug.Log(this.name + " is selected. ");
        }
        */
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
