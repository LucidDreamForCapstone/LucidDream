/*using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlitchRenderPass : ScriptableRenderPass {
    private Material glitchMaterial;
    private RenderTargetIdentifier currentTarget;
    private GlitchEffect glitchEffect;

    public GlitchRenderPass(Material material) {
        this.glitchMaterial = material;
    }

    public void Setup(RenderTargetIdentifier source, GlitchEffect effect) {
        this.currentTarget = source;
        this.glitchEffect = effect;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
        if (glitchMaterial == null || glitchEffect == null || !glitchEffect.IsActive())
            return;

        var cmd = CommandBufferPool.Get("Glitch Effect");
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    private void Render(CommandBuffer cmd, ref RenderingData renderingData) {
        int tempTargetId = Shader.PropertyToID("_TempTarget");
        cmd.GetTemporaryRT(tempTargetId, renderingData.cameraData.cameraTargetDescriptor);
        cmd.Blit(currentTarget, tempTargetId);

        glitchMaterial.SetFloat("_Intensity", glitchEffect.intensity.value);

        cmd.Blit(tempTargetId, currentTarget, glitchMaterial);
        cmd.ReleaseTemporaryRT(tempTargetId);
    }
}*/
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlitchRenderPass : ScriptableRenderPass {
    private Material glitchMaterial;
    private GlitchEffect glitchEffect;

    public GlitchRenderPass(Material material) {
        this.glitchMaterial = material;
    }

    public void Setup(GlitchEffect effect) {
        this.glitchEffect = effect;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
        if (glitchMaterial == null || glitchEffect == null || !glitchEffect.IsActive())
            return;

        // 카메라 컬러 타겟을 Execute 메서드에서 가져옵니다.
        RenderTargetIdentifier currentTarget = renderingData.cameraData.renderer.cameraColorTarget;

        var cmd = CommandBufferPool.Get("Glitch Effect");
        Render(cmd, ref renderingData, currentTarget);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    private void Render(CommandBuffer cmd, ref RenderingData renderingData, RenderTargetIdentifier currentTarget) {
        int tempTargetId = Shader.PropertyToID("_TempTarget");
        cmd.GetTemporaryRT(tempTargetId, renderingData.cameraData.cameraTargetDescriptor);
        cmd.Blit(currentTarget, tempTargetId);

        glitchMaterial.SetFloat("_Intensity", glitchEffect.intensity.value);

        cmd.Blit(tempTargetId, currentTarget, glitchMaterial);
        cmd.ReleaseTemporaryRT(tempTargetId);
    }
}



