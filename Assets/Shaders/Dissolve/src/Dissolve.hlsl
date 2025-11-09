#ifndef DISSOLVE_INCLUDED
#define DISSOLVE_INCLUDED

CBUFFER_START(NoiseDissolveData)
    float4  _DissolveOrigin;
    float   _DissolveRadius;
    float4  _DissolveColor;
    float   _DissolveEmission;
    float   _DissolveArea;
CBUFFER_END

TEXTURE2D(_NoiseTexture);
SAMPLER(sampler_NoiseTexture);

float ApplyDissolveCutout(float3 positionWS, float2 uv)
{
    float distance = length(positionWS - _DissolveOrigin);
    float noise = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture, uv).x;
    distance = distance + _DissolveArea * noise;

    if(distance < _DissolveRadius - _DissolveArea)
    {
        discard;
    }

    return distance;
}

half4 ApplyDissolveEffect(float distance, half4 color)
{
    if(distance <= _DissolveRadius + _DissolveArea)
    {
        color = color * _DissolveColor * _DissolveEmission;
    }
    return color;
}

#endif
