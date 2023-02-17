using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MissileTurret : MonoBehaviour
{
    [SerializeField] private AirDefenseMissile missilePrefab;

    [SerializeField] private Transform turretTransform;

    [SerializeField] private Transform[] positionOfMissileSlots;

    [SerializeField] private ParticleSystem[] launchMissileParticleSystem;

    [SerializeField] private float missileReloadingTime;
    [SerializeField] private float fireRite;
    [SerializeField] private float detectionRange;
    [SerializeField] private float missileXRotation;

    private Transform targetTransform;
    private Transform lastTargetTransform = null;

    private int currentFiringSlot = 0;

    private bool isReloading = false;

    private void Start()
    {
        StartCoroutine(AirTargetDetector());
    }

    IEnumerator AirTargetDetector()
    {
        while (true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(turretTransform.position, detectionRange);

            foreach (var target in hitColliders)
            {
                if (target.TryGetComponent<IAirDefenceTarget>(out IAirDefenceTarget airDefenceTarget))
                {
                    if (!isReloading)
                    {
                        if (lastTargetTransform != target.transform)
                        {
                            lastTargetTransform = target.transform;
                            targetTransform = target.transform;

                            launchMissileParticleSystem[currentFiringSlot].Play();

                            LaunchMissileServerRpc();
                            currentFiringSlot++;

                            if (currentFiringSlot >= positionOfMissileSlots.Length)
                            {
                                isReloading = true;
                                Invoke(nameof(ReloadMissiles), missileReloadingTime);
                            }
                        }
                    }
                }
            }

            yield return new WaitForSecondsRealtime(0.3f);
        }
    }

    private void FixedUpdate()
    {
        if (targetTransform != null)
        {
            Vector3 AimPosition = targetTransform.position;
            AimPosition.y = turretTransform.position.y;

            turretTransform.LookAt(AimPosition);
        }
        else
        {
            turretTransform.Rotate(0f, 3f, 0f);
        }
    }

    private void ReloadMissiles()
    {
        print("ReloadMissiles");
        currentFiringSlot = 0;
        isReloading = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void LaunchMissileServerRpc()
    {
        if (targetTransform != null)
        {
            AirDefenseMissile airDefenseMissileClone = Instantiate(missilePrefab,
                positionOfMissileSlots[currentFiringSlot].position,
                Quaternion.Euler(missileXRotation, turretTransform.eulerAngles.y, 0f));

            airDefenseMissileClone.GetComponent<NetworkObject>().Spawn();

            airDefenseMissileClone.SetTargetTransform = targetTransform;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(turretTransform.position, detectionRange);
    }

    private void OnDisable()
    {
        StopCoroutine(AirTargetDetector());
    }
}