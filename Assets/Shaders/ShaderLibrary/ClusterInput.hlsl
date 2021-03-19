#ifndef CLUSTER_INPUT_INCLUDED
#define CLUSTER_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// x, y represent size in pixel; z represent dispatched z size.
uint4 _clusterSize;

struct ClusterAABB
{
	float4 max;
	float4 min;
};

#endif // !CLUSTER_INPUT_INCLUDED
