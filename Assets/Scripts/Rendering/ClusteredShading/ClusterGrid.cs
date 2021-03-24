using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterGrid : MonoBehaviour
{
    [SerializeField]
    private Transform cube;

    [SerializeField, Range(0, 10)]
    private int sizeX, sizeY, sizeZ;

    private Transform[] cubes;

    private GameObject go;
    private void Awake()
    {
        if (cube != null)
        {
            cubes = new Transform[sizeX * sizeY * sizeZ];

            int id = 0;
            for (int z = 0; z < sizeZ; z++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {                        
                        //cubes[id] = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity, transform);
                        //cubes[id].name = "cube" + ++id;
                    }
                }
            }
        }

    }

    private RenderTexture rtColor;
    private RenderTexture rtDepth;

    private void Start()
    {
        rtColor = new RenderTexture(Screen.width, Screen.height, 24);
        rtDepth = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (cube != null)
        {
            //GL.wireframe = true;
            Graphics.SetRenderTarget(src.colorBuffer, src.depthBuffer);
            //GL.Clear(true, true, Color.green);
            cube.GetComponent<MeshRenderer>().sharedMaterial.SetPass(0);
            //Graphics.DrawProcedural(cube.GetComponent<MeshRenderer>().material, new Bounds(Vector3.zero, new Vector3(10, 10, 10)), MeshTopology.Triangles, cubes.Length);
            Graphics.DrawProceduralNow(MeshTopology.Points, cubes.Length);
            Graphics.Blit(src, dst);
            //GL.wireframe = false;
        }
    }
}
