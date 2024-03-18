using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
///     Attached to the UIManager prefab.
///     Handles updating player health, upgrade ranks, and kill count (score).
/// 
/// </summary>

public class Hud : MonoBehaviour
{
    // is a singleton
    public static Hud Instance;

    // references to UI game objects to be assigned in the editor
    [SerializeField] private RectTransform hpBar;
    [SerializeField] private Slider hpAmount;
    [SerializeField] private GameObject[] ReloadRanks;
    [SerializeField] private GameObject[] SpeedRanks;
    [SerializeField] private GameObject[] AimRanks;
    [SerializeField] private GameObject[] DamageRanks;
    [SerializeField] private TextMeshProUGUI KillCount;
    [SerializeField] private TextMeshProUGUI MaxHPIndicator;

    // cached references for the player
    private GameObject player;
    private Damageable playerHealth;
    private TankController playerTankController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // initialzation is delayed to fix null reference issues due to script execution timing when loading in a level
        Invoke("Initialize", 1f);
    }

    /// <summary>
    /// 
    ///     Gets references to the player and updates the UI to reflect the player's default hp and stat ranks.
    ///     Will need updated for multiplayer to get the local player only.
    /// 
    /// </summary>
    private void Initialize()
    {
        player = FindObjectOfType<PlayerInputHandler>().gameObject;
        playerHealth = player.GetComponent<Damageable>();
        playerTankController = player.GetComponent<TankController>();
        UpgradeMaxHP();
        UpdateReloadRank();
        UpdateSpeedRank();
        UpdateAimRank();
        UpdateDamageRank();
        UpdateTanksDestroyed();
    }

    /// <summary>
    /// 
    ///     Updates the UI for the player's HP/health amount.
    /// 
    /// </summary>
    public void UpdateHPAmount()
    {
        hpAmount.value = playerHealth.CurrHP / playerHealth.MaxHP;
    }

    /// <summary>
    /// 
    ///     Updates the UI for the player's max HP/health.
    /// 
    /// </summary>
    public void UpgradeMaxHP()
    {
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerHealth.MaxHP);
        UpdateHPAmount();
        if(playerHealth.MaxHP == playerHealth.HPCap)
        {
            MaxHPIndicator.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 
    ///     Updates the UI to relect the player's current Reload Rank.
    /// 
    /// </summary>
    public void UpdateReloadRank()
    {
        for(int i = 0; i < playerTankController.ReloadSpeedRank; i++)
        {
            ReloadRanks[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 
    ///     Updates the UI to reflect the player's current Speed Rank.
    /// 
    /// </summary>
    public void UpdateSpeedRank()
    {
        for (int i = 0; i < playerTankController.MoveSpeedRank; i++)
        {
            SpeedRanks[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 
    ///     Updates the UI to reflect the player's current Aim (turret rotation speed) Rank.
    /// 
    /// </summary>
    public void UpdateAimRank()
    {
        for (int i = 0; i < playerTankController.AimSpeedRank; i++)
        {
            AimRanks[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 
    ///     Updates the UI to reflect the player's current Damage Rank.
    /// 
    /// </summary>
    public void UpdateDamageRank()
    {
        for (int i = 0; i < playerTankController.DamageRank; i++)
        {
            DamageRanks[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 
    ///     Updates the UI to reflect the player's current Kill Count (score).
    /// 
    /// </summary>
    public void UpdateTanksDestroyed()
    {
        KillCount.text = LevelManager.Instance.KillCount.ToString();
    }

}
