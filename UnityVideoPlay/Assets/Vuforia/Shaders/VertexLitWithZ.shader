//==============================================================================
//Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
//All Rights Reserved.
//==============================================================================

Shader "Custom/Transparent" {
     Properties {
         _Color ("Color", Color) = (1,1,1,1)
         _MainTex ("Albedo (RGB)", 2D) = "white" {}
         _Glossiness ("Smoothness", Range(0,1)) = 0.5
         _Metallic ("Metallic", Range(0,1)) = 0.0
         _Num("Num",float) = 0.5 

    }

    SubShader {
         Tags { "Queue"="Transparent"  "RenderType"="Transparent"}
         LOD 200

        CGPROGRAM
     #pragma surface surf NoLighting alpha:auto
 
     fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
         {
             fixed4 c;
             c.rgb = s.Albedo;
             c.a = s.Alpha;
             return c;
         }
     float _Num;

        sampler2D _MainTex;

        struct Input {
             float2 uv_MainTex;
         };

        half _Glossiness;
         half _Metallic;
         fixed4 _Color;
         UNITY_INSTANCING_BUFFER_START(Props)
         UNITY_INSTANCING_BUFFER_END(Props)
     void surf (Input IN, inout SurfaceOutput o) 
         {
             o.Emission = tex2D(_MainTex, IN.uv_MainTex).rgb;            
             

             if (IN.uv_MainTex.x >= 0.5)
             {
                 o.Alpha = 0;
             }
             else
             {
                 o.Alpha = tex2D(_MainTex, float2(IN.uv_MainTex.x + 0.5, IN.uv_MainTex.y)).rgb;                        
             }

        }
         ENDCG
     }
     FallBack "Diffuse"
     }