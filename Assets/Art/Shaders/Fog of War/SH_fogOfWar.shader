// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Leonidas Legacy/SH_fogOfWar"
{
	Properties
	{
		_VisibleRenderTexture("Visible Render Texture", 2D) = "white" {}
		_ReavaledRenderTexture("Reavaled Render Texture", 2D) = "white" {}
		_VisibleFogColor("Visible Fog Color", Color) = (0,0,0,0)
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
			};

			uniform float4 _VisibleFogColor;
			uniform sampler2D _ReavaledRenderTexture;
			float4x4 unity_Projector;
			uniform sampler2D _VisibleRenderTexture;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 vertexToFrag88 = mul( unity_Projector, v.vertex );
				o.ase_texcoord = vertexToFrag88;
				
				float3 vertexValue =  float3(0,0,0) ;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 finalColor;
				float4 vertexToFrag88 = i.ase_texcoord;
				float2 UV92 = ( (vertexToFrag88).xy / (vertexToFrag88).w );
				float2 temp_output_17_0_g6 = UV92;
				float4 appendResult114 = (float4(_VisibleFogColor.rgb , ( 1.0 - ( tex2D( _ReavaledRenderTexture, temp_output_17_0_g6 ).r + (tex2D( _VisibleRenderTexture, temp_output_17_0_g6 ).r*0.5 + 0.0) ) )));
				
				
				finalColor = appendResult114;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16400
869;364;822;784;586.0635;205.8607;1;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;86;-1165.384,-1084.507;Float;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.UnityProjectorMatrixNode;85;-1165.384,-1164.507;Float;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-957.384,-1164.507;Float;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexToFragmentNode;88;-813.384,-1164.507;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;90;-573.384,-1084.507;Float;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;89;-573.384,-1164.507;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;91;-333.384,-1164.507;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;92;-54.47729,-1108.388;Float;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;22;-1457.804,102.1081;Float;True;Property;_ReavaledRenderTexture;Reavaled Render Texture;1;0;Create;True;0;0;False;0;415db34ea95895944a90477051d7f3f7;e93f1a3b0294efb4b9f655eeb7186220;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-1248.199,381.4355;Float;False;92;UV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-1457.896,-224.8671;Float;True;Property;_VisibleRenderTexture;Visible Render Texture;0;0;Create;True;0;0;False;0;415db34ea95895944a90477051d7f3f7;415db34ea95895944a90477051d7f3f7;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ColorNode;5;-359.6971,-170.224;Float;False;Property;_VisibleFogColor;Visible Fog Color;3;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;112;-885.8787,102.3848;Float;True;SH_FogOfWar_MergedRenderTextures;-1;;6;262719dec9990b343ac98842b2b6281c;0;3;18;SAMPLER2D;;False;13;SAMPLER2D;0;False;17;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;114;-101.9856,3.075256;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;113;102.9362,15.41363;Float;False;True;2;Float;ASEMaterialInspector;0;1;Leonidas Legacy/SH_fogOfWar;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;0;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;87;0;85;0
WireConnection;87;1;86;0
WireConnection;88;0;87;0
WireConnection;90;0;88;0
WireConnection;89;0;88;0
WireConnection;91;0;89;0
WireConnection;91;1;90;0
WireConnection;92;0;91;0
WireConnection;112;18;1;0
WireConnection;112;13;22;0
WireConnection;112;17;93;0
WireConnection;114;0;5;0
WireConnection;114;3;112;0
WireConnection;113;0;114;0
ASEEND*/
//CHKSM=F7D6813F2E12DDD2E6FFEDECF1E0379A6C80899D