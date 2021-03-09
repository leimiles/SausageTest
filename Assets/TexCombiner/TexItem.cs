using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexItem {
    string name;
    Shader shader;
    MeshRenderer mr;
    MeshFilter mf;
    Material material;
    List<Texture2D> textures;
    bool isSelected;
    public bool isPressed;
    public Vector2 position;
    public Vector2 sizeForCanvas;
    //public string meshGuid;
    public CanvasPositionHolder cph;
    public Mesh newBorn;
    public bool isMoving;

    public TexItem(string name, MeshRenderer mr) {
        this.name = name;
        this.mr = mr;
        this.mf = mr.GetComponent<MeshFilter>();
        if(mf.sharedMesh != null) {
            this.name = mf.sharedMesh.name;
        }
        this.shader = mr.sharedMaterial.shader;
        this.material = mr.sharedMaterial;
        isSelected = false;
        isPressed = false;
        textures = new List<Texture2D>();
        this.textures = this.GetTextures();
        cph = mr.GetComponent<CanvasPositionHolder>();
        if(cph != null) {
            position = cph.position;
        } else {
            position = new Vector2(0, 0);
        }
        sizeForCanvas = new Vector2(125, 125);
        positionForFinalTexture = this.position;
        //SetMeshGUID(mf.sharedMesh);
        isMoving = false;

    }

    public void SetNewBorn(Mesh mesh) {
        this.newBorn = mesh;
    }

    public Mesh GetNewBorn() {
        return this.newBorn;
    }

    public Transform GetTransform() {
        return this.mr.gameObject.transform;
    }

    public string GetObjName() {
        return this.mr.gameObject.name;
    }

    public Vector2 uvOffset;

    public void SetUVOffset(Vector2 offset) {
        this.uvOffset = offset;
    }

    public Shader GetShader() {
        return this.shader;
    }

    public Material GetMaterial() {
        return this.material;
    }

    public Mesh GetSharedMesh() {
        return this.mf.sharedMesh;
    }

    public Vector2 positionForFinalTexture;
    public Vector2 GetPositionForFinalTexture(int outputSize, Rect canvasSize, float textureScale = 1.0f) {
        positionForFinalTexture.x = this.position.x * ((float)outputSize / canvasSize.width);
        positionForFinalTexture.y = this.position.y * ((float)outputSize / canvasSize.height);
        //for texture space

        return this.positionForFinalTexture;
    }



    public bool ContainsTextures() {
        if(this.textures != null && this.textures.Count >= 1) {
            return true;
        } else {
            return false;
        }
    }

    public Vector2 SetSizeForCanvas(int outputSize, Rect canvasSize, float textureScale = 1.0f) {
        //Debug.Log(outputSize);
        if(this.theTexture == null) {
            return this.sizeForCanvas;
        }

        sizeForCanvas.x = canvasSize.width * ((this.theTexture.width * textureScale) / (float)outputSize);
        sizeForCanvas.y = canvasSize.height * ((this.theTexture.height * textureScale) / (float)outputSize);


        return this.sizeForCanvas;
    }


    /*
    public void SetPositionAndSize(int sizeScale) {

        if(this.theTexture != null) {
            this.sizeForCanvas.x *= 0.5f;
            this.sizeForCanvas.y *= 0.5f ;
            //Debug.Log(sizeScale);
            //this.size *= sizeScale;
        }
    }
    */

    public string GetTheTextureName() {
        if(theTexture != null) {
            return theTexture.name;
        } else {
            return "Null The Texture";
        }
    }

    public Texture2D theTexture;
    public Texture2D GetTheTexture(Material mat, string textureName) {
        if(this.material.shader.name != mat.shader.name) {
            return null;
        }
        theTexture = (Texture2D)material.GetTexture(textureName);
        return theTexture;
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

    public static bool operator ==(TexItem ti1, TexItem ti2) {
        bool status = false;
        if(ti1.mr.GetComponent<MeshFilter>().sharedMesh.name == ti2.mr.GetComponent<MeshFilter>().sharedMesh.name) {
            if(ti1.material.name == ti2.material.name) {
                status = true;
            }
        }
        return status;
    }

    public static bool operator !=(TexItem ti1, TexItem ti2) {
        bool status = false;
        if(ti1.mr.GetComponent<MeshFilter>().sharedMesh.name != ti2.mr.GetComponent<MeshFilter>().sharedMesh.name) {
            if(ti1.material.name != ti2.material.name) {
                status = true;
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
