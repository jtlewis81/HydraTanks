using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public static Hud Instance;

    [SerializeField] private RectTransform hpBar;
    [SerializeField] private Slider hpAmount;
    [SerializeField] private GameObject[] ReloadRanks;
    [SerializeField] private GameObject[] SpeedRanks;
    [SerializeField] private GameObject[] AimRanks;
    [SerializeField] private GameObject[] DamageRanks;
    [SerializeField] private TextMeshProUGUI KillCount;

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
        Invoke("Initialize", 1f);
    }

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

    public void UpdateHPAmount()
    {
        hpAmount.value = playerHealth.CurrHP / playerHealth.MaxHP;
    }

    public void UpgradeMaxHP()
    {
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerHealth.MaxHP);
        UpdateHPAmount();
    }

    public void UpdateReloadRank()
    {
        for(int i = 0; i < playerTankController.ReloadSpeedRank; i++)
        {
            ReloadRanks[i].gameObject.SetActive(true);
        }
    }

    public void UpdateSpeedRank()
    {
        for (int i = 0; i < playerTankController.MoveSpeedRank; i++)
        {
            SpeedRanks[i].gameObject.SetActive(true);
        }
    }

    public void UpdateAimRank()
    {
        for (int i = 0; i < playerTankController.AimSpeedRank; i++)
        {
            AimRanks[i].gameObject.SetActive(true);
        }
    }

    public void UpdateDamageRank()
    {
        for (int i = 0; i < playerTankController.DamageRank; i++)
        {
            DamageRanks[i].gameObject.SetActive(true);
        }
    }

    public void UpdateTanksDestroyed()
    {
        KillCount.text = LevelManager.Instance.KillCount.ToString();
    }

}
