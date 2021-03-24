using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DebugClusterGrid : ScriptableRendererFeature
{
    public RenderPassEvent InjectPoint = RenderPassEvent.AfterRenderingOpaques;

    [System.Serializable]
    public class DebugSettings
    {
        public ComputeShader ClusterCompute;
    }

    [SerializeField]
    private DebugSettings settings = new DebugSettings();

    private const string shaderName = "Hidden/Debug/ClusterGrid";

    private Material clusterMaterial;
    private ClusteredRenderPass clusteredPass;

    /// <inheritdoc/>
    public override void Create()
    {
        if (clusteredPass == null)
        { 
            clusteredPass = new ClusteredRenderPass();
        }


        clusteredPass.profilerTag = name;
        // Configures where the render pass should be injected.
        clusteredPass.renderPassEvent = InjectPoint;

        if (clusterMaterial == null)
        {
            clusterMaterial = CoreUtils.CreateEngineMaterial(shaderName);
            clusteredPass.clusterMaterial = clusterMaterial;
        }
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (clusterMaterial == null)
        {
            clusterMaterial = CoreUtils.CreateEngineMaterial(shaderName);
            clusteredPass.clusterMaterial = clusterMaterial;
        }

        if (clusteredPass.Setup(settings))
        { 
            renderer.EnqueuePass(clusteredPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(clusterMaterial);
        clusteredPass.CleanUp();
    }

    private class ClusteredRenderPass : ScriptableRenderPass
    {
        internal string profilerTag;
        internal Material clusterMaterial;

        private ComputeShader computeShader;

        private ComputeBuffer clusterList, itemList;

        private static readonly int clusterSizeID = Shader.PropertyToID("_ClusterSize");
        private static readonly int clustersID = Shader.PropertyToID("_Clusters");

        internal ClusteredRenderPass()
        {
            if (clusterList == null)
            { 
                clusterList = new ComputeBuffer(2048, 2 * 4 * 4);
            }
            //if (itemList == null)
            //{
            //    itemList = new ComputeBuffer(2048, 4);
            //}
        }

        internal bool Setup(DebugSettings settings)
        {
            computeShader = settings.ClusterCompute;
            return clusterMaterial != null;
        }

        private void UpdateAABB()
        {
            if (computeShader != null)
            {
                int kernal = computeShader.FindKernel("ClusterAABBKernel");
                computeShader.SetInts(clusterSizeID, new int[] { Screen.width / 16, Screen.height / 16, 16, 0 });
                computeShader.SetBuffer(kernal, clustersID, clusterList);

                computeShader.Dispatch(kernal, 1, 1, 16);
                
            }
        }

        internal void CleanUp()
        {
            //CoreUtils.SafeRelease(clusterList);
            //CoreUtils.SafeRelease(itemList);
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            UpdateAABB();
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            Matrix4x4 worldMatrix;
            if (renderingData.cameraData.isSceneViewCamera)
            {
                worldMatrix = Camera.main.transform.localToWorldMatrix;
            }
            else
            {
                worldMatrix = renderingData.cameraData.camera.transform.localToWorldMatrix;
            }

            if (clusterMaterial != null)
            {
                clusterMaterial.SetBuffer(clustersID, clusterList);
                cmd.DrawProcedural(worldMatrix, clusterMaterial, 0, MeshTopology.Points, 1000);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            Vector4[] aabb = new Vector4[2048 * 2];
            clusterList.GetData(aabb);
            Debug.Log("ab0:" + aabb[0]);
            Debug.Log("ab1:" + aabb[1]);
            Debug.Log("ab2:" + aabb[2]);
            Debug.Log("ab3:" + aabb[3]);
        }
    }

}


