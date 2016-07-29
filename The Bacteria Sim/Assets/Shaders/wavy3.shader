// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32911,y:32692,varname:node_3138,prsc:2|emission-5907-OUT;n:type:ShaderForge.SFN_Tex2d,id:1383,x:32486,y:32798,ptovrint:False,ptlb:node_1383,ptin:_node_1383,varname:node_1383,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8a10d830bac4ba94881a5e5570993d8e,ntxv:0,isnm:False|UVIN-8201-OUT;n:type:ShaderForge.SFN_Lerp,id:8201,x:32300,y:32798,varname:node_8201,prsc:2|A-9167-OUT,B-8692-R,T-8237-OUT;n:type:ShaderForge.SFN_Panner,id:5594,x:31847,y:32798,varname:node_5594,prsc:2,spu:0,spv:0.1|UVIN-9167-OUT,DIST-5765-OUT;n:type:ShaderForge.SFN_TexCoord,id:6266,x:31602,y:32576,varname:node_6266,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:8237,x:31769,y:33097,ptovrint:False,ptlb:wavyness,ptin:_wavyness,varname:node_8237,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.75,cur:1,max:1;n:type:ShaderForge.SFN_Tex2d,id:8692,x:32052,y:32798,ptovrint:False,ptlb:node_1383_copy,ptin:_node_1383_copy,varname:_node_1383_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-5594-UVOUT;n:type:ShaderForge.SFN_Slider,id:5765,x:31400,y:32877,ptovrint:False,ptlb:movement,ptin:_movement,varname:_node_8237_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0.5213675,max:5;n:type:ShaderForge.SFN_Color,id:9774,x:32475,y:32598,ptovrint:False,ptlb:node_9774,ptin:_node_9774,varname:node_9774,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.8970588,c2:0.8970588,c3:0.8970588,c4:1;n:type:ShaderForge.SFN_Multiply,id:5907,x:32697,y:32752,varname:node_5907,prsc:2|A-9774-RGB,B-1383-RGB;n:type:ShaderForge.SFN_Multiply,id:9167,x:31832,y:32576,varname:node_9167,prsc:2|A-7512-OUT,B-6266-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:7512,x:31801,y:32485,ptovrint:False,ptlb:tiling,ptin:_tiling,varname:node_7512,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;proporder:1383-8692-8237-5765-9774-7512;pass:END;sub:END;*/

Shader "Shader Forge/wavy" {
    Properties {
        _node_1383 ("node_1383", 2D) = "white" {}
        _node_1383_copy ("node_1383_copy", 2D) = "white" {}
        _wavyness ("wavyness", Range(0.75, 1)) = 1
        _movement ("movement", Range(-5, 5)) = 0.5213675
        _node_9774 ("node_9774", Color) = (0.8970588,0.8970588,0.8970588,1)
        _tiling ("tiling", Float ) = 4
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _node_1383; uniform float4 _node_1383_ST;
            uniform float _wavyness;
            uniform sampler2D _node_1383_copy; uniform float4 _node_1383_copy_ST;
            uniform float _movement;
            uniform float4 _node_9774;
            uniform float _tiling;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_9167 = (_tiling*i.uv0);
                float2 node_5594 = (node_9167+_movement*float2(0,0.1));
                float4 _node_1383_copy_var = tex2D(_node_1383_copy,TRANSFORM_TEX(node_5594, _node_1383_copy));
                float2 node_8201 = lerp(node_9167,float2(_node_1383_copy_var.r,_node_1383_copy_var.r),_wavyness);
                float4 _node_1383_var = tex2D(_node_1383,TRANSFORM_TEX(node_8201, _node_1383));
                float3 emissive = (_node_9774.rgb*_node_1383_var.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
