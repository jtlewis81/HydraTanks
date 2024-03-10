using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    private int maxHP;
    [SerializeField]
    private int hpCap;
    [SerializeField]
    private float currHP;

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
    public int HPCap { get => hpCap; private set { } }

    public UnityEvent OnDead;
    public UnityEvent<float> OnHPChange;
    public UnityEvent OnHit, OnHeal;


    void Awake()
    {
        currHP = MaxHP;
    }

    public void Hit(float damage)
    {
        CurrHP -= damage;
        if (currHP <= 0)
        {
            OnDead?.Invoke();
            if (gameObject.GetComponent<EnemyAI>())
            {
                LevelManager.Instance.HandleEnemyDestoyed();
            }
            if (gameObject.GetComponent<PlayerInputHandler>())
            {
                Hud.Instance.UpdateHPAmount();
                LevelManager.Instance.GameOver();
            }
        }
        else
        {
            if (gameObject.GetComponent<PlayerInputHandler>())
            {
                Hud.Instance.UpdateHPAmount();
            }
            OnHit?.Invoke();
        }
    }

    public void Heal(float addedHP)
    {
        CurrHP += addedHP;
        CurrHP = Mathf.Clamp(currHP, 0, MaxHP);
        OnHeal?.Invoke();
    }
}
