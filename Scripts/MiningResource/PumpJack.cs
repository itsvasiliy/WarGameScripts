using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PumpJack : MonoBehaviour, IResourcesMiner
{
    [SerializeField] private NetworkObject _networkObject;

    private PlayerBalance _playerBalance;

    private int _IncomePerSecond = 45;

    public void SetNewOwner(PlayerBalance playerBalance)
    {
        _playerBalance = playerBalance;
    }

    public void IncomePerSecond()
    {
        if (_playerBalance != null)
        {
            _playerBalance.GetIncome(_IncomePerSecond);
        }

        Invoke(nameof(IncomePerSecond), 1f);

        return;
    }

    public Vector3 GetTransformPosition()
    {
        return transform.position;
    }
}