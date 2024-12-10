using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimeLineTrigger : TutorialStart {
    [SerializeField] private CinemachineVirtualCamera targetCamera; // Target Cinemachine Virtual Camera
    [SerializeField] private CinemachineVirtualCamera defaultCamera; // Default Virtual Camera
    [SerializeField] private TimeLineManager timelineManager; // Timeline Manager
    private bool hasTriggered = false; // Check if the player has already triggered the event
    private CinemachineBrain cinemachineBrain; // Reference to the Cinemachine Brain

    private void Start() {
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        if (cinemachineBrain == null) {
            Debug.LogError("CinemachineBrain not found on the main camera!");
        }
    }

    private async void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player") || hasTriggered) {
            return;
        }

        hasTriggered = true;

        // Temporarily set Manual Update for Cinemachine
        if (cinemachineBrain != null) {
            cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.ManualUpdate;
        }
        // Switch to target camera
        ActivateCamera(targetCamera);

        await FadeInColor_unscaled(); // Assume this is a color fade-in effect
        //timelineManager.ResumeTimeline(); // Resume the timeline

        // Wait for 2 seconds (ignores time scale)
        await UniTask.Delay(System.TimeSpan.FromSeconds(2), ignoreTimeScale: true);

        // Restore default camera
        RestoreDefaultCamera();

        // Restore Cinemachine to Smart Update
        if (cinemachineBrain != null) {
            cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.SmartUpdate;
        }
    }

    private void ActivateCamera(CinemachineVirtualCamera cameraToActivate) {
        if (cameraToActivate != null) {
            foreach (var vcam in FindObjectsOfType<CinemachineVirtualCamera>()) {
                vcam.Priority = 0; // Reset all cameras' priority
            }

            cameraToActivate.Priority = 10; // Set the active camera's priority
        }
    }

    private void RestoreDefaultCamera() {
        if (defaultCamera != null) {
            foreach (var vcam in FindObjectsOfType<CinemachineVirtualCamera>()) {
                vcam.Priority = 0; // Reset all cameras' priority
            }

            defaultCamera.Priority = 10; // Set the default camera's priority
        }
    }
}
