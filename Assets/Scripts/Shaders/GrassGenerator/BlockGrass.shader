Shader "Custom/BlockGrassCompute"
{
    Properties
    {
        // Shader properties which are editable in the material
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        
        Pass 
        {
            // Forward Lit Pass. The main pass which renders colors
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            // Signla this shader requires compute buffesr
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 5.0

            // Lighting and shadow keywords
            #pragma multi_compile _ _Main_LIGHT_SHADOWS
            #pragma multi_compile _ _Main_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHT
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            // Register our function
            #pragma vertex Vertex
            #pragma fragment Fragment

            // Include our logic file
            #include "BlockGrass.hlsl"

            ENDHLSL
        }

        Pass 
        {
            // Forward Lit Pass. The main pass which renders colors
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            HLSLPROGRAM
            // Signla this shader requires compute buffesr
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 5.0

            // Lighting and shadow keywords
            #pragma multi_compile_shadowcaster

            // Register our function
            #pragma vertex Vertex
            #pragma fragment Fragment

            // Define a special keyword so our logic can change if inside the shadow caster pass
            #define SHADOW_CASTER_PASS

            // Include our logic file
            #include "BlockGrass.hlsl"

            ENDHLSL
        }
    }
}
