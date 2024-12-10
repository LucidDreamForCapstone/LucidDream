using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("Custom/GlitchEffect")]
public class GlitchEffect : VolumeComponent, IPostProcessComponent {
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);  // �۸�ġ ����

    public bool IsActive() => intensity.value > 0f;
    public bool IsTileCompatible() => false;
}
