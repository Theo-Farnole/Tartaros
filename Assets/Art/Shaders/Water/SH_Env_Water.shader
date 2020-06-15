// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Tartaros/Water"
{
	Properties
	{
		_wavespeed("wave speed", Float) = 1
		_wavetile("wavetile", Float) = 1
		_waveheight("wave height", Float) = 0.5
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_watercolorpur("watercolorpur", Color) = (0.02883589,0.157089,0.2264151,0)
		_topcolor("topcolor", Color) = (0.1559274,0.3769791,0.4528302,0)
		_edhedistance("edhe distance", Float) = 1
		_edgepower("edge power", Range( 0 , 1)) = 1
		_PanSpeed("PanSpeed", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha noshadow vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float _waveheight;
		uniform float _wavespeed;
		uniform float _wavetile;
		uniform sampler2D _TextureSample0;
		uniform float2 _PanSpeed;
		uniform float4 _watercolorpur;
		uniform float4 _topcolor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _edhedistance;
		uniform float _edgepower;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			float4 temp_cast_4 = (8.0).xxxx;
			return temp_cast_4;
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float temp_output_8_0 = ( _Time.y * _wavespeed );
			float2 _wavedirection = float2(-1,0);
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult12 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 worldspace13 = appendResult12;
			float4 WavetileUv24 = ( ( worldspace13 * float4( float2( 0.15,0.02 ), 0.0 , 0.0 ) ) * _wavetile );
			float2 panner3 = ( temp_output_8_0 * _wavedirection + WavetileUv24.xy);
			float simplePerlin2D1 = snoise( panner3 );
			float2 panner27 = ( temp_output_8_0 * _wavedirection + ( WavetileUv24 * float4( 0,0,0,0 ) ).xy);
			float simplePerlin2D28 = snoise( panner27 );
			float temp_output_30_0 = ( simplePerlin2D1 + simplePerlin2D28 );
			float2 waveheight36 = ( ( float2( 0,0.2 ) * _waveheight ) * temp_output_30_0 );
			v.vertex.xyz += float3( waveheight36 ,  0.0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 panner57 = ( 1.0 * _Time.y * _PanSpeed + i.uv_texcoord);
			o.Normal = tex2D( _TextureSample0, panner57 ).rgb;
			float temp_output_8_0 = ( _Time.y * _wavespeed );
			float2 _wavedirection = float2(-1,0);
			float3 ase_worldPos = i.worldPos;
			float4 appendResult12 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 worldspace13 = appendResult12;
			float4 WavetileUv24 = ( ( worldspace13 * float4( float2( 0.15,0.02 ), 0.0 , 0.0 ) ) * _wavetile );
			float2 panner3 = ( temp_output_8_0 * _wavedirection + WavetileUv24.xy);
			float simplePerlin2D1 = snoise( panner3 );
			float2 panner27 = ( temp_output_8_0 * _wavedirection + ( WavetileUv24 * float4( 0,0,0,0 ) ).xy);
			float simplePerlin2D28 = snoise( panner27 );
			float temp_output_30_0 = ( simplePerlin2D1 + simplePerlin2D28 );
			float wavepattern33 = temp_output_30_0;
			float clampResult45 = clamp( wavepattern33 , 0.0 , 1.0 );
			float4 lerpResult43 = lerp( _watercolorpur , _topcolor , clampResult45);
			float4 albedo46 = lerpResult43;
			o.Albedo = albedo46.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth49 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth49 = abs( ( screenDepth49 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _edhedistance ) );
			float clampResult56 = clamp( ( ( 1.0 - distanceDepth49 ) * _edgepower ) , 0.0 , 1.0 );
			float edge54 = clampResult56;
			float3 temp_cast_5 = (edge54).xxx;
			o.Emission = temp_cast_5;
			o.Smoothness = 0.9;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
-158;706;1906;718;1035.473;184.751;1;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;11;-4311.224,-63.16336;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;12;-4003.125,-46.26364;Float;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-3679.919,-123.3019;Float;True;worldspace;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;-3040.938,-408.9083;Float;True;13;worldspace;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;16;-2908.517,-259.308;Float;True;Constant;_wavestretch;wave stretch;2;0;Create;True;0;0;False;0;0.15,0.02;0.23,0.01;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-2597.009,-375.4017;Float;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-2591.518,-131.0586;Float;True;Property;_wavetile;wavetile;1;0;Create;True;0;0;False;0;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-2287.519,-321.0584;Float;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-2003.339,-320.4594;Float;True;WavetileUv;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-3089.016,2423.839;Float;True;24;WavetileUv;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-3185.939,2202.323;Float;True;Property;_wavespeed;wave speed;0;0;Create;True;0;0;False;0;1;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;7;-3197.9,1954.38;Float;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-2945.791,1943.181;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-2810.448,2427.182;Float;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-3414.715,1081.484;Float;True;24;WavetileUv;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;6;-3396.69,1475.386;Float;True;Constant;_wavedirection;wave direction;0;0;Create;True;0;0;False;0;-1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;27;-2624.267,1968.381;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;3;-2662.785,1406.047;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;1;-2591.721,1101.687;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;28;-2379.418,1681.892;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-2164.01,1276.717;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-2575.036,-998.1062;Float;True;Property;_edhedistance;edhe distance;6;0;Create;True;0;0;False;0;1;108.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;49;-2336.882,-985.6219;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-1768.007,1248.413;Float;True;wavepattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1754.332,-50.69797;Float;True;Property;_waveheight;wave height;2;0;Create;True;0;0;False;0;0.5;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;51;-2012.036,-979.1062;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;22;-1748.807,-344.0103;Float;True;Constant;_waveup;wave up;2;0;Create;True;0;0;False;0;0,0.2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;53;-2145.343,-713.9811;Float;True;Property;_edgepower;edge power;7;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;44;-508.4577,-777.7838;Float;True;33;wavepattern;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1461.607,-222.3462;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;42;-680.1888,-967.0157;Float;False;Property;_topcolor;topcolor;5;0;Create;True;0;0;False;0;0.1559274,0.3769791,0.4528302,0;0.2578763,0.4893439,0.5754716,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;41;-634.9231,-1167.007;Float;False;Property;_watercolorpur;watercolorpur;4;0;Create;True;0;0;False;0;0.02883589,0.157089,0.2264151,0;0.1624242,0.3656613,0.4716981,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-1734.236,-987.7061;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;45;-298.4956,-734.395;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;61;-610.4733,-192.251;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;56;-1501.964,-976.1556;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;43;-236.7807,-1000.1;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1165.438,-98.29785;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;60;-588.4733,-27.25098;Float;True;Property;_PanSpeed;PanSpeed;8;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;157.8794,-937.4745;Float;True;albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;57;-340.4733,-90.25098;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-861.567,-93.81518;Float;True;waveheight;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-1224.526,-973.8838;Float;True;edge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;142.2952,-288.5643;Float;True;46;albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-151.8964,685.3022;Float;True;Constant;_Tesselation;Tesselation;2;0;Create;True;0;0;False;0;8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-3104.245,1408.387;Float;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.1,0.1,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;39;-95.70459,-148.5383;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;ae063a40f83978546869efa4d46baf5e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-166.3731,219.3758;Float;True;Constant;_smoothness;smoothness;3;0;Create;True;0;0;False;0;0.9;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-516.6051,149.3117;Float;True;54;edge;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-166.1531,456.5612;Float;True;36;waveheight;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;500.2875,129.3519;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Tartaros/Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;11;1
WireConnection;12;1;11;3
WireConnection;13;0;12;0
WireConnection;15;0;14;0
WireConnection;15;1;16;0
WireConnection;18;0;15;0
WireConnection;18;1;19;0
WireConnection;24;0;18;0
WireConnection;8;0;7;0
WireConnection;8;1;9;0
WireConnection;32;0;31;0
WireConnection;27;0;32;0
WireConnection;27;2;6;0
WireConnection;27;1;8;0
WireConnection;3;0;25;0
WireConnection;3;2;6;0
WireConnection;3;1;8;0
WireConnection;1;0;3;0
WireConnection;28;0;27;0
WireConnection;30;0;1;0
WireConnection;30;1;28;0
WireConnection;49;0;50;0
WireConnection;33;0;30;0
WireConnection;51;0;49;0
WireConnection;23;0;22;0
WireConnection;23;1;34;0
WireConnection;52;0;51;0
WireConnection;52;1;53;0
WireConnection;45;0;44;0
WireConnection;56;0;52;0
WireConnection;43;0;41;0
WireConnection;43;1;42;0
WireConnection;43;2;45;0
WireConnection;35;0;23;0
WireConnection;35;1;30;0
WireConnection;46;0;43;0
WireConnection;57;0;61;0
WireConnection;57;2;60;0
WireConnection;36;0;35;0
WireConnection;54;0;56;0
WireConnection;29;0;25;0
WireConnection;39;1;57;0
WireConnection;0;0;48;0
WireConnection;0;1;39;0
WireConnection;0;2;55;0
WireConnection;0;4;38;0
WireConnection;0;11;37;0
WireConnection;0;14;20;0
ASEEND*/
//CHKSM=B0331F70657A883A9C4060115168B0A8D1AA4DD2