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
            //Debug.Log(kvp.Key + " is added.");
            //go.AddComponent<MeshFilter>().mesh = GetCombinedMesh(container[kvp.Key], go.gameObject.name + "_mesh");
            go.AddComponent<MeshFilter>().mesh = GetCombinedMeshV2(container[kvp.Key], go.gameObject.name + "_mesh");
            i++;
        }
    }

    private Mesh GetCombinedMeshV2(List<MeshFilter> meshFilters, string combinedMeshName) {
        //Debug.Log(meshFilters.Count + " meshes....");
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Count];
        for(int i = 0; i < combineInstances.Length; i++) {
            combineInstances[i] = new CombineInstance();
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.name = combinedMeshName;
        combinedMesh.CombineMeshes(combineInstances);

        return combinedMesh;
    }

    private Mesh GetCombinedMesh(List<MeshFilter> meshFilters, string meshName) {
        List<CombineInstance> combinedInstances = new List<CombineInstance>();
        CombineInstance ci;
        for(int i = 0; i < meshFilters.Count; i++) {
            ci = new CombineInstance();
            ci.mesh = meshFilters[i].mesh;
            ci.transform = meshFilters[i].transform.localToWorldMatrix;
            combinedInstances.Add(ci);
            break;
        }

        //Debug.Log(combinedInstances.Count);



        Mesh combinedMesh = new Mesh();
        combinedMesh.name = meshName;
        combinedMesh.CombineMeshes(combinedInstances.ToArray());
        //Debug.Log(combinedMesh.vertices.Length);
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
