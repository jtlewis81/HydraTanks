using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 
///     Attached to tanks & destructible objects for taking damage.
///     This is essentially a health system.
/// 
/// </summary>

public class Damageable : MonoBehaviour
{
    // settings
    [SerializeField] private int maxHP;
    [SerializeField] private int hpCap;
    [SerializeField] private float currHP;

    // public references for the Hud Instance
    public float CurrHP
    {
        get { return currHP; }
        set
        {
            currHP = value;
            OnHPChange?.Invoke((float)CurrHP / MaxHP);
        }
    }

    public int MaxHP { get => maxHP; set => maxHP = value; }
    public int HPCap { get => hpCap; private set { } } // also accessed by Pickup

    // Unity Events
    public UnityEvent OnDead;
    public UnityEvent<float> OnHPChange;
    public UnityEvent OnHit, OnHeal;

    void Awake()
    {
        // fill HP
        currHP = MaxHP;
    }

    /// <summary>
    /// 
    ///     Apply damage to the gameobject this script instance is attached to
    ///     and handle death events.
    /// 
    /// </summary>
    /// <param name="damage"></param>
    public void Hit(float damage)
    {
        // apply damage
        CurrHP -= damage;

        if (currHP <= 0) // handle death
        {
            OnDead?.Invoke();
            if (gameObject.GetComponent<EnemyAI>()) // is enemy
            {
                LevelManager.Instance.HandleEnemyDestoyed();
            }
            else if (gameObject.GetComponent<PlayerInputHandler>()) // is player
            {
                Hud.Instance.UpdateHPAmount();
                LevelManager.Instance.GameOver();
            }
            else // is a destructible object
            {
                Collider2D collider = GetComponent<Collider2D>();
                if (collider == null)
                {
                    collider = GetComponentInChildren<Collider2D>();
                }
                
                LevelManager.Instance.UpdateMap(collider);
            }
        }
        else // update UI for player
        {
            if (gameObject.GetComponent<PlayerInputHandler>())
            {
                Hud.Instance.UpdateHPAmount();
            }

            OnHit?.Invoke();
        }
    }

    /// <summary>
    /// 
    ///     Healing method called from Pickups.
    ///     Applies to players only.
    /// 
    /// </summary>
    /// <param name="addedHP"></param>
    public void Heal(float addedHP)
    {
        CurrHP += addedHP;
        CurrHP = Mathf.Clamp(currHP, 0, MaxHP);
        OnHeal?.Invoke();
    }
}
