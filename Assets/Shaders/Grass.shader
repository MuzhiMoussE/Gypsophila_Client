Shader "MyShader/Grass"
{
	Properties
	{
		[Header(Shading)]
		_TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		_TranslucentGain("Translucent Gain", Range(0,1)) = 0.5
			// 控制叶片弯曲程度
			_BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0.2
			// 草宽、草高控制
			_BladeWidth("Blade Width", Float) = 0.05
			_BladeWidthRandom("Blade Width Random", Float) = 0.02
			_BladeHeight("Blade Height", Float) = 0.5
			_BladeHeightRandom("Blade Height Random", Float) = 0.3
			_BladeForward("Blade Forward Amount", Float) = 0.38
			_BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2
			// 草密度控制（实际上就是控制曲面细分的细分程度的参数）
			_Density("Density", Range(1,64)) = 4
			// 风扰动图
			_WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
		// 风频
		_WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
			// 风强
			_WindStrength("Wind Strength", Float) = 1
	}

		CGINCLUDE
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "Autolight.cginc"
			//UnityDistanceBasedTess 函数在下面库中
#include "Tessellation.cginc"

#define MIN_DISTANCE 25
#define MAX_DISTANCE 35
#define BLADE_SEGMENTS 3

//草弯曲参数
float _BendRotationRandom;
		//草宽高参数
		float _BladeHeight;
		float _BladeHeightRandom;
		float _BladeWidth;
		float _BladeWidthRandom;
		float _BladeForward;
		float _BladeCurve;
		//密度参数
		float _Density;

		//风相关参数
		sampler2D _WindDistortionMap;
		float4 _WindDistortionMap_ST;
		float2 _WindFrequency;
		float _WindStrength;

		//传入顶点着色器的初始数据
		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
		};

		//顶点着色器传到曲面细分的数据
		struct a2t
		{
			float4 pos : SV_POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
		};

		//曲面细分着色器传到几何着色器的数据
		struct t2g
		{
			float4 pos : SV_POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
		};

		//几何着色器传到片元着色器的数据
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

		//在 (minDist,maxDist)区间内细分会不断变化
		float4 tessDistance(appdata v0, appdata v1, appdata v2) {
			float minDist = MIN_DISTANCE; //细分最小距离，小于细分不在增加
			float maxDist = MAX_DISTANCE; //细分最远距离，超出不在细分
			//这个函数计算每个顶点到相机的距离，得出最终的tessellation 因子。
			return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Density);
		}

		//曲面细分相关
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

		//指明输入进hull shader的图元是三角形。
		[UNITY_domain("tri")]
		//决定舍入规则，fractional_odd意为factor截断在[1,max]范围内，然后取整到小于此数的最大奇数整数值。
		[UNITY_partitioning("fractional_odd")]
		//决定图元的朝向，由组成三角形的三个顶点的顺序所产生的方向决定，cw为clockwise顺时针，ccw为counter clockwise逆时针。
		[UNITY_outputtopology("triangle_cw")]
		//指明计算factor的方法
		[UNITY_patchconstantfunc("hsconst_surf")]
		//hull shader输出的outputpatch中的顶点数量。
		[UNITY_outputcontrolpoints(3)]
		//给出控制点在path中的ID，与outputcontrolpoints对应，例如outputcontrolpoints为4，那么i的取值就是[0,4)的整数。
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
			//计算切线空间三轴
			float3 normal = input[ALIGN_POINT].normal;
			float4 tangent = input[ALIGN_POINT].tangent;
			float3 binormal = cross(normal, tangent) * tangent.w;

			//制造随机草宽、高
			float height = (rand(pos) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
			float width = (rand(pos) * 2 - 1) * _BladeWidthRandom + _BladeWidth;

			// 风扰动
			float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
			float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
			float3 wind = normalize(float3(windSample.x, windSample.y, 0));
			float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);

			// 这个矩阵还得研究一下 跟法线贴图那张联系一下 切线空间转模型空间
			float3x3 obect2TangentMatrix = float3x3(
				tangent.x, binormal.x, normal.x,
				tangent.y, binormal.y, normal.y,
				tangent.z, binormal.z, normal.z
				);

			g2f o1;
			g2f o2;
			g2f o3;

			//随机朝向 水平方向旋转
			float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
			//float3x3 transformationMatrix = mul(obect2TangentMatrix, facingRotationMatrix);

			//草弯曲矩阵
			float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));

			// float3x3 transformationMatrix = mul(mul(obect2TangentMatrix, facingRotationMatrix), bendRotationMatrix);
			float3x3 transformationMatrix = mul(mul(mul(obect2TangentMatrix, windRotation), facingRotationMatrix), bendRotationMatrix);
			//着重理解一下 为什么要同一个input 以及 float3(0.5, 0, 0) 是怎么设置顶点的 ，比如最后一个float3(0, 1, 0)改变1可以改变高度
			//uv的设置也要研究一下，是如何设定顶点和两个底的UV的
			//o1.pos = UnityObjectToClipPos(pos + mul(transformationMatrix, float3(width, 0, 0)));
			//o1.uv = float2(0, 0);
			//outStream.Append(GenerateGrassVertex(pos, width, 0, float2(0, 0), transformationMatrix));

			//o2.pos = UnityObjectToClipPos(pos + mul(transformationMatrix, float3(-width, 0, 0)));
			//o2.uv = float2(1, 0);
			//outStream.Append(GenerateGrassVertex(pos, -width, 0, float2(1, 0), transformationMatrix));

			//这里同样要研究一下
			//我们最初将我们的“向上”方向定义为Y轴；然而，在切线空间中，惯例通常指示向上的方向是沿着Z轴心，因此把坐标改为(0,0,1)
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
					// 判断内外表面
					float3 normal = facing > 0 ? i.normal : -i.normal;
					// 获取阴影
					float shadow = SHADOW_ATTENUATION(i);
					//半罗伯特反射
					float diffuse = (1 + dot(_WorldSpaceLightPos0, normal) + _TranslucentGain) / 2 * shadow;
					//float diffuse = saturate(saturate(dot(normal, _WorldSpaceLightPos0)) + _TranslucentGain) * shadow;
					//详看链接文章
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