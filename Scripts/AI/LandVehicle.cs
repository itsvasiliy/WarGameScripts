using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class LandVehicle : NetworkBehaviour, IUnit, IDamageable
{
    [SerializeField] private NavMeshAgent _landVehicleAgent;
    [SerializeField] private ObstacleAgent _obstacleAgent;

    [SerializeField] private Outline _outline;

    [SerializeField] private float _health;
    [SerializeField] private float _destructionTime;

    private TurretGun turretGunInChildren;

    private float _maxHealth;

    private void Awake()
    {
        _landVehicleAgent.avoidancePriority = Random.Range(0, 99);
        _maxHealth = _health;
    }

    public void SelectUnit()
    {
        if (enabled)
            _outline.OutlineWidth = 2f;
    }

    public void DeselectUnit()
    {
        if (enabled)
            _outline.OutlineWidth = 0f;
    }

    public void SetTarget(Vector3 _targetPosition)
    {
        if (enabled)
            _obstacleAgent.SetDestination(_targetPosition);
    }

    public void GetDamage(float damage)
    {
        _health -= damage;

        GetDamageEffect(_maxHealth, _health);

        if (_health <= 0f)
        {
            Invoke(nameof(Destruction), _destructionTime);
            TurnOffFunctionalityClientRpc();

            return;
        }

        return;
    }

    [ClientRpc]
    private void TurnOffFunctionalityClientRpc()
    {
        turretGunInChildren = gameObject.GetComponentInChildren<TurretGun>();
        if (turretGunInChildren != null) turretGunInChildren.enabled = false;

        GetComponent<LandVehicle>().enabled = false;

        return;
    }

    public void GetDamageEffect(float maxHealth, float currentHealth)
    {
        float healthInPercent = currentHealth / maxHealth * 100f;

        if (healthInPercent < 75f && healthInPercent > 50f)
        {
            print("Health: " + "50-75");
        }
        else if (healthInPercent < 50f && healthInPercent > 25f)
        {
            print("Health: " + "25-50");
        }
        else if (healthInPercent < 25f && healthInPercent > 0f)
        {
            print("Health: " + "0-25");
        }
        else if (healthInPercent <= 0f)
        {
            print("Health: " + "< 0");
        }

        return;
    }

    public void Destruction()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }
}