using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ExplosiveAmmunition : NetworkBehaviour
{
    [SerializeField] private ParticleSystem explosion;

    [SerializeField] private NetworkObject _networkObject;

    private float bulletDamage = 0f;

    public float BulletDamage
    {
        get { return bulletDamage; }
        set { bulletDamage = value; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        SpawnExplosionServerRpc(transform.position);

        if(collision.gameObject.TryGetComponent<IDamageable>(out IDamageable _damageable))
        {
            _damageable.GetDamage(bulletDamage);
        }

        DestructionServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnExplosionServerRpc(Vector3 _spawnPosition)
    {
        Instantiate(explosion, _spawnPosition, Quaternion.identity)
            .GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestructionServerRpc()
    {
        _networkObject.Despawn(true);
    }
}
