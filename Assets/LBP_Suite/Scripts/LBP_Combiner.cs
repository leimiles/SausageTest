using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBP_Combiner : MonoBehaviour {

    List<MeshRenderer> listMeshRenders;
    public void Start() {
        
        GameObject[] objects = GameObject.FindGameObjectsWithTag("LBP Deep");
        if(objects.Length >= 1) {
            
            listMeshRenders = new List<MeshRenderer>();
            foreach(GameObject obj in objects) {
                listMeshRenders.AddRange(obj.GetComponentsInChildren<MeshRenderer>());
            }
            Dictionary<Material, List<MeshFilter>> temp = new Dictionary<Material, List<MeshFilter>>();
            
            foreach(MeshRenderer mr in listMeshRenders) {
                mr.gameObject.SetActive(false);
                if(!temp.ContainsKey(mr.sharedMaterial)) {

                    temp.Add(mr.sharedMaterial, new List<MeshFilter>());
                    temp[mr.sharedMaterial].Add(mr.GetComponent<MeshFilter>());
                } else {
                    temp[mr.sharedMaterial].Add(mr.GetComponent<MeshFilter>());
                }
            }

            // --------------------------------------------------------------------------------

            //Test(temp);
            GenerateCombinedMeshes(temp);
            //Debug.Log(listMeshRenders.Count);
        }

    }

    private void GenerateCombinedMeshes(Dictionary<Material, List<MeshFilter>> container) {
        int i = 0;
        foreach(KeyValuePair<Material, List<MeshFilter>> kvp in container) {

            GameObject go = new GameObject("LBP_Combined_" + i);
            go.AddComponent<MeshRenderer>().sharedMaterial = kvp.Key;
            go.AddComponent<MeshFilter>().mesh = GetCombinedMesh(container[kvp.Key], go.gameObject.name + "_mesh");
            i++;
        }
    }

    private Mesh GetCombinedMesh(List<MeshFilter> meshFilters, string meshName) {
        List<CombineInstance> combinedInstances = new List<CombineInstance>();
        for(int i = 0; i < meshFilters.Count; i++) {
            CombineInstance ci = new CombineInstance();
            ci.mesh = meshFilters[i].mesh;
            ci.transform = meshFilters[i].transform.localToWorldMatrix;
            combinedInstances.Add(ci);
        }
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = meshName;
        combinedMesh.CombineMeshes(combinedInstances.ToArray());
        return combinedMesh;
    }

    public void Test(Dictionary<Material, List<MeshFilter>> container) {
        foreach(Material t in container.Keys) {
            Debug.Log("MaterialName: " + t.name.ToUpper());
            foreach(MeshFilter mf in container[t]) {
                Debug.Log("MeshFilterName: " + mf.name);
            }
        }
    }
}
