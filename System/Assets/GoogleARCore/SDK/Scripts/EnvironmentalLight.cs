namespace GoogleARCore
{
    using GoogleARCoreInternal;
    using UnityEngine;
    using UnityEngine.Rendering;

    [ExecuteInEditMode]
    [HelpURL("https://developers.google.com/ar/reference/unity/class/GoogleARCore/EnvironmentalLight")]
    public class EnvironmentalLight : MonoBehaviour
    {
        [SuppressMemoryAllocationError(IsWarning = true, Reason = "Requires further investigation.")]
        public void Update()
        {
            if (Application.isEditor && (!Application.isPlaying || !GoogleARCoreInternal.ARCoreProjectSettings.Instance.IsInstantPreviewEnabled))
            {
                Shader.SetGlobalColor("_GlobalColorCorrection", Color.white);
                Shader.SetGlobalFloat("_GlobalLightEstimation", 1f);
                return;
            }

            if (Frame.LightEstimate.State != LightEstimateState.Valid)
            {
                return;
            }

            const float middleGray = 0.466f;
            float normalizedIntensity = Frame.LightEstimate.PixelIntensity / middleGray;

            Shader.SetGlobalColor("_GlobalColorCorrection", Frame.LightEstimate.ColorCorrection * normalizedIntensity);

            Shader.SetGlobalFloat("_GlobalLightEstimation", normalizedIntensity);
        }
    }
}
