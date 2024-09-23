using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlitchRenderPassFeature : ScriptableRendererFeature {
    class GlitchRenderPass : ScriptableRenderPass {
        private Material glitchMaterial;
        private RenderTargetIdentifier source;
        private RenderTargetHandle temporaryRenderTexture;

        public GlitchRenderPass(Material material) {
            glitchMaterial = material;
            temporaryRenderTexture.Init("_TemporaryRenderTexture");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            source = renderingData.cameraData.renderer.cameraColorTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (glitchMaterial == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Glitch Effect");

            // Get a temporary render texture
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(temporaryRenderTexture.id, opaqueDesc, FilterMode.Bilinear);

            // Apply the glitch effect using Blit
            Blit(cmd, source, temporaryRenderTexture.Identifier(), glitchMaterial);
            Blit(cmd, temporaryRenderTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(temporaryRenderTexture.id);
        }
    }

    [System.Serializable]
    public class GlitchSettings {
        public Material glitchMaterial = null;
    }

    public GlitchSettings settings = new GlitchSettings();
    private GlitchRenderPass glitchRenderPass;

    public override void Create() {
        glitchRenderPass = new GlitchRenderPass(settings.glitchMaterial) {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (settings.glitchMaterial != null) {
            renderer.EnqueuePass(glitchRenderPass);
        }
    }
}

