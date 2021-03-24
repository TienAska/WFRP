using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SDFRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class SDFSettings
    {
        public SDFRenderer[] SDFs;
    }

    private const string sdfShaderName = "Hidden/SDF";

    private Material sdfMaterial;
    private SDFRenderPass sdfPass;

    public override void Create()
    {
        sdfPass = new SDFRenderPass();

        sdfPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        sdfPass.SdfMaterial = CoreUtils.CreateEngineMaterial(sdfShaderName);
    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(sdfPass);
    }

    class SDFRenderPass : ScriptableRenderPass
    {
        public Matrix4x4 SdfMatrix;
        public Material SdfMaterial;

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.DrawProcedural(SdfMatrix, SdfMaterial, 0, MeshTopology.Points, 1);
        }
    }
}


