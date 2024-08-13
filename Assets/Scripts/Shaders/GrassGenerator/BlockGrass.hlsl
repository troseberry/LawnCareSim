// Make sure this file is not included twice
#ifndef BLOCKGRASS_INCLUDED
#define BLOCKGRASS_INCLUDED

// Inlcude helper functions from URP
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "NMGPyramidGraphicsHelpers.hlsl"

// A vertex on the generated mesh
struct DrawVertex
{
    float3 positionWS;  // position in world space
    float2 uv;          // UV
};

struct DrawTriangle
{
    float3 normalWS;    // normal in world space. All points share this normal
    DrawVertex vertices[3];
    float color;
};

// The buffer to draw from
StructuredBuffer<DrawTriangle> _drawTriangles;

struct VertexOutput
{
    float3 positionWS       : TEXCOORD0;    // Position in world space
    float3 normalWS         : TEXCOORD1;    // Normal vector in world space
    float2 uv                : TEXCOORD2;    // UVs
    float4 positionCS       : SV_POSITION;  // Position in clip space
    float color             : PSIZE1;
};

// Properties
float4 _BaseColor;
float4 _TipColor;
float _MinY;
float _MaxY;


// The _MainTex property. The sample and scale/offset vector is also created
TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_ST;

// The SV_VertexID semantic is an index we can use to get a vertex to work on
// The max value of this is the first segment in the indirect args buffer
// The system will create triangles out of each three consecutive vertices
VertexOutput Vertex(uint vertexID: SV_VertexID)
{
    // Init the output struct
    VertexOutput output = (VertexOutput)0;

    // Get the vertex from the buffer
    // Since the buffer is structured in triangles, we need to divide the vertexID by 3
    // to get the triangle, and then modulo by 3 to get the vertex on the triangle
    DrawTriangle tri = _drawTriangles[vertexID / 3];
    DrawVertex input = tri.vertices[vertexID % 3];

    output.positionWS = input.positionWS;
    output.normalWS = tri.normalWS;
    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
    // Apply the shadow caster logic to the CS position
    output.positionCS = CalculatePositionCSWithShadowCasterLogic(input.positionWS, tri.normalWS);
    output.color = tri.color;

    return output;
}

float4 Fragment(VertexOutput input) : SV_Target
{
#ifdef SHADOW_CASTER_PASS
    // If in the shadow caster pass, we can just return now
    // it's enough to signal that should will cast a shadow
    return 0;
#else
    // Init some information for the lighting function
    InputData lightingInput = (InputData)0;
    lightingInput.positionWS = input.positionWS;
    lightingInput.normalWS = input.normalWS;        // No need to renormalize, since triangles all share normals
    lightingInput.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS);
    lightingInput.shadowCoord = CalculateShadowCoord(input.positionWS, input.positionCS);

    //float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb;
    
    // Lerp between the base and tip color based on the blade height
    //float colorLerp = lerp(_MinY, _MaxY, input.positionWS.y);
    float3 albedo = lerp(_BaseColor.rgb, _TipColor.rgb, input.color);

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
#endif
}
#endif