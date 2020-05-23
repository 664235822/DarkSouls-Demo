// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Procedural Worlds/Simple Water"
{
	Properties
	{
		_GlobalTiling("Global Tiling", Range( 0 , 32768)) = 50
		_WaveSpeed("Wave Speed", Vector) = (-0.04,0.045,0,0)
		_SurfaceOpacity("Surface Opacity", Range( 0 , 1)) = 1
		_WaterNormal("Water Normal", 2D) = "white" {}
		_NormalScale("Normal Scale", Range( 0.025 , 2)) = 0.4
		_SurfaceColor("Surface Color", Color) = (0.4329584,0.5616246,0.6691177,1)
		_SurfaceColorBlend("Surface Color Blend", Range( 0 , 1)) = 0.9
		_WaterSpecular("Water Specular", Range( 0 , 1)) = 0.1
		_WaterSmoothness("Water Smoothness", Range( 0 , 1)) = 0.9
		_Distortion("Distortion", Range( 0 , 2)) = 0.2
		_FoamMap("Foam Map", 2D) = "white" {}
		_FoamTint("Foam Tint", Color) = (1,1,1,0)
		_FoamOpacity("Foam Opacity", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma surface surf StandardSpecular alpha:fade keepalpha 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float3 worldRefl;
			INTERNAL_DATA
			float3 worldNormal;
			float3 worldPos;
		};

		uniform sampler2D _WaterNormal;
		uniform float _NormalScale;
		uniform float2 _WaveSpeed;
		uniform float _GlobalTiling;
		uniform sampler2D _GrabTexture;
		uniform float _Distortion;
		uniform sampler2D _CameraDepthTexture;
		uniform float _SurfaceOpacity;
		uniform float4 _SurfaceColor;
		uniform float _SurfaceColorBlend;
		uniform float4 _FoamTint;
		uniform sampler2D _FoamMap;
		uniform float _FoamOpacity;
		uniform samplerCUBE SkyboxReflection;
		uniform float _WaterSpecular;
		uniform float _WaterSmoothness;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float temp_output_130_0_g47 = _NormalScale;
			float2 temp_output_137_0_g47 = _WaveSpeed;
			float temp_output_6_0_g47 = (temp_output_137_0_g47).x;
			float2 temp_cast_0 = (temp_output_6_0_g47).xx;
			float temp_output_132_0_g47 = _GlobalTiling;
			float _Tiling389_g47 = temp_output_132_0_g47;
			float2 temp_cast_1 = (_Tiling389_g47).xx;
			float2 temp_cast_2 = (0.15).xx;
			float2 uv_TexCoord11_g47 = i.uv_texcoord * temp_cast_1 + temp_cast_2;
			float2 panner17_g47 = ( 1.0 * _Time.y * temp_cast_0 + uv_TexCoord11_g47);
			float cos272_g47 = cos( temp_output_6_0_g47 );
			float sin272_g47 = sin( temp_output_6_0_g47 );
			float2 rotator272_g47 = mul( panner17_g47 - float2( 0.2,0 ) , float2x2( cos272_g47 , -sin272_g47 , sin272_g47 , cos272_g47 )) + float2( 0.2,0 );
			float2 temp_cast_3 = (( temp_output_132_0_g47 + 2.0 )).xx;
			float2 temp_cast_4 = (1.2).xx;
			float2 uv_TexCoord10_g47 = i.uv_texcoord * temp_cast_3 + temp_cast_4;
			float2 panner16_g47 = ( 1.0 * _Time.y * (( temp_output_137_0_g47 / 2.0 )).xy + uv_TexCoord10_g47);
			float3 _Normal25_g47 = BlendNormals( UnpackScaleNormal( tex2D( _WaterNormal, rotator272_g47 ), temp_output_130_0_g47 ) , UnpackScaleNormal( tex2D( _WaterNormal, panner16_g47 ), ( temp_output_130_0_g47 - 0.001 ) ) );
			o.Normal = _Normal25_g47;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor86_g47 = tex2D( _GrabTexture, ( float3( (ase_grabScreenPosNorm).xy ,  0.0 ) + ( _Normal25_g47 * _Distortion ) ).xy );
			float4 _DistortionDeep383_g47 = ( screenColor86_g47 * ( 1.0 - 1.5 ) );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float clampDepth20_g47 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPosNorm ))));
			float _ScreenPosition391_g47 = abs( ( clampDepth20_g47 - ase_screenPosNorm.w ) );
			float screenDepth555_g47 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth555_g47 = abs( ( screenDepth555_g47 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( saturate( pow( ( _ScreenPosition391_g47 + ( 1.0 - 1.0 ) ) , ( 1.0 - 1.0 ) ) ) ) );
			float _WaterDepth559_g47 = distanceDepth555_g47;
			float clampResult606_g47 = clamp( _WaterDepth559_g47 , 0.0 , 2.0 );
			float clampResult666 = clamp( 0.0 , 0.0 , 30.0 );
			float clampResult668 = clamp( 1.0 , 0.0 , 5.0 );
			float4 lerpResult583_g47 = lerp( ( float4(0.05201113,0.1105556,0.1911763,1) * ( 1.0 - 1.0 ) ) , _DistortionDeep383_g47 , ( 1.0 - saturate( pow( ( clampResult606_g47 + ( 1.0 - clampResult666 ) ) , ( 1.0 - clampResult668 ) ) ) ));
			float4 _DeepColor598_g47 = saturate( lerpResult583_g47 );
			float clampResult681 = clamp( _SurfaceOpacity , 0.5 , 1.0 );
			float temp_output_331_0_g47 = ( 1.0 - clampResult681 );
			float4 lerpResult657_g47 = lerp( _DeepColor598_g47 , _DistortionDeep383_g47 , temp_output_331_0_g47);
			float temp_output_625_0_g47 = _SurfaceColorBlend;
			float4 _DistortionShallow382_g47 = screenColor86_g47;
			float clampResult642_g47 = clamp( _WaterDepth559_g47 , 0.0 , 8.0 );
			float clampResult597 = clamp( 0.0 , 0.0 , 30.0 );
			float clampResult596 = clamp( 1.0 , 0.0 , 5.0 );
			float4 lerpResult617_g47 = lerp( ( _SurfaceColor * temp_output_625_0_g47 ) , _DistortionShallow382_g47 , ( 1.0 - saturate( pow( ( clampResult642_g47 + ( 1.0 - clampResult597 ) ) , ( 1.0 - clampResult596 ) ) ) ));
			float4 _ShallowColor634_g47 = saturate( lerpResult617_g47 );
			float4 lerpResult658_g47 = lerp( _ShallowColor634_g47 , _DistortionShallow382_g47 , temp_output_331_0_g47);
			float clampResult668_g47 = clamp( _WaterDepth559_g47 , 0.0 , 1.0 );
			float4 lerpResult654_g47 = lerp( lerpResult657_g47 , lerpResult658_g47 , abs( pow( clampResult668_g47 , ( 1.0 - 1.0 ) ) ));
			float4 clampResult692_g47 = clamp( lerpResult654_g47 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float clampResult610 = clamp( 3.5 , 2.0 , 4.0 );
			float clampResult562_g47 = clamp( saturate( pow( ( _WaterDepth559_g47 + ( 1.0 - 0.4941176 ) ) , ( 1.0 - clampResult610 ) ) ) , 0.0 , 3.0 );
			float2 temp_cast_7 = (( _Tiling389_g47 * 2.1 )).xx;
			float2 uv_TexCoord45_g47 = i.uv_texcoord * temp_cast_7;
			float2 panner49_g47 = ( 1.0 * _Time.y * float2( -0.01,0.01 ) + uv_TexCoord45_g47);
			float4 clampResult663_g47 = clamp( ( clampResult562_g47 * _FoamTint * tex2D( _FoamMap, panner49_g47 ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 lerpResult328_g47 = lerp( clampResult663_g47 , float4( 0,0,0,0 ) , ( 1.0 - _FoamOpacity ));
			float4 _FoamAlbedo314_g47 = saturate( lerpResult328_g47 );
			float4 _Albedo105_g47 = saturate( ( clampResult692_g47 + _FoamAlbedo314_g47 ) );
			o.Albedo = _Albedo105_g47.rgb;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult35_g47 = dot( (WorldNormalVector( i , _Normal25_g47 )) , ase_worldlightDir );
			float temp_output_116_0_g47 = 1.0;
			float _FakeShadow99_g47 = ( 1.0 - saturate( (dotResult35_g47*temp_output_116_0_g47 + temp_output_116_0_g47) ) );
			float4 lerpResult143_g47 = lerp( texCUBE( SkyboxReflection, WorldReflectionVector( i , _Normal25_g47 ) ) , float4( 1,1,1,0 ) , _FakeShadow99_g47);
			float4 _Reflections95_g47 = saturate( lerpResult143_g47 );
			float clampResult310 = clamp( ( 1.0 - 1.0 ) , 0.0 , 1.0 );
			o.Emission = ( _Reflections95_g47 * clampResult310 ).rgb;
			float clampResult231_g47 = clamp( _WaterSpecular , 0.0 , 0.05 );
			float clampResult232_g47 = clamp( 1.0 , 0.0 , 0.2 );
			float lerpResult97_g47 = lerp( clampResult231_g47 , clampResult232_g47 , _FoamAlbedo314_g47.r);
			float _Specular104_g47 = lerpResult97_g47;
			float3 temp_cast_11 = (_Specular104_g47).xxx;
			o.Specular = temp_cast_11;
			float clampResult402_g47 = clamp( _WaterSmoothness , 0.0 , 0.99 );
			float lerpResult96_g47 = lerp( clampResult402_g47 , 1.0 , _FoamAlbedo314_g47.r);
			float _Smoothness98_g47 = lerpResult96_g47;
			o.Smoothness = _Smoothness98_g47;
			float _AmbientOcclusion672_g47 = 1.0;
			o.Occlusion = _AmbientOcclusion672_g47;
			float clampResult556_g47 = clamp( distanceDepth555_g47 , 0.0 , 1.0 );
			float _EdgeBlend490_g47 = clampResult556_g47;
			o.Alpha = _EdgeBlend490_g47;
		}

		ENDCG
	}
	Fallback "Standard"
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
7.2;0.8;1010;752;1502.323;644.5295;1.805161;True;False
Node;AmplifyShaderEditor.CommentaryNode;265;-1541.605,-1234.942;Float;False;1808.274;3302.058;Main Setup;38;0;302;310;38;47;51;34;539;75;666;426;79;596;230;469;597;351;245;53;70;629;69;261;681;668;73;185;670;262;476;610;39;667;44;665;435;190;439;Main Setup;0,0.5448275,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;439;-1511.87,11.1601;Float;False;Constant;_ShallowDepth;Shallow Depth;9;0;Create;True;0;0;False;0;0;30;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-938.8583,289.8095;Float;False;Constant;_ReflectionAmount;Reflection Amount;21;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;435;-1505.986,-220.1141;Float;False;Property;_SurfaceOpacity;Surface Opacity;2;0;Create;True;0;0;False;0;1;0.97;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;665;-1515.244,125.6636;Float;False;Constant;_DeepDepth;Deep Depth;11;0;Create;True;0;0;False;0;0;30;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1511.087,327.8101;Float;False;Constant;_ShallowFalloff;Shallow Falloff;10;0;Create;True;0;0;False;0;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;667;-1509.707,450.4643;Float;False;Constant;_DeepFalloff;Deep Falloff;13;0;Create;True;0;0;False;0;1;5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1522.381,1679.864;Float;False;Constant;_FoamFalloff;Foam Falloff;17;0;Create;True;0;0;False;0;3.5;3.5;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;469;-1516.762,-1059.099;Float;False;Property;_SurfaceColorBlend;Surface Color Blend;6;0;Create;True;0;0;False;0;0.9;0.6;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;230;-1526.817,1967.628;Float;False;Constant;_ShadowDepth;Shadow Depth;19;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;596;-1215.514,282.2962;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-1517.524,-949.1722;Float;False;Property;_GlobalTiling;Global Tiling;0;0;Create;True;0;0;False;0;50;1024;0;32768;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;426;-1522.069,1314.222;Float;False;Property;_FoamOpacity;Foam Opacity;12;0;Create;True;0;0;False;0;1;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;666;-1221.596,62.85124;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;30;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;51;-1488.671,-586.8487;Float;False;Property;_SurfaceColor;Surface Color;5;0;Create;True;0;0;False;0;0.4329584,0.5616246,0.6691177,1;0.1333333,0.2784314,0.3960784,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;539;-1515.507,226.6118;Float;False;Constant;_ShorelineAmount;Shoreline Amount;24;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1496.335,-671.2239;Float;False;Property;_NormalScale;Normal Scale;4;0;Create;True;0;0;False;0;0.4;0.3;0.025;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;597;-1218.222,-51.65228;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;30;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;47;-1491.281,-415.8536;Float;False;Constant;_DeepColor;Deep Color;9;0;Create;True;0;0;False;0;0.05201113,0.1105556,0.1911763,1;0.05201113,0.1105556,0.1911763,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-1526.971,1405.014;Float;False;Constant;_FoamDepth;Foam Depth;19;0;Create;True;0;0;False;0;0.4941176;0.4;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-1515.994,1584.157;Float;False;Constant;_FoamSmoothness;Foam Smoothness;18;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;351;-1520.862,1134.512;Float;False;Property;_FoamTint;Foam Tint;11;0;Create;True;0;0;False;0;1,1,1,0;0.4392157,0.4392157,0.4392157,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;70;-1524.733,638.6342;Float;False;Property;_WaterSpecular;Water Specular;7;0;Create;True;0;0;False;0;0.1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-1523.442,838.1867;Float;False;Property;_Distortion;Distortion;9;0;Create;True;0;0;False;0;0.2;0.2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;610;-1202.584,1632.066;Float;False;3;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;476;-1518.762,-1160.099;Float;False;Constant;_DeepColorBlend;Deep Color Blend;9;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;262;-1494.886,-865.0838;Float;True;Property;_WaterNormal;Water Normal;3;0;Create;True;0;0;False;0;None;dd2fd2df93418444c8e280f1d34deeb5;True;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;670;-1517.476,733.6425;Float;False;Constant;_ColorBlendingAmount;Color Blending Amount;11;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;245;-1478.323,-119.3994;Float;False;Property;_WaveSpeed;Wave Speed;1;0;Create;True;0;0;False;0;-0.04,0.045;0.1,-0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;73;-1525.754,1492.516;Float;False;Constant;_FoamSpecular;Foam Specular;17;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;185;-1528.364,1773.411;Float;True;Global;SkyboxReflection;Skybox Reflection;13;0;Create;True;0;0;False;0;None;None;False;white;LockedToCube;Cube;0;1;SAMPLERCUBE;0
Node;AmplifyShaderEditor.ClampOpNode;681;-1179.958,-263.3964;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;261;-1514.407,931.3614;Float;True;Property;_FoamMap;Foam Map;10;0;Create;True;0;0;False;0;None;d01457b88b1c5174ea4235d140b5fab8;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-1524.101,557.4289;Float;False;Property;_WaterSmoothness;Water Smoothness;8;0;Create;True;0;0;False;0;0.9;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;629;-595.9492,299.3329;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;668;-1211.534,437.4506;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;735;-950.627,-469.7546;Float;False;Simple Water Sample;-1;;47;f2f2c9b193fabb4458e37ad6b3ec204c;0;27;595;FLOAT;0;False;625;FLOAT;0;False;132;FLOAT;256;False;131;SAMPLER2D;;False;130;FLOAT;0.2;False;628;COLOR;0.4329584,0.5616246,0.6691177,1;False;611;COLOR;0.05201113,0.1105556,0.1911763,1;False;330;FLOAT;0;False;137;FLOAT2;-0.04,0.045;False;630;FLOAT;0;False;594;FLOAT;0;False;558;FLOAT;1;False;626;FLOAT;0;False;590;FLOAT;0;False;122;FLOAT;0.97;False;123;FLOAT;1;False;655;FLOAT;0;False;119;FLOAT;0.2;False;138;SAMPLER2D;;False;234;COLOR;0.4313726,0.4313726,0.4313726,0;False;320;FLOAT;0.5;False;124;FLOAT;2.8;False;125;FLOAT;2;False;120;FLOAT;1;False;121;FLOAT;1;False;117;SAMPLERCUBE;;False;116;FLOAT;0.025;False;7;COLOR;0;FLOAT3;106;COLOR;107;FLOAT;108;FLOAT;109;FLOAT;674;FLOAT;146
Node;AmplifyShaderEditor.ClampOpNode;310;-369.1227,-26.34986;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;302;-199.3212,-116.4392;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4.082899,-352.3193;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;Procedural Worlds/Simple Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;False;True;True;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Standard;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;596;0;44;0
WireConnection;666;0;665;0
WireConnection;597;0;439;0
WireConnection;610;0;39;0
WireConnection;681;0;435;0
WireConnection;629;0;190;0
WireConnection;668;0;667;0
WireConnection;735;595;476;0
WireConnection;735;625;469;0
WireConnection;735;132;79;0
WireConnection;735;131;262;0
WireConnection;735;130;34;0
WireConnection;735;628;51;0
WireConnection;735;611;47;0
WireConnection;735;330;681;0
WireConnection;735;137;245;0
WireConnection;735;630;597;0
WireConnection;735;594;666;0
WireConnection;735;558;539;0
WireConnection;735;626;596;0
WireConnection;735;590;668;0
WireConnection;735;122;69;0
WireConnection;735;123;70;0
WireConnection;735;655;670;0
WireConnection;735;119;53;0
WireConnection;735;138;261;0
WireConnection;735;234;351;0
WireConnection;735;320;426;0
WireConnection;735;124;38;0
WireConnection;735;125;610;0
WireConnection;735;120;73;0
WireConnection;735;121;75;0
WireConnection;735;117;185;0
WireConnection;735;116;230;0
WireConnection;310;0;629;0
WireConnection;302;0;735;107
WireConnection;302;1;310;0
WireConnection;0;0;735;0
WireConnection;0;1;735;106
WireConnection;0;2;302;0
WireConnection;0;3;735;108
WireConnection;0;4;735;109
WireConnection;0;5;735;674
WireConnection;0;9;735;146
ASEEND*/
//CHKSM=57DC3B8690E22517B75A7A77A52209AFFD69A1C0