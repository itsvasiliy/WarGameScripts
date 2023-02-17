using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurretGun : NetworkBehaviour 
{
    [SerializeField] private ParticleSystem muzzleFlash;
    
    [SerializeField] private NetworkObject bulletPrefab;

    [Header("Transforms")]
    [SerializeField] private Transform turretTransform;
    [SerializeField] private Transform muzzleTransform;

    [Header("floats")]
    [SerializeField] private float firingRadius;
    [SerializeField] private float fireRite;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletDamage;

    private Transform targetTransform;

    private bool isShooting = false;

    private void FixedUpdate()
    {
        if (!IsServer) return;

        if (targetTransform == null || firingRadius < Vector3.Distance(turretTransform.position, targetTransform.position))
        {
            turretTransform.localRotation = Quaternion.Euler(Vector3.zero);

            TargetDetection();
            return;
        }


        LookAtTarget();

        if (!isShooting)
        {
            if (targetTransform.TryGetComponent<LandVehicle>(out LandVehicle _landVehicle) && _landVehicle.enabled)
            {
                Shoot();
            }
            else targetTransform = null;
        }
    }

    private void Shoot()
    {
        muzzleFlash.Play();

        SpawnBulletServerRpc(bulletSpeed);

        isShooting = true;
        Invoke(nameof(ResetShootingStatus), fireRite);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBulletServerRpc(float _bulletSpeed)
    {
        NetworkObject networkObjectBullet = Instantiate(bulletPrefab, muzzleTransform.position, turretTransform.rotation);
        networkObjectBullet.Spawn(true);

        networkObjectBullet.GetComponent<ExplosiveAmmunition>().BulletDamage = bulletDamage;
        networkObjectBullet.GetComponent<Rigidbody>().velocity = turretTransform.forward * _bulletSpeed;
    }

    private void ResetShootingStatus() => isShooting = false;

    private void LookAtTarget()
    {
        Vector3 lookAtPosition = targetTransform.position;
        lookAtPosition.y = turretTransform.position.y;

        turretTransform.LookAt(lookAtPosition);
    }

    private void TargetDetection()
    {
        Collider[] hitColliders = Physics.OverlapSphere(turretTransform.position, firingRadius);

        foreach (var enemy in hitColliders)
        {
            if (enemy.TryGetComponent<LandVehicle>(out LandVehicle _landVehicle) && _landVehicle.enabled) // Change "LandVehicle" to IDamageable 
            {
                if (_landVehicle.IsOwner != IsOwner)
                {
                    // It is necessary to check the Y position of the target
                    targetTransform = enemy.transform;
                }
            }
        }

        return;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(turretTransform.position, firingRadius);
    }
}