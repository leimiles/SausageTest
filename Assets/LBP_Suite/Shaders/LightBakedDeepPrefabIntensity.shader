Shader "MileShader/LightBakedDeepPrefabIntensity" {
	Properties {
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Lightmap("Lightmap", 2D) = "white" {}
		//[PerRendererData]_Lightmap_ST("LightmapUV", Vector) = (1, 1, 0, 0)
		_SpecularColor("Specular Color", Color) = (0, 0, 0, 0)
		_Gloss("Gloss", Range(8, 200)) = 50
		_LightmapWeight("Lightmap Effect", Range(0, 1)) = 1
	}
	SubShader {
		Tags {"RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "LightweightPipeline"}
		LOD 200
		Pass {
			Name "LBP"
			Tags {"LightMode" = "LightweightForward"}
			HLSLPROGRAM
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			#pragma vertex vert
			#pragma fragment frag

			// when work with realtime shadows I need these macros
			#pragma shader_feature _RECEIVE_SHADOWS_OFF
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

			#pragma multi_compile_fog
			#pragma multi_compile_instancing

			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"

			sampler2D _MainTex;
			TEXTURE2D(_Lightmap);			SAMPLER(sampler_Lightmap);
			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float4 _Lightmap_ST;
			float _Gloss;
			half4 _SpecularColor;
			half _LightmapWeight;
			CBUFFER_END

			struct a2v {
				float4 positionOS :POSITION;
				float2 uv1 :TEXCOORD0;
				float2 uv2 :TEXCOORD1;
				half3 normal :NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 positionCS :SV_POSITION;
				float2 uvTex :TEXCOORD0;
				float2 uvLightmap :TEXCOORD1;
				half3 normal_World :TEXCOORD2;
				half3 view_World :TEXCOORD3;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord :TEXCOORD4;
				#endif
				float4 positionWSAndFog :TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(a2v v) {
				v2f o;
				o = (v2f)0;
				VertexPositionInputs vpi = GetVertexPositionInputs(v.positionOS.xyz);
				o.positionCS = vpi.positionCS;

				o.positionWSAndFog.xyz = vpi.positionWS;
				o.positionWSAndFog.w = ComputeFogFactor(vpi.positionCS.z);

				o.uvTex = TRANSFORM_TEX(v.uv1, _MainTex);
				o.uvLightmap = TRANSFORM_TEX(v.uv2, _Lightmap);
				//o.uvLightmap = v.uv2;

				VertexNormalInputs vni = GetVertexNormalInputs(v.normal.xyz);
				o.normal_World = vni.normalWS;
				o.view_World = GetCameraPositionWS() - vpi.positionWS;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				o.shadowCoord = GetShadowCoord(vpi);
				#endif
				return o;
			}

			half3 SampleLightmap_LBP(float2 lightmapUV, half3 normalWS) {
				#ifdef UNITY_LIGHTMAP_FULL_HDR
                bool encodeLightmap = false;
				#else
                bool encodeLightmap = true;
				#endif
				half4 decodeInstructions = half4(LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0h, 0.0h);
				half4 transformCoords = half4(1, 1, 0, 0);
				return SampleSingleLightmap(TEXTURE2D_PARAM(_Lightmap, sampler_Lightmap), lightmapUV, transformCoords, encodeLightmap, decodeInstructions);
			}

			void InitializeInputData(v2f o, out InputData inputData, float2 lightmapUV) {
				inputData.positionWS = o.positionWSAndFog.xyz;
				inputData.normalWS = NormalizeNormalPerPixel(o.normal_World);
				inputData.viewDirectionWS = SafeNormalize(o.view_World);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				inputData.shadowCoord = o.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
				inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
				#else
				inputData.shadowCoord = float4(0, 0, 0, 0);
				#endif

				//inputData.shadowCoord = float4(0, 0, 0, 0);
				inputData.fogCoord = o.positionWSAndFog.w;
				inputData.vertexLighting = half3(0, 0, 0);
				inputData.bakedGI = SampleLightmap_LBP(lightmapUV, inputData.normalWS);
			}

			half4 frag(v2f i) : SV_Target {
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				InputData inputData;
				InitializeInputData(i, inputData, i.uvLightmap);
				Light mainLight = GetMainLight(inputData.shadowCoord);
				// specular color
				half3 halfVec = SafeNormalize(mainLight.direction + inputData.viewDirectionWS);
				half3 NdotH = saturate(dot(inputData.normalWS, halfVec));
				half3 spec = pow(NdotH, _Gloss) * _SpecularColor.rgb;
				// main texture
				half4 mainTex = tex2D(_MainTex, i.uvTex);
				// receive realtime shadow
				half3 mixedRealtimeShadowWithLightmap = SubtractDirectMainLightFromLightmap(mainLight, inputData.normalWS, inputData.bakedGI);
				// mix maintexColor with lightmapColor
				half3 finalColorWithLightmap = min(half3(1, 1, 1), mainTex.rgb * mixedRealtimeShadowWithLightmap + mainTex.rgb * spec);
				half3 finalColor = mainTex.rgb + spec.rgb;
				// user change switch between lightmap and mainTex
				finalColor = lerp(finalColor, finalColorWithLightmap, _LightmapWeight);
				return half4(finalColor, 1);
	
			}
			ENDHLSL
		}
		Pass {
			Name "DepthOnly"
			Tags {"LightMode" = "DepthOnly"}

			ZWrite On
			ColorMask  0

			HLSLPROGRAM
			#pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

			#pragma vertex vertDepthOnly
			#pragma fragment fragDepthOnly

			#pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float4 _Lightmap_ST;
			float _Gloss;
			half4 _SpecularColor;
			CBUFFER_END
			
			struct a2v {
				float4 vertex :POSITION;
			};

			struct v2f {
				float4 positionCS :SV_POSITION;
			};

			v2f vertDepthOnly(a2v v) {
				v2f o;
				o = (v2f)0;
				VertexPositionInputs vpi = GetVertexPositionInputs(v.vertex.xyz);
				o.positionCS = vpi.positionCS;
				return o;
			}

			half4 fragDepthOnly(v2f i) : SV_TARGET {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				return 0;
			}

			ENDHLSL
		}
	}

	
}