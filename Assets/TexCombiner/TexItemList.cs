using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TexItemList {
    private static List<MeshRenderer> _meshRenderers;
    public static List<MeshRenderer> GetSelectionObject() {
        return _meshRenderers;
    }

    public static void RemoveFromList(int index) {

    }

    public static void AddData() {
        if(_meshRenderers == null) {
            return;
        }
        foreach(var go in Selection.objects) {
            MeshRenderer mr = (go as GameObject).GetComponent<MeshRenderer>();
            if(mr != null) {
                if(!_meshRenderers.Contains(mr)) {
                    _meshRenderers.Add(mr);
                }
            }
        }
    }

    public static void SetData() {
        if(_meshRenderers != null) {
            _meshRenderers.Clear();
        }
        _meshRenderers = new List<MeshRenderer>();
        if(!(Selection.objects.Length >= 1)) {
            return;
        } else {
            Debug.Log(Selection.objects[0].name);
        }
        foreach(var go in Selection.objects) {
            MeshRenderer mr = (go as GameObject).GetComponent<MeshRenderer>();
            if(mr != null) {
                if(!_meshRenderers.Contains(mr)) {
                    _meshRenderers.Add(mr);
                }
            }
        }

    }
}

