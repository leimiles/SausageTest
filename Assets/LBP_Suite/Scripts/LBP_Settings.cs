using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MileCode {
    [ExecuteInEditMode]
    public class LBP_Settings : MonoBehaviour {
        public int lightmapIndex;
        public Vector4 lightmapScaleAndOffset = new Vector4(1, 1, 0, 0);
        public Texture2D lightmap;
        private Renderer renderer;
        private MaterialPropertyBlock propBlock;

        void SetLightmapValues(int index, Vector4 scaleAndOffset) {
            this.lightmapIndex = index;
            this.lightmapScaleAndOffset = scaleAndOffset;
        }

        private void Awake() {
            this.propBlock = new MaterialPropertyBlock();
            this.renderer = GetComponent<Renderer>();
        }

        private void Update() {
            if(propBlock != null) {
                renderer.GetPropertyBlock(propBlock);
                propBlock.SetVector("_Lightmap_ST", lightmapScaleAndOffset);
                if(lightmap != null) {
                    propBlock.SetTexture("_Lightmap", lightmap);
                }
                renderer.SetPropertyBlock(propBlock);
            }
        }
    }
}