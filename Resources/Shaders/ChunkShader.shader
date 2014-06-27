// Shader created with Shader Forge Beta 0.30 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.30;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.100346,fgcg:0.2561031,fgcb:0.4705882,fgca:1,fgde:0.005,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-2-RGB,emission-3013-OUT,amdfl-20-RGB;n:type:ShaderForge.SFN_Tex2d,id:2,x:33482,y:32622,ptlb:TileSheet,ptin:_TileSheet,tex:c68b860f81c28394b91ef9b081431936,ntxv:0,isnm:False;n:type:ShaderForge.SFN_AmbientLight,id:20,x:33004,y:32924;n:type:ShaderForge.SFN_TexCoord,id:398,x:34412,y:32949,uv:0;n:type:ShaderForge.SFN_Code,id:518,x:33526,y:33157,code:aQBmACgAYgBsAG8AYwBrAFgAIAA9AD0AIAAtADEAIAB8AHwAIABiAGwAbwBjAGsAWQAgAD0APQAgAC0AMQApAAoAcgBlAHQAdQByAG4AIAAwADsACgAKAGYAbABvAGEAdAAgAHcAaQB0AGgAaQBuAFAAbwBzACAAPQAgADEALgAwADsACgBmAGwAbwBhAHQAMgAgAGIAbABvAGMAawBNAGkAbgBQAG8AcwAgAD0AIABmAGwAbwBhAHQAMgAoAGIAYQBzAGUAVgBhAGwAdQBlACAAKgAgAGIAbABvAGMAawBYACwAIABiAGEAcwBlAFYAYQBsAHUAZQAgACoAIABiAGwAbwBjAGsAWQApADsACgBiAGwAbwBjAGsAWAAgACsAPQAgADEAOwAKAGIAbABvAGMAawBZACAAKwA9ACAAMQA7AAoAZgBsAG8AYQB0ADIAIABiAGwAbwBjAGsATQBhAHgAUABvAHMAIAA9ACAAIABmAGwAbwBhAHQAMgAoAGIAYQBzAGUAVgBhAGwAdQBlACAAKgAgAGIAbABvAGMAawBYACwAIABiAGEAcwBlAFYAYQBsAHUAZQAgACoAIABiAGwAbwBjAGsAWQApADsACgAKAGkAZgAoAHYAYQBsAHUAZQAuAHgAIAA8ACAAYgBsAG8AYwBrAE0AaQBuAFAAbwBzAC4AeAApAAoAdwBpAHQAaABpAG4AUABvAHMAIAA9ACAAMAAuADAAOwAKAGkAZgAoAHYAYQBsAHUAZQAuAHkAIAA8ACAAYgBsAG8AYwBrAE0AaQBuAFAAbwBzAC4AeQApAAoAdwBpAHQAaABpAG4AUABvAHMAIAA9ACAAMAAuADAAOwAKAGkAZgAoAHYAYQBsAHUAZQAuAHgAIAA+ACAAYgBsAG8AYwBrAE0AYQB4AFAAbwBzAC4AeAApAAoAdwBpAHQAaABpAG4AUABvAHMAIAA9ACAAMAAuADAAOwAKAGkAZgAoAHYAYQBsAHUAZQAuAHkAIAA+ACAAYgBsAG8AYwBrAE0AYQB4AFAAbwBzAC4AeQApAAoAdwBpAHQAaABpAG4AUABvAHMAIAA9ACAAMAAuADAAOwAKAAoAcgBlAHQAdQByAG4AIAB3AGkAdABoAGkAbgBQAG8AcwA7AAoA,output:0,fname:GetBlockUV,width:596,height:292,input:1,input:0,input:0,input:0,input_1_label:value,input_2_label:blockX,input_3_label:blockY,input_4_label:baseValue|A-398-UVOUT,B-2956-OUT,C-2957-OUT,D-2929-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2925,x:34572,y:33375,ptlb:TileSheetSize,ptin:_TileSheetSize,glob:False,v1:16;n:type:ShaderForge.SFN_Vector1,id:2927,x:34541,y:33129,v1:1;n:type:ShaderForge.SFN_Divide,id:2929,x:34321,y:33225,cmnt:Base tile Size|A-2927-OUT,B-2925-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2956,x:34320,y:33491,ptlb:BlockX,ptin:_BlockX,glob:False,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:2957,x:34320,y:33683,ptlb:BlockY,ptin:_BlockY,glob:False,v1:15;n:type:ShaderForge.SFN_Multiply,id:3013,x:33175,y:32800|A-518-OUT,B-3014-OUT;n:type:ShaderForge.SFN_Vector1,id:3014,x:33359,y:32904,v1:0.25;proporder:2-2925-2956-2957;pass:END;sub:END;*/

Shader "Shader Forge/ChunkShader" {
    Properties {
        _TileSheet ("TileSheet", 2D) = "white" {}
        _TileSheetSize ("TileSheetSize", Float ) = 16
        _BlockX ("BlockX", Float ) = 2
        _BlockY ("BlockY", Float ) = 15
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _TileSheet; uniform float4 _TileSheet_ST;
            float GetBlockUV( float2 value , float blockX , float blockY , float baseValue ){
            if(blockX == -1 || blockY == -1)
            return 0;
            
            float withinPos = 1.0;
            float2 blockMinPos = float2(baseValue * blockX, baseValue * blockY);
            blockX += 1;
            blockY += 1;
            float2 blockMaxPos =  float2(baseValue * blockX, baseValue * blockY);
            
            if(value.x < blockMinPos.x)
            withinPos = 0.0;
            if(value.y < blockMinPos.y)
            withinPos = 0.0;
            if(value.x > blockMaxPos.x)
            withinPos = 0.0;
            if(value.y > blockMaxPos.y)
            withinPos = 0.0;
            
            return withinPos;
            
            }
            
            uniform float _TileSheetSize;
            uniform float _BlockX;
            uniform float _BlockY;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
////// Emissive:
                float node_3013 = (GetBlockUV( i.uv0.rg , _BlockX , _BlockY , (1.0/_TileSheetSize) )*0.25);
                float3 emissive = float3(node_3013,node_3013,node_3013);
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                diffuseLight += UNITY_LIGHTMODEL_AMBIENT.rgb; // Diffuse Ambient Light
                float2 node_3026 = i.uv0;
                finalColor += diffuseLight * tex2D(_TileSheet,TRANSFORM_TEX(node_3026.rg, _TileSheet)).rgb;
                finalColor += emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _TileSheet; uniform float4 _TileSheet_ST;
            float GetBlockUV( float2 value , float blockX , float blockY , float baseValue ){
            if(blockX == -1 || blockY == -1)
            return 0;
            
            float withinPos = 1.0;
            float2 blockMinPos = float2(baseValue * blockX, baseValue * blockY);
            blockX += 1;
            blockY += 1;
            float2 blockMaxPos =  float2(baseValue * blockX, baseValue * blockY);
            
            if(value.x < blockMinPos.x)
            withinPos = 0.0;
            if(value.y < blockMinPos.y)
            withinPos = 0.0;
            if(value.x > blockMaxPos.x)
            withinPos = 0.0;
            if(value.y > blockMaxPos.y)
            withinPos = 0.0;
            
            return withinPos;
            
            }
            
            uniform float _TileSheetSize;
            uniform float _BlockX;
            uniform float _BlockY;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_3027 = i.uv0;
                finalColor += diffuseLight * tex2D(_TileSheet,TRANSFORM_TEX(node_3027.rg, _TileSheet)).rgb;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
