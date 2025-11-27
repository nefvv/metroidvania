using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    [Header("Frame Rate Settings")]
    [Tooltip("Target frame rate. Set to -1 for unlimited.")]
    public int targetFrameRate = 60;
    
    [Tooltip("Enable VSync (overrides target frame rate)")]
    public bool enableVSync = false;
    
    void Awake()
    {
        // Apply frame rate settings immediately
        ApplyFrameRateSettings();
    }
    
    private void ApplyFrameRateSettings()
    {
        // Set VSync first
        QualitySettings.vSyncCount = enableVSync ? 1 : 0;
        
        // Set target frame rate (only works when VSync is off)
        if (!enableVSync)
        {
            Application.targetFrameRate = targetFrameRate;
        }
        
        Debug.Log($"Frame Rate Settings Applied - Target: {(enableVSync ? "VSync" : targetFrameRate.ToString())} FPS");
    }
    
    // Methods to change frame rate at runtime
    [ContextMenu("Set 60 FPS")]
    public void SetFrameRate60()
    {
        targetFrameRate = 60;
        enableVSync = false;
        ApplyFrameRateSettings();
    }
    
    [ContextMenu("Set 120 FPS")]
    public void SetFrameRate120()
    {
        targetFrameRate = 120;
        enableVSync = false;
        ApplyFrameRateSettings();
    }
    
    [ContextMenu("Set 240 FPS")]
    public void SetFrameRate240()
    {
        targetFrameRate = 240;
        enableVSync = false;
        ApplyFrameRateSettings();
    }
    
    [ContextMenu("Set Unlimited FPS")]
    public void SetFrameRateUnlimited()
    {
        targetFrameRate = -1;
        enableVSync = false;
        ApplyFrameRateSettings();
    }
    
    [ContextMenu("Enable VSync")]
    public void EnableVSync()
    {
        enableVSync = true;
        ApplyFrameRateSettings();
    }
}