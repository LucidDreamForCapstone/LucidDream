/*using UnityEngine;
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
}*/
/* using UnityEngine;
 using UnityEngine.Rendering;
 using UnityEngine.Rendering.Universal;

 public class GlitchRenderFeature : ScriptableRendererFeature {
     class GlitchRenderPass : ScriptableRenderPass {
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
     }

     public Material glitchMaterial;

     GlitchRenderPass pass;

     public override void Create() {
         pass = new GlitchRenderPass(glitchMaterial);
         pass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
     }

     public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
         var volumeStack = VolumeManager.instance.stack;
         var glitchEffect = volumeStack.GetComponent<GlitchEffect>();

         if (glitchEffect == null || !glitchEffect.IsActive())
             return;

         pass.Setup(renderer.cameraColorTarget, glitchEffect);
         renderer.EnqueuePass(pass);
     }
 } */


using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlitchRenderPassFeature : ScriptableRendererFeature {
    class GlitchRenderPass : ScriptableRenderPass {
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

    public Material glitchMaterial;
    private GlitchRenderPass pass;

    public override void Create() {
        pass = new GlitchRenderPass(glitchMaterial);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents; // 또는 필요한 이벤트로 설정
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        var volumeStack = VolumeManager.instance.stack;
        var glitchEffect = volumeStack.GetComponent<GlitchEffect>();

        // 글리치 이펙트가 활성화되지 않으면 패스를 추가하지 않음
        if (glitchEffect == null || !glitchEffect.IsActive())
            return;

        pass.Setup(glitchEffect); // 글리치 이펙트만 설정
        renderer.EnqueuePass(pass); // 렌더 패스를 큐에 추가
    }
}

