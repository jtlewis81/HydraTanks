using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public static Hud Instance;

     public Text HP, FireRate, TanksDestroyed;
     
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
        UpdateHP();
        UpdateFireRate();
        UpdateTanksDestroyed();
    }

	public void UpdateHP()
	{
          HP.text = playerHealth.CurrHP.ToString() + " / " + playerHealth.MaxHP.ToString();
	}

     public void UpdateFireRate()
	{
          FireRate.text = playerTankController.ReloadTimer.ToString() + " s";
	}

	public void UpdateTanksDestroyed()
	{
          TanksDestroyed.text = LevelManager.Instance.KillCount.ToString();
	}
}
