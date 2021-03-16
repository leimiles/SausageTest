using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerResults {
    Material materialForResults;
    Mesh[] meshesForResult;
    Matrix4x4[] matrix4X4sForResult;
    Texture2D[] textures;
    public List<TexItem> combinedItems;
    public bool IsReady() {
        bool status = true;
        if(this.materialForResults == null) {
            status = false;

        }
        if(this.combinedItems == null || this.combinedItems.Count == 0) {
            status = false;
        }
        if(this.meshesForResult == null || this.meshesForResult.Length <= 0) {
            status = false;
        }
        return status;
    }

    public void SetMaterial(Material mat) {
        this.materialForResults = mat;
        
    }

    public Mesh[] GetMeshes() {
        return this.meshesForResult;
    }

    public Material GetMaterial() {
        return this.materialForResults;
    }

    public void SetItems(List<TexItem> items) {
        this.combinedItems = items; 
    }

    public void SetMeshes(List<Mesh> meshes) {
        this.meshesForResult = meshes.ToArray();
    }
}
