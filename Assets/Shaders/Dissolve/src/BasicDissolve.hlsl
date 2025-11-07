#ifndef BASIC_DISSOLVE_INCLUDED
#define BASIC_DISSOLVE_INCLUDED

// Shared dissolve parameters
CBUFFER_START(BasicDissolveData)
    float4  _DissolveOrigin;
    float   _DissolveRadius;
CBUFFER_END

void ApplyDissolve(float3 positionWS)
{
    float distance = length(positionWS - _DissolveOrigin);
    if(distance < _DissolveRadius)
    {
        discard;
    }
}
#endif
