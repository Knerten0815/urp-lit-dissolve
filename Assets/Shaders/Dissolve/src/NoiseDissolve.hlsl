#ifndef NOISE_DISSOLVE_INCLUDED
#define NOISE_DISSOLVE_INCLUDED

#include "Assets/Shaders/Dissolve/src/BasicDissolve.hlsl"

CBUFFER_START(NoiseDissolveData)
    float4  _DissolveColor;
    float   _DissolveEmission;
    float   _DissolveArea;
CBUFFER_END

TEXTURE2D(_NoiseTexture);
SAMPLER(sampler_NoiseTexture);

float4 ApplyNoiseDissolve(float3 positionWS, float2 uv)
{
    float distance = length(positionWS - _DissolveOrigin);
    float noise = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, uv).x;
    distance = distance - _DissolveArea * noise;

    if(distance < _DissolveRadius)
    {
        discard;
    }

    // float4 textureColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv); test
    float4 textureColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, uv);

    if(distance <= _DissolveRadius + _DissolveArea)
    {
        return textureColor * _DissolveColor * _DissolveEmission;
    }

    return textureColor * _BaseColor;
}

#endif
