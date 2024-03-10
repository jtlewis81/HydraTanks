using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    private float timeToLive = 10f;

    private enum PickupType { MaxHP, Repair, FireRate, MoveSpeed, AimSpeed, Damage };

    [SerializeField]
    private PickupType type;

    private GameObject player;
    private Damageable playerHealth;
    private TankController playerController;

    private void Awake()
    {
        player = FindObjectOfType<PlayerInputHandler>().gameObject;
        playerHealth = player.GetComponent<Damageable>();
        playerController = player.GetComponent<TankController>();
    }

    private void Update()
    {
        timeToLive -= Time.deltaTime;

        if (timeToLive <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            switch (type)
            {
                case PickupType.MaxHP:
                    {
                        if(playerHealth.MaxHP < playerHealth.HPCap)
                        {
                            playerHealth.MaxHP = playerHealth.MaxHP + playerController.HPUpgradeModifier;
                            // give a 50% repair of current damage with this upgrade (could be optional if advanced settings were implemented?)
                            playerHealth.Heal((playerHealth.MaxHP - playerHealth.CurrHP) / 2);
                            Hud.Instance.UpgradeMaxHP();
                        }
                        Destroy(gameObject);
                        break;
                    }
                case PickupType.Repair:
                    {
                        playerHealth.Heal(playerHealth.MaxHP);
                        Hud.Instance.UpdateHPAmount();
                        Destroy(gameObject);
                        break;
                    }
                case PickupType.FireRate:
                    {
                        if(playerController.ReloadSpeedRank < playerController.UpgradeRankCap)
                        {
                            playerController.UpgradeReloadSpeed();
                            Hud.Instance.UpdateReloadRank();
                        }
                        Destroy(gameObject);
                        break;
                    }
                case PickupType.MoveSpeed:
                    {
                        if (playerController.MoveSpeedRank < playerController.UpgradeRankCap)
                        {
                            playerController.UpgradeMoveSpeed();
                            Hud.Instance.UpdateSpeedRank();
                        }
                        Destroy(gameObject);
                        break;
                    }
                case PickupType.AimSpeed:
                    {
                        if(playerController.AimSpeedRank < playerController.UpgradeRankCap)
                        {
                            playerController.UpgradeAimSpeed();
                            Hud.Instance.UpdateAimRank();
                        }
                        Destroy(gameObject);
                        break;
                    }
                case PickupType.Damage:
                    {
                        if(playerController.DamageRank < playerController.UpgradeRankCap)
                        {
                            playerController.UpgradeDamage();
                            Hud.Instance.UpdateDamageRank();
                        }
                        Destroy(gameObject);
                        break;
                    }
                default: break;
            }
        }
    }


}
