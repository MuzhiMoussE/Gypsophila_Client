Shader "MyShader/Grass"
{
	Properties
	{
		[Header(Shading)]
		_TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		_TranslucentGain("Translucent Gain", Range(0,1)) = 0.5
			// ����ҶƬ�����̶�
			_BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0.2
			// �ݿ��ݸ߿���
			_BladeWidth("Blade Width", Float) = 0.05
			_BladeWidthRandom("Blade Width Random", Float) = 0.02
			_BladeHeight("Blade Height", Float) = 0.5
			_BladeHeightRandom("Blade Height Random", Float) = 0.3
			_BladeForward("Blade Forward Amount", Float) = 0.38
			_BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2
			// ���ܶȿ��ƣ�ʵ���Ͼ��ǿ�������ϸ�ֵ�ϸ�̶ֳȵĲ�����
			_Density("Density", Range(1,64)) = 4
			// ���Ŷ�ͼ
			_WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
		// ��Ƶ
		_WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
			// ��ǿ
			_WindStrength("Wind Strength", Float) = 1
	}

		CGINCLUDE
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "Autolight.cginc"
			//UnityDistanceBasedTess �������������
#include "Tessellation.cginc"

#define MIN_DISTANCE 25
#define MAX_DISTANCE 35
#define BLADE_SEGMENTS 3

//����������
float _BendRotationRandom;
		//�ݿ�߲���
		float _BladeHeight;
		float _BladeHeightRandom;
		float _BladeWidth;
		float _BladeWidthRandom;
		float _BladeForward;
		float _BladeCurve;
		//�ܶȲ���
		float _Density;

		//����ز���
		sampler2D _WindDistortionMap;
		float4 _WindDistortionMap_ST;
		float2 _WindFrequency;
		float _WindStrength;

		//���붥����ɫ���ĳ�ʼ����
		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
		};

		//������ɫ����������ϸ�ֵ�����
		struct a2t
		{
			float4 pos : SV_POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
		};

		//����ϸ����ɫ������������ɫ��������
		struct t2g
		{
			float4 pos : SV_POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
		};

		//������ɫ������ƬԪ��ɫ��������
		struct g2f
		{
			float4 pos : SV_POSITION;
#if UNITY_PASS_FORWARDBASE		
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
			// unityShadowCoord4 is defined as a float4 in UnityShadowLibrary.cginc.
			unityShadowCoord4 _ShadowCoord : TEXCOORD1;
#endif
		};

		//�� (minDist,maxDist)������ϸ�ֻ᲻�ϱ仯
		float4 tessDistance(appdata v0, appdata v1, appdata v2) {
			float minDist = MIN_DISTANCE; //ϸ����С���룬С��ϸ�ֲ�������
			float maxDist = MAX_DISTANCE; //ϸ����Զ���룬��������ϸ��
			//�����������ÿ�����㵽����ľ��룬�ó����յ�tessellation ���ӡ�
			return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Density);
		}

		//����ϸ�����
		UnityTessellationFactors hsconst_surf(InputPatch<a2t, 3> v)
		{
			UnityTessellationFactors o;
			float4 tf;
			appdata vi[3];
			vi[0].vertex = v[0].pos;
			vi[0].tangent = v[0].tangent;
			vi[0].normal = v[0].normal;
			vi[1].vertex = v[1].pos;
			vi[1].tangent = v[1].tangent;
			vi[1].normal = v[1].normal;
			vi[2].vertex = v[2].pos;
			vi[2].tangent = v[2].tangent;
			vi[2].normal = v[2].normal;
			tf = tessDistance(vi[0], vi[1], vi[2]);
			o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
			return o;
		}

		//ָ�������hull shader��ͼԪ�������Ρ�
		[UNITY_domain("tri")]
		//�����������fractional_odd��Ϊfactor�ض���[1,max]��Χ�ڣ�Ȼ��ȡ����С�ڴ����������������ֵ��
		[UNITY_partitioning("fractional_odd")]
		//����ͼԪ�ĳ�������������ε����������˳���������ķ��������cwΪclockwise˳ʱ�룬ccwΪcounter clockwise��ʱ�롣
		[UNITY_outputtopology("triangle_cw")]
		//ָ������factor�ķ���
		[UNITY_patchconstantfunc("hsconst_surf")]
		//hull shader�����outputpatch�еĶ���������
		[UNITY_outputcontrolpoints(3)]
		//�������Ƶ���path�е�ID����outputcontrolpoints��Ӧ������outputcontrolpointsΪ4����ôi��ȡֵ����[0,4)��������
		a2t hs_surf(InputPatch<a2t, 3> v, uint id : SV_OutputControlPointID) {
			return v[id];
		}

		[UNITY_domain("tri")]
		t2g ds_surf(UnityTessellationFactors tessFactors, const OutputPatch<a2t, 3> vi, float3 bary : SV_DomainLocation) {
			appdata v;
			UNITY_INITIALIZE_OUTPUT(appdata, v);
			v.vertex = vi[0].pos * bary.x + vi[1].pos * bary.y + vi[2].pos * bary.z;
			v.tangent = vi[0].tangent * bary.x + vi[1].tangent * bary.y + vi[2].tangent * bary.z;
			v.normal = vi[0].normal * bary.x + vi[1].normal * bary.y + vi[2].normal * bary.z;

			t2g o;
			o.pos = v.vertex;
			o.tangent = v.tangent;
			o.normal = v.normal;
			return o;
		}

		// Simple noise function, sourced from http://answers.unity.com/answers/624136/view.html
		// Extended discussion on this function can be found at the following link:
		// https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/#post-2949326
		// Returns a number in the 0...1 range.
		float rand(float3 co)
		{
			return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
		}

		// Construct a rotation matrix that rotates around the provided axis, sourced from:
		// https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
		float3x3 AngleAxis3x3(float angle, float3 axis)
		{
			float c, s;
			sincos(angle, s, c);

			float t = 1 - c;
			float x = axis.x;
			float y = axis.y;
			float z = axis.z;

			return float3x3(
				t * x * x + c, t * x * y - s * z, t * x * z + s * y,
				t * x * y + s * z, t * y * y + c, t * y * z - s * x,
				t * x * z - s * y, t * y * z + s * x, t * z * z + c
				);
		}

		a2t vert(appdata v)
		{
			a2t t;
			t.pos = v.vertex;
			t.normal = v.normal;
			t.tangent = v.tangent;
			return t;
		}

		/*g2f VertexOutput(float3 pos,float2 uv)
		{
			g2f o;
			o.pos = UnityObjectToClipPos(pos);
			o.uv = uv;
			return o;
		}*/

		g2f GenerateGrassVertex(float3 vertexPosition, float width, float height, float forward, float2 uv, float3x3 transformMatrix)
		{
			g2f o;
			float3 tangentPoint = float3(width, forward, height);
			float3 pos = vertexPosition + mul(transformMatrix, tangentPoint);
			o.pos = UnityObjectToClipPos(pos);
			//o.uv = uv; 
#if UNITY_PASS_FORWARDBASE
			float3 tangentNormal = normalize(float3(0, -1, forward));
			float3 normal = mul(transformMatrix, tangentNormal);
			o.normal = UnityObjectToWorldNormal(normal);
			o.uv = uv;
			// Shadows are sampled from a screen-space shadow map texture.
			o._ShadowCoord = ComputeScreenPos(o.pos);
#elif UNITY_PASS_SHADOWCASTER
			// Applying the bias prevents artifacts from appearing on the surface.
			o.pos = UnityApplyLinearShadowBias(o.pos);
#endif
			return o;
		}

#define ALIGN_POINT 2
		[maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
		void geom(triangle t2g input[3], inout TriangleStream<g2f> outStream) {

			float3 pos = input[ALIGN_POINT].pos;
			//�������߿ռ�����
			float3 normal = input[ALIGN_POINT].normal;
			float4 tangent = input[ALIGN_POINT].tangent;
			float3 binormal = cross(normal, tangent) * tangent.w;

			//��������ݿ���
			float height = (rand(pos) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
			float width = (rand(pos) * 2 - 1) * _BladeWidthRandom + _BladeWidth;

			// ���Ŷ�
			float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
			float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
			float3 wind = normalize(float3(windSample.x, windSample.y, 0));
			float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);

			// ������󻹵��о�һ�� ��������ͼ������ϵһ�� ���߿ռ�תģ�Ϳռ�
			float3x3 obect2TangentMatrix = float3x3(
				tangent.x, binormal.x, normal.x,
				tangent.y, binormal.y, normal.y,
				tangent.z, binormal.z, normal.z
				);

			g2f o1;
			g2f o2;
			g2f o3;

			//������� ˮƽ������ת
			float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
			//float3x3 transformationMatrix = mul(obect2TangentMatrix, facingRotationMatrix);

			//����������
			float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));

			// float3x3 transformationMatrix = mul(mul(obect2TangentMatrix, facingRotationMatrix), bendRotationMatrix);
			float3x3 transformationMatrix = mul(mul(mul(obect2TangentMatrix, windRotation), facingRotationMatrix), bendRotationMatrix);
			//�������һ�� ΪʲôҪͬһ��input �Լ� float3(0.5, 0, 0) ����ô���ö���� ���������һ��float3(0, 1, 0)�ı�1���Ըı�߶�
			//uv������ҲҪ�о�һ�£�������趨����������׵�UV��
			//o1.pos = UnityObjectToClipPos(pos + mul(transformationMatrix, float3(width, 0, 0)));
			//o1.uv = float2(0, 0);
			//outStream.Append(GenerateGrassVertex(pos, width, 0, float2(0, 0), transformationMatrix));

			//o2.pos = UnityObjectToClipPos(pos + mul(transformationMatrix, float3(-width, 0, 0)));
			//o2.uv = float2(1, 0);
			//outStream.Append(GenerateGrassVertex(pos, -width, 0, float2(1, 0), transformationMatrix));

			//����ͬ��Ҫ�о�һ��
			//������������ǵġ����ϡ�������ΪY�᣻Ȼ���������߿ռ��У�����ͨ��ָʾ���ϵķ���������Z���ģ���˰������Ϊ(0,0,1)
			//o3.pos = UnityObjectToClipPos(pos + mul(transformationMatrix, float3(0, 0, height)));
			//o3.uv = float2(0.5, 1);
			//outStream.Append(GenerateGrassVertex(pos, 0, height, float2(0.5, 1), transformationMatrix));

			float forward = rand(pos.yyz) * _BladeForward;

			for (int i = 0; i < BLADE_SEGMENTS; i++)
			{
				float t = i / (float)BLADE_SEGMENTS;

				float segmentHeight = height * t;
				float segmentWidth = width * (1 - t);
				float segmentForward = pow(t, _BladeCurve) * forward;

				// Select the facing-only transformation matrix for the root of the blade.
				//float3x3 transformMatrix = i == 0 ? transformationMatrixFacing : transformationMatrix;

				outStream.Append(GenerateGrassVertex(pos, segmentWidth, segmentHeight, segmentForward, float2(0, t), transformationMatrix));
				outStream.Append(GenerateGrassVertex(pos, -segmentWidth, segmentHeight, segmentForward, float2(1, t), transformationMatrix));
			}

			// Add the final vertex as the tip.
			outStream.Append(GenerateGrassVertex(pos, 0, height, forward, float2(0.5, 1), transformationMatrix));
		}
		ENDCG

			SubShader
		{
			Cull Off

			Pass
			{
				Tags
				{
					"RenderType" = "Opaque"
					"LightMode" = "ForwardBase"
				}

				CGPROGRAM
				#pragma vertex vert
				#pragma hull hs_surf
				#pragma domain ds_surf
				#pragma geometry geom
				#pragma fragment frag
				#pragma target 4.6
				#pragma multi_compile_fwdbase
				#include "Lighting.cginc"

				float4 _TopColor;
				float4 _BottomColor;
				float _TranslucentGain;


				/*float4 frag(g2f i) : SV_Target
				{
					float shadow = SHADOW_ATTENUATION(i);
					return shadow * lerp(_BottomColor, _TopColor, i.uv.y);
				}*/
				float4 frag(g2f i, fixed facing : VFACE) : SV_Target
				{
					// �ж��������
					float3 normal = facing > 0 ? i.normal : -i.normal;
					// ��ȡ��Ӱ
					float shadow = SHADOW_ATTENUATION(i);
					//���޲��ط���
					float diffuse = (1 + dot(_WorldSpaceLightPos0, normal) + _TranslucentGain) / 2 * shadow;
					//float diffuse = saturate(saturate(dot(normal, _WorldSpaceLightPos0)) + _TranslucentGain) * shadow;
					//�꿴��������
					float3 ambient = ShadeSH9(float4(normal, 1));
					float4 lightIntensity = diffuse * _LightColor0 + float4(ambient, 1);
					float4 col = lerp(_BottomColor, _TopColor * lightIntensity, i.uv.y);

					return col;
				}
				ENDCG
			}
			Pass
			{
				Tags
				{
					"LightMode" = "ShadowCaster"
				}

				CGPROGRAM
				#pragma vertex vert
				#pragma hull hs_surf
				#pragma domain ds_surf
				#pragma geometry geom
				#pragma fragment frag
				#pragma target 4.6
				#pragma multi_compile_shadowcaster

				float4 frag(g2f i) : SV_Target
				{
					SHADOW_CASTER_FRAGMENT(i)
				}

				ENDCG
			}
		}
}