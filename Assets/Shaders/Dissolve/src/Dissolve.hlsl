#ifndef DISSOLVE_INCLUDED
#define DISSOLVE_INCLUDED

TEXTURE2D(_NoiseMap);       SAMPLER(sampler_NoiseMap);

float ApplyDissolveCutout(float3 positionWS, float2 uv)
{
    float distance = length(positionWS - _DissolveOrigin.xyz);
    float noise = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, uv).x;
    distance = distance + _DissolveArea * noise;

    if(distance < _DissolveRadius - _DissolveArea)
    {
        discard;
    }

    return distance;
}

half4 ApplyDissolveEffect(float distance, half4 color)
{
    if(distance <= _DissolveRadius)
    {
        color = color + _DissolveColor * _DissolveEmission;
    }
    return color;
}

#endif
