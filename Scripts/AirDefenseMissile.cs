using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class AirDefenseMissile : MonoBehaviour
{
    [SerializeField] private NetworkObject _networkObject;
    [SerializeField] private NetworkObject explosionEffectNetworkObject;

    [SerializeField] private Transform missileTransform;

    [SerializeField] private float missileSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float damage;
    [SerializeField] private float triggerZoneRadius;
    [SerializeField] private float blastWaveRadius;

    private Transform targetTransform;

    public float GetMissileSpeed
    {
        get { return missileSpeed; }
    }

    public Transform SetTargetTransform
    {
        set
        {
            targetTransform = value;
            StartCoroutine(FlyToTarget());
            StartCoroutine(TargetDetectionAround());
        }
    }

    IEnumerator FlyToTarget()
    {
        while (targetTransform != null)
        {
            missileTransform.position = Vector3.Lerp(missileTransform.position, targetTransform.position, missileSpeed * Time.fixedDeltaTime);
            missileTransform.LookAt(targetTransform.position);

            yield return new WaitForFixedUpdate();
        }

        MissileExplosion();
    }

    IEnumerator TargetDetectionAround()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Collider[] hitColliders = Physics.OverlapSphere(missileTransform.position, triggerZoneRadius);

            foreach (var target in hitColliders)
            {
                if (target.TryGetComponent<IDamageable>(out IDamageable airDefenceTarget))
                {
                    MissileExplosion();
                    break;
                }
            }
        }
    }

    private void MissileExplosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(missileTransform.position, triggerZoneRadius);

        foreach (var target in hitColliders)
        {
            if (target.TryGetComponent<IDamageable>(out IDamageable _damageable))
            {
                _damageable.GetDamage(damage);
            }
        }

        MissileExplosionServerRpc();
    }

    [ServerRpc]
    private void MissileExplosionServerRpc()
    {
        NetworkObject explosionEffect = Instantiate(explosionEffectNetworkObject, missileTransform.position, Quaternion.identity);
        explosionEffect.Spawn();

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(missileTransform.position, triggerZoneRadius);
    }

    private void OnDisable()
    {
        StopCoroutine(FlyToTarget());
        StopCoroutine(TargetDetectionAround());
    }
}