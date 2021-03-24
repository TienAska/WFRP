using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SDFRenderer : MonoBehaviour
{
    private SphereCollider sdf;

    public Vector3 Center => sdf.center;

    public float Radius => sdf.radius;

    private void Awake()
    {
        sdf = GetComponent<SphereCollider>();
    }
}
