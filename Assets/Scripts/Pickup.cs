using UnityEngine;

/// <summary>
/// 
///     Attached to a Pickup prefab that is dropped by a destroyed Crate.
/// 
/// </summary>

public class Pickup : MonoBehaviour
{
    // different types of pickups
    private enum PickupType { MaxHP, Repair, FireRate, MoveSpeed, AimSpeed, Damage };

    [SerializeField]
    private PickupType type; // type of pickup this instance is
    [SerializeField]
    private float timeToLive = 10f; // how long the pickup should be available before disappearing

    // connected objects we can get references to easily on a collision event
    private Damageable playerHealth;
    private TankController playerController;

    private void Update()
    {
        // start countdown immediately
        timeToLive -= Time.deltaTime;

        // remove object when time runs out
        if (timeToLive <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 
    ///     Handles how the pickup affects the player who collects it.
    /// 
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerHealth = collider.GetComponent<Damageable>();
            playerController = collider.GetComponent<TankController>();

            switch (type)
            {
                case PickupType.MaxHP:
                    {
                        if(playerHealth.MaxHP < playerHealth.HPCap) // player has not reached max hp cap
                        {
                            // increase max hp
                            playerHealth.MaxHP = playerHealth.MaxHP + playerController.HPUpgradeModifier;
                            // give a 50% repair of current damage with this upgrade (could be optional if advanced settings were implemented?)
                            playerHealth.Heal((playerHealth.MaxHP - playerHealth.CurrHP) / 2);
                            Hud.Instance.UpgradeMaxHP();
                        }
                        else if (playerHealth.MaxHP == playerHealth.HPCap) // player is at max hp cap
                        {
                            // give a 50% repair of current damage with this upgrade (could be optional if advanced settings were implemented?)
                            playerHealth.Heal((playerHealth.MaxHP - playerHealth.CurrHP) / 2);
                            Hud.Instance.UpdateHPAmount();
                        }
                        Destroy(gameObject);
                        break;
                    }
                case PickupType.Repair:
                    {
                        // completely heal the player
                        playerHealth.Heal(playerHealth.MaxHP);
                        Hud.Instance.UpdateHPAmount();
                        Destroy(gameObject);
                        break;
                    }
                case PickupType.FireRate: // upgrade fire rate
                    {
                        if(playerController.ReloadSpeedRank < playerController.UpgradeRankCap)
                        {
                            playerController.UpgradeReloadSpeed();
                            Hud.Instance.UpdateReloadRank();
                        }
                        Destroy(gameObject);
                        break;
                    }
                case PickupType.MoveSpeed: // upgrade move speed
                    {
                        if (playerController.MoveSpeedRank < playerController.UpgradeRankCap)
                        {
                            playerController.UpgradeMoveSpeed();
                            Hud.Instance.UpdateSpeedRank();
                        }
                        Destroy(gameObject);
                        break;
                    }
                case PickupType.AimSpeed: // upgrade turret rotation (aim) speed
                    {
                        if(playerController.AimSpeedRank < playerController.UpgradeRankCap)
                        {
                            playerController.UpgradeAimSpeed();
                            Hud.Instance.UpdateAimRank();
                        }
                        Destroy(gameObject);
                        break;
                    }
                case PickupType.Damage: // upgrade the amount of damage the player does
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
