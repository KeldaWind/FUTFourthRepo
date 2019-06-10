Shader "Toon_Water"
{
    Properties
    {	

		/// COULEUR EAU
		// Couleurs eau
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
		// Cutoff dégradé (variable surement à tweak)
		_DepthMaxDistance("Depth Maximum Distance", Float) = 1

		///VAGUES
		_SurfaceNoise("Surface Noise", 2D) = "white" {}
		_SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0, 1)) = 0.777

		/// ECUME
		_FoamMaxDistance("Foam Maximum Distance", Float) = 0.4 // epaisseur mousse bords objets immergés
		_FoamMinDistance("Foam Minimum Distance", Float) = 0.04 // epaisseur mousse bords plane

		/// ANIMATION
		_SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
		_SurfaceDistortion("Surface Distortion", 2D) = "white" {}
		_SurfaceDistortionAmount("Surface Distortion Amount", Range(0, 1)) = 0.27 // force distortion

		/// DEBUG
		_FoamSmooth("Foam Smooth", Range(0,0.15)) = 0.05
		//_FakeDepthMap("Fake Depth Map", 2D) = "black" {}
		//_FakeDepthIntensity("Fake Depth Intensity", Range(0,1)) = 0
    }
    SubShader
    {
        Pass
        {
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#define SMOOTHSTEP_AA 0.05

			// INCLUDES
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float4 screenPosition : TEXCOORD2;
				float2 noiseUV : TEXCOORD0;
				float2 distortUV : TEXCOORD1;
				float3 viewNormal : NORMAL;
            };

			// Déclarations properties
			sampler2D _SurfaceNoise;
			float4 _SurfaceNoise_ST;
			float _SurfaceNoiseCutoff;
			float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;
			float _DepthMaxDistance;
			sampler2D _CameraDepthTexture; // valeurs de gris selon profondeur
			float _FoamMaxDistance;
			float _FoamMinDistance;
			float2 _SurfaceNoiseScroll;
			sampler2D _SurfaceDistortion;
			float4 _SurfaceDistortion_ST;
			float _SurfaceDistortionAmount;
			sampler2D _CameraNormalsTexture;

			sampler2D _FakeDepthMap;
			float _FakeDepthIntensity;
			float _FoamSmooth;

			// VERTEX SHADER
            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPosition = ComputeScreenPos(o.vertex);
				o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
				o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
				o.viewNormal = COMPUTE_VIEW_NORMAL;

                return o;
            }

			// FRAGMENT SHADER
            float4 frag (v2f i) : SV_Target
            {


				// dégradé de gris pour montrer profondeur
				///float existingDepth01 = tex2Dproj(_FakeDepthMap, UNITY_PROJ_COORD(i.screenPosition)).r * _FakeDepthIntensity; // valeur non linéaire
				float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r; // valeur non linéaire
				float existingDepthLinear = LinearEyeDepth(existingDepth01); // conversion en valeur linéaire

				
				float depthDifference = existingDepthLinear - i.screenPosition.w;
				//float depthDifference = existingDepth01 - i.screenPosition.w;

				// mise en couleur du dégradé de gris
				float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
				float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);
				// Ecume bords objets immergés

				float3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.screenPosition));
				float3 normalDot = saturate(dot(existingNormal, i.viewNormal));
				float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);

				// Ecume bord plane
				float foamDepthDifference01 = saturate(depthDifference / foamDistance);


				float surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutoff;
				float2 distortSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;
				// Ajustement noise écume
				float2 noiseUV = float2((i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x, (i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y); // anim
				float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;
				float surfaceNoise = step(surfaceNoiseCutoff, surfaceNoiseSample);
				//float surfaceNoise = smoothstep(surfaceNoiseCutoff - _FoamSmooth, surfaceNoiseCutoff + _FoamSmooth, surfaceNoiseSample);
				

				return waterColor + surfaceNoise;

            }

            ENDCG
        }
    }
}