// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Leonidas Legacy/SH_fogOfWar"
{
	Properties
	{
		_VisibleRenderTexture("Visible Render Texture", 2D) = "white" {}
		_ReavaledRenderTexture("Reavaled Render Texture", 2D) = "white" {}
		_VisibleFogColor("Visible Fog Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 4.6
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _VisibleFogColor;
		uniform sampler2D _VisibleRenderTexture;
		uniform float4 _VisibleRenderTexture_ST;
		uniform sampler2D _ReavaledRenderTexture;
		uniform float4 _ReavaledRenderTexture_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _VisibleFogColor.rgb;
			float2 uv_VisibleRenderTexture = i.uv_texcoord * _VisibleRenderTexture_ST.xy + _VisibleRenderTexture_ST.zw;
			float4 tex2DNode3 = tex2D( _VisibleRenderTexture, uv_VisibleRenderTexture );
			float visibleGreyScale10 = ( 1.0 - ( ( tex2DNode3.r + tex2DNode3.g + tex2DNode3.b ) / 3.0 ) );
			float2 uv_ReavaledRenderTexture = i.uv_texcoord * _ReavaledRenderTexture_ST.xy + _ReavaledRenderTexture_ST.zw;
			float4 tex2DNode17 = tex2D( _ReavaledRenderTexture, uv_ReavaledRenderTexture );
			float revealedGreyScale31 = ( 1.0 - ( ( tex2DNode17.r + tex2DNode17.g + tex2DNode17.b ) / 3.0 ) );
			o.Alpha = ( visibleGreyScale10 * revealedGreyScale31 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
262;578;958;398;1619.325;66.39079;1.708696;True;True
Node;AmplifyShaderEditor.TexturePropertyNode;22;-1066.245,-502.1151;Float;True;Property;_ReavaledRenderTexture;Reavaled Render Texture;1;0;Create;True;0;0;False;0;415db34ea95895944a90477051d7f3f7;e93f1a3b0294efb4b9f655eeb7186220;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-1043.769,-794.9138;Float;True;Property;_VisibleRenderTexture;Visible Render Texture;0;0;Create;True;0;0;False;0;415db34ea95895944a90477051d7f3f7;415db34ea95895944a90477051d7f3f7;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;17;-721.0123,-516.5862;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-684.1692,-796.8138;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-359.3594,-340.9532;Float;False;Constant;_Float1;Float 1;1;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-323.7938,-483.3987;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-286.9507,-763.6263;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-322.5164,-621.1808;Float;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;20;-124.3595,-471.9532;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;7;-114.5164,-751.1808;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;12;36.5119,-752.394;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;32;38.63573,-458.2019;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;249.4962,-460.5883;Float;False;revealedGreyScale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;264.5984,-758.8741;Float;False;visibleGreyScale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-958.6179,270.9508;Float;False;31;revealedGreyScale;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;11;-1067.763,118.397;Float;False;10;visibleGreyScale;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;5;-1077.605,-134.204;Float;False;Property;_VisibleFogColor;Visible Fog Color;2;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-701.1291,148.0592;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;16;-219.5564,-166.6095;Float;False;True;6;Float;ASEMaterialInspector;0;0;Unlit;Leonidas Legacy/SH_fogOfWar;False;False;False;False;True;True;True;True;True;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;22;0
WireConnection;3;0;1;0
WireConnection;18;0;17;1
WireConnection;18;1;17;2
WireConnection;18;2;17;3
WireConnection;6;0;3;1
WireConnection;6;1;3;2
WireConnection;6;2;3;3
WireConnection;20;0;18;0
WireConnection;20;1;19;0
WireConnection;7;0;6;0
WireConnection;7;1;9;0
WireConnection;12;0;7;0
WireConnection;32;0;20;0
WireConnection;31;0;32;0
WireConnection;10;0;12;0
WireConnection;70;0;11;0
WireConnection;70;1;27;0
WireConnection;16;2;5;0
WireConnection;16;9;70;0
ASEEND*/
//CHKSM=1979E6361096919127E65A4A1F3D48538ACF88BC