using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUIItem : MonoBehaviour
{
    [Header("UI Components")]
    public Image abilityIcon;
    public TextMeshProUGUI abilityName;
    public TextMeshProUGUI abilityDescription;
    public GameObject lockedOverlay;
    public Button unlockButton;
    
    [Header("Visual States")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = Color.gray;
    
    private BaseAbility _ability;
    private PlayerAbilities _playerAbilities;
    private bool _isUnlocked;
    
    public void Initialize(BaseAbility ability, PlayerAbilities playerAbilities)
    {
        _ability = ability;
        _playerAbilities = playerAbilities;
        
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        if (_ability == null) return;
        
        _isUnlocked = _playerAbilities.Has(_ability.abilityId);
        
        // Update icon
        if (abilityIcon != null)
        {
            abilityIcon.sprite = _ability.icon;
            abilityIcon.color = _isUnlocked ? unlockedColor : lockedColor;
        }
        
        // Update name
        if (abilityName != null)
        {
            abilityName.text = _ability.abilityName;
            abilityName.color = _isUnlocked ? unlockedColor : lockedColor;
        }
        
        // Update description
        if (abilityDescription != null)
        {
            abilityDescription.text = _isUnlocked ? _ability.description : _ability.GetFullDescription();
        }
        
        // Update locked overlay
        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(!_isUnlocked);
        }
        
        // Update unlock button
        if (unlockButton != null)
        {
            bool canUnlock = !_isUnlocked && _playerAbilities.CanUnlockAbility(_ability.abilityId);
            unlockButton.gameObject.SetActive(canUnlock);
            unlockButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(() => TryUnlockAbility());
        }
    }
    
    private void TryUnlockAbility()
    {
        if (_playerAbilities.CanUnlockAbility(_ability.abilityId))
        {
            _playerAbilities.UnlockAbility(_ability.abilityId);
            UpdateUI();
        }
    }
    
    public void OnAbilityClicked()
    {
        // This could show more detailed info, use the ability, etc.
        if (_isUnlocked && !_ability.isPassive)
        {
            _playerAbilities.UseAbility(_ability.abilityId);
        }
    }
}