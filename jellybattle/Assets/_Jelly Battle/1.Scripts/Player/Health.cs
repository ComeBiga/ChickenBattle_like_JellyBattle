using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int max = 100;
    [SerializeField] int current;
    public bool IsDead { get { return current <= 0; } }

    public OnTakeDamage onTakeDamage;
    public UnityEvent onDie;

    [System.Serializable]
    public class OnTakeDamage : UnityEvent<int>
    {

    }

    public void Start()
    {
        current = max;
    }

    public void SetCurrentHP(int hp)
    {
        current = hp;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        current = Mathf.Max(current - damage, 0);

        if (IsDead)
        {
            onDie.Invoke();
            Die();
        }
        else
        {
            onTakeDamage.Invoke(damage);
        }
    }

    [PunRPC]
    public void Heal(int healthToRestore)
    {
        current = Mathf.Min(current + healthToRestore, max);
    }

    private void Die()
    {

    }
}
