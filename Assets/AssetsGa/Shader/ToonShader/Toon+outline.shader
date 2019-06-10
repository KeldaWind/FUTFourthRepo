Shader "CellShading"
{
	Properties
	{
		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}

		[HDR] // couleur ciel
		_AmbientColor("Ambient Color", Color) = (0.4, 0.4, 0.4, 1)

		[HDR] // specular reflection
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		_Glossiness("Glossiness", Float) = 32

		[HDR] // rim light
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1 // longueur / largeur de la ligne de rim light

		// Outline
		_OutlineExtrusion("Outline Extrusion", float) = 0
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineDot("Outline Dot", float) = 0.25
	}

	SubShader
	{
		Pass // cel shading
		{
			Tags
			{
			"LightMode" = "ForwardBase" // indique mode de rendu
			"PassFlags" = "OnlyDirectional" // slmt lumiere de type directional
			}

			Stencil
			{
				Ref 4
				Comp always
				Pass replace
				ZFail keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase // Shortcut Unity pour compiler les ombres

			// Includes
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"


			// Buildup fonction application of data
			struct appdata
			{
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};
			

			// Buildup fonction vertex to fragment
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : NORMAL;
				float3 viewDir : TEXCOORD1;

				// Collect infos ombres
				SHADOW_COORDS(2)
			};


			// Déclarations variables
			
				// ?
			sampler2D _MainTex;
			float4 _MainTex_ST;

				// Specular reflection
			float _Glossiness;
			float4 _SpecularColor;

				// Rim light
			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold; // ligne de la rim light
			
			// VERTEX SHADER : recupère les infos de l'object
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				// Transfer infos ombres
				TRANSFER_SHADOW(o)
				return o;
			};
			
			float4 _Color;
			float4 _AmbientColor;

			// FRAGMENT SHADER : assigne en découpes
			float4 frag (v2f i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);
				float shadow = SHADOW_ATTENUATION(i);
				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow); // déclaration light intensity avec smoothstep en prenant en compte d'autres ombres
				float4 light = lightIntensity * _LightColor0; // _LightColor0 = couleur of the main directional light

				// Specular reflection
				float3 viewDir = normalize(i.viewDir);

				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);
				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float4 rimDot = 1 - dot(viewDir, normal); // rim light sur les bords en fonction de la vue cam'

					// Toonify la rim light
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
				float4 rim = rimIntensity * _RimColor;
					// Toonify la specular reflection
				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;

				float4 sample = tex2D(_MainTex, i.uv);


				return _Color * sample * (_AmbientColor + light + specular + rim);

			}
			
			ENDCG
		}

		Pass // outline
		{
			Cull OFF
			ZWrite OFF
			ZTest ON
			Stencil
			{
				Ref 4
				Comp notequal
				Fail keep
				Pass replace
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Properties
			uniform float4 _OutlineColor;
			uniform float _OutlineSize;
			uniform float _OutlineExtrusion;
			uniform float _OutlineDot;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				float4 newPos = input.vertex;

				// normal extrusion technique
				float3 normal = normalize(input.normal);
				newPos += float4(normal, 0.0) * _OutlineExtrusion;

				// convert to world space
				output.pos = UnityObjectToClipPos(newPos);

				output.color = _OutlineColor;
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				// checker value will be negative for 4x4 blocks of pixels
				// in a checkerboard pattern
				//input.pos.xy = floor(input.pos.xy * _OutlineDot) * 0.5;
				//float checker = -frac(input.pos.r + input.pos.g);

				return input.color;
			}

			ENDCG
		}

		// Shadow casting support par Unity
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}

}