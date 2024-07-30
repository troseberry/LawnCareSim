// Make sure this file is not included twice
#ifndef GRASSBLADES_INCLUDED
#define GRASSBLADES_INCLUDED

// Include some helper functions
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "NMGBladeGrassGraphicsHelpers.hlsl"

struct Attributes {
    float3 positionOS       : POSITION;
    float3 normalOS         : NORMAL;
    float2 uv               : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput {
    float2 uv           : TEXCOORD0; // The height of this vertex on the grass blade
    float3 positionWS   : TEXCOORD1; // Position in world space
    float3 normalWS     : TEXCOORD2; // Normal vector in world space

    float4 positionCS   : SV_POSITION; // Position in clip space
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// Properties
float4 _BaseColor;
float4 _TipColor;

// Vertex functions

VertexOutput Vertex(Attributes input) {
    // Initialize the output struct
    VertexOutput output = (VertexOutput)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

    output.positionWS = GetVertexPositionInputs(input.positionOS).positionWS;
    output.normalWS = GetVertexNormalInputs(input.normalOS).normalWS;
    output.uv = input.uv;
    output.positionCS = TransformWorldToHClip(output.positionWS);

    return output;
}

// Fragment functions

half4 Fragment(VertexOutput input) : SV_Target {
    UNITY_SETUP_INSTANCE_ID(input);

    // Gather some data for the lighting algorithm
    InputData lightingInput = (InputData)0;
    lightingInput.positionWS = input.positionWS;
    lightingInput.normalWS = input.normalWS; // No need to normalize, triangles share a normal
    lightingInput.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS); // Calculate the view direction
    lightingInput.shadowCoord = CalculateShadowCoord(input.positionWS, input.positionCS);

    // Lerp between the base and tip color based on the blade height
    float colorLerp = input.uv.y;
    float3 albedo = lerp(_BaseColor.rgb, _TipColor.rgb, colorLerp);

    SurfaceData surfaceInput = (SurfaceData)0;
    surfaceInput.albedo = albedo;
    surfaceInput.specular = 1;
    surfaceInput.specular = 0;
    surfaceInput.smoothness = 0;
    surfaceInput.alpha = 1;

    // The URP simple lit algorithm
    // The arguments are lighting input data, albedo color, specular color, smoothness, emission color, and alpha
    //return UniversalFragmentBlinnPhong(lightingInput, albedo, 1, 0, 0, 1);

    return UniversalFragmentBlinnPhong(lightingInput, surfaceInput);
}

#endif