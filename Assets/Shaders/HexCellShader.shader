Shader "Akko/HexCellShader"
{
    Properties
    {
        [Header(Base)]
        _BaseColor("Base Color", Color) = (1,1,1,1)

        [Header(Ambient light)]
        _AmbientFactor("Ambient Factor",Range(0,1))=0.3
        
        [Header(Diffuse Factor)]
        _DiffuseFactor("Diffuse Factor",Range(1,10))=1


        [Header(Specular)]
        _SpecularStrength("Specular Strength", Range(0,1)) = 0.5
        _Shininess("Shininess", Range(1,128)) = 32

        [Header(Shadow)]
        _ShadowColor("Shader Color",Color)=(0,0,0,1)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque"
        }
        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _ShadowColor;
            
                float _AmbientFactor;

                float _DiffuseFactor;

                float _SpecularStrength;
                float _Shininess;

            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                // 世界空间位置 & normal
                float3 posWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normal);

                // 投影到裁剪空间
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionWS = posWS;
                output.normalWS = normalWS;

                // 阴影坐标（主光源阴影）
                output.shadowCoord = TransformWorldToShadowCoord(posWS);

                // 基础颜色
                output.color = input.color * _BaseColor;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // return half4(input.normalWS,1);
                // 主光源方向和颜色
                Light mainLight = GetMainLight(input.shadowCoord);

                // 阴影强度 (0=完全在阴影里, 1=完全不在阴影)
                half shadowAtten = mainLight.shadowAttenuation;

                // --- 环境光 ---
                half3 ambient = input.color.rgb * _BaseColor.rgb * _AmbientFactor;

                // --- 漫反射 ---
                float3 N = normalize(input.normalWS);
                float3 L = normalize(mainLight.direction);
                half NdotL = saturate(dot(N, L));
                NdotL=pow(NdotL,_DiffuseFactor);
                half3 diffuse = input.color.rgb * mainLight.color * NdotL;

                // --- 镜面反射 (Blinn-Phong) ---
                float3 V = normalize(GetWorldSpaceViewDir(input.positionWS)); // 视线方向
                float3 H = normalize(L + V); // 半角向量
                half NdotH = saturate(dot(N, H));
                half3 specular = pow(NdotH, _Shininess) * _SpecularStrength * mainLight.color.rgb;


                // --- 阴影混合 ---
                half3 litColor = ambient + (diffuse + specular) * shadowAtten;

                // 最终颜色
                return half4(litColor, 1);
            }
            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Universal Pipeline keywords

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ColorMask R
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

    }
}