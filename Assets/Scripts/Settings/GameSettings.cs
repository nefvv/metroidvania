using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSettings : MonoBehaviour
{
    [Header("Frame Rate Settings")]
    public TMP_Dropdown frameRateDropdown;
    public Toggle vsyncToggle;
    
    [Header("Available Frame Rates")]
    public int[] availableFrameRates = { 30, 60, 120, 144, 240, -1 }; // -1 = Unlimited
    
    private const string FRAME_RATE_KEY = "TargetFrameRate";
    private const string VSYNC_KEY = "VSync";
    
    public static GameSettings Instance { get; private set; }
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Load settings on startup
        LoadSettings();
    }
    
    void Start()
    {
        SetupUI();
    }
    
    private void SetupUI()
    {
        // Setup frame rate dropdown
        if (frameRateDropdown != null)
        {
            frameRateDropdown.options.Clear();
            
            for (int i = 0; i < availableFrameRates.Length; i++)
            {
                string optionText = availableFrameRates[i] == -1 ? "Unlimited" : availableFrameRates[i] + " FPS";
                frameRateDropdown.options.Add(new TMP_Dropdown.OptionData(optionText));
            }
            
            // Set current value
            int currentFrameRate = Application.targetFrameRate;
            int selectedIndex = System.Array.IndexOf(availableFrameRates, currentFrameRate);
            if (selectedIndex == -1) selectedIndex = 1; // Default to 60 FPS
            
            frameRateDropdown.value = selectedIndex;
            frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);
        }
        
        // Setup VSync toggle
        if (vsyncToggle != null)
        {
            vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
            vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        }
    }
    
    public void OnFrameRateChanged(int dropdownIndex)
    {
        if (dropdownIndex < availableFrameRates.Length)
        {
            int targetFrameRate = availableFrameRates[dropdownIndex];
            SetTargetFrameRate(targetFrameRate);
        }
    }
    
    public void OnVSyncChanged(bool isEnabled)
    {
        SetVSync(isEnabled);
    }
    
    public void SetTargetFrameRate(int frameRate)
    {
        if (frameRate == -1)
        {
            Application.targetFrameRate = -1; // Unlimited
            Debug.Log("Frame rate set to: Unlimited");
        }
        else
        {
            Application.targetFrameRate = frameRate;
            Debug.Log($"Frame rate set to: {frameRate} FPS");
        }
        
        // Save setting
        PlayerPrefs.SetInt(FRAME_RATE_KEY, frameRate);
        PlayerPrefs.Save();
    }
    
    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        Debug.Log($"VSync: {(enabled ? "Enabled" : "Disabled")}");
        
        // Save setting
        PlayerPrefs.SetInt(VSYNC_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    private void LoadSettings()
    {
        // Load frame rate
        int savedFrameRate = PlayerPrefs.GetInt(FRAME_RATE_KEY, 60); // Default to 60 FPS
        SetTargetFrameRate(savedFrameRate);
        
        // Load VSync
        bool vsyncEnabled = PlayerPrefs.GetInt(VSYNC_KEY, 0) == 1;
        SetVSync(vsyncEnabled);
    }
    
    // Public methods for other scripts to use
    public int GetCurrentFrameRate()
    {
        return Application.targetFrameRate;
    }
    
    public bool IsVSyncEnabled()
    {
        return QualitySettings.vSyncCount > 0;
    }
    
    // Method to be called from UI buttons for quick frame rate changes
    public void SetFrameRate60() => SetTargetFrameRate(60);
    public void SetFrameRate120() => SetTargetFrameRate(120);
    public void SetFrameRate240() => SetTargetFrameRate(240);
    public void SetFrameRateUnlimited() => SetTargetFrameRate(-1);
}