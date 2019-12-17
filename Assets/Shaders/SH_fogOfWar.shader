// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Leonidas Legacy/SH_fogOfWar"
{
	Properties
	{
		_VisibleRenderTexture("Visible Render Texture", 2D) = "white" {}
		_ReavaledRenderTexture("Reavaled Render Texture", 2D) = "white" {}
		_VisibleFogColor("Visible Fog Color", Color) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap vertex:vertexDataFunc 
		struct Input
		{
			float4 vertexToFrag88;
		};

		uniform float4 _VisibleFogColor;
		uniform sampler2D _VisibleRenderTexture;
		float4x4 unity_Projector;
		uniform sampler2D _ReavaledRenderTexture;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_vertex4Pos = v.vertex;
			o.vertexToFrag88 = mul( unity_Projector, ase_vertex4Pos );
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _VisibleFogColor.rgb;
			float2 UV92 = ( (i.vertexToFrag88).xy / (i.vertexToFrag88).w );
			float grayscale74 = Luminance(tex2D( _VisibleRenderTexture, UV92 ).rgb);
			float visibleGreyScale10 = grayscale74;
			float grayscale75 = Luminance(tex2D( _ReavaledRenderTexture, UV92 ).rgb);
			float revealedGreyScale31 = grayscale75;
			o.Alpha = ( 1.0 - ( visibleGreyScale10 + (revealedGreyScale31*0.5 + 0.0) ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
401;73;970;545;933.0692;1044.524;1.353483;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;86;-1165.384,-1084.507;Float;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.UnityProjectorMatrixNode;85;-1165.384,-1164.507;Float;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-957.384,-1164.507;Float;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexToFragmentNode;88;-813.384,-1164.507;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;90;-573.384,-1084.507;Float;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;89;-573.384,-1164.507;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;91;-333.384,-1164.507;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;92;-54.47729,-1108.388;Float;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;22;-1066.245,-502.1151;Float;True;Property;_ReavaledRenderTexture;Reavaled Render Texture;1;0;Create;True;0;0;False;0;415db34ea95895944a90477051d7f3f7;e93f1a3b0294efb4b9f655eeb7186220;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-990.3185,-582.67;Float;False;92;UV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-1065.597,-824.6785;Float;True;Property;_VisibleRenderTexture;Visible Render Texture;0;0;Create;True;0;0;False;0;415db34ea95895944a90477051d7f3f7;415db34ea95895944a90477051d7f3f7;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;17;-721.0123,-516.5862;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-684.1692,-796.8138;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;75;-263.3438,-505.8137;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;74;-302.5099,-795.7855;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-8.339065,-507.7195;Float;False;revealedGreyScale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-1225.756,624.525;Float;False;Constant;_Float2;Float 2;3;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-1404.709,392.3595;Float;True;31;revealedGreyScale;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-64.29809,-788.6508;Float;False;visibleGreyScale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;84;-1007.693,383.9495;Float;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;11;-1330.364,139.0402;Float;True;10;visibleGreyScale;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;77;-723.7157,141.6252;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;81;-474.2069,141.0472;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;5;-1077.605,-134.204;Float;False;Property;_VisibleFogColor;Visible Fog Color;2;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;16;-219.5564,-166.6095;Float;False;True;6;Float;ASEMaterialInspector;0;0;Unlit;Leonidas Legacy/SH_fogOfWar;False;False;False;False;True;True;True;True;True;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;87;0;85;0
WireConnection;87;1;86;0
WireConnection;88;0;87;0
WireConnection;90;0;88;0
WireConnection;89;0;88;0
WireConnection;91;0;89;0
WireConnection;91;1;90;0
WireConnection;92;0;91;0
WireConnection;17;0;22;0
WireConnection;17;1;93;0
WireConnection;3;0;1;0
WireConnection;3;1;93;0
WireConnection;75;0;17;0
WireConnection;74;0;3;0
WireConnection;31;0;75;0
WireConnection;10;0;74;0
WireConnection;84;0;27;0
WireConnection;84;1;73;0
WireConnection;77;0;11;0
WireConnection;77;1;84;0
WireConnection;81;0;77;0
WireConnection;16;2;5;0
WireConnection;16;9;81;0
ASEEND*/
//CHKSM=FAFF4C7469EF51DEC132D2104CCF78F72EAE1ABA