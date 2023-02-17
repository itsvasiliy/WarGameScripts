using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refinery : MonoBehaviour, IResourcesMiner, IDamageable
{
    private PlayerBalance ownerBalance;

    private float health = 2200;

    private int _IncomePerSecond = 185;

    public void Destruction()
    {
        Destroy(gameObject);
    }

    public void GetDamage(float damage)
    {
        health -= damage;

        if(health >= 0)
        {
            Destruction();
        }
    }

    public Vector3 GetTransformPosition()
    {
        return transform.position;
    }

    public void IncomePerSecond()
    {
        if (ownerBalance != null)
        {
            ownerBalance.GetIncome(_IncomePerSecond);
        }

        Invoke(nameof(IncomePerSecond), 1f);

        return;
    }

    public void SetNewOwner(PlayerBalance playerBalance)
    {
        ownerBalance = playerBalance;
    }
}
