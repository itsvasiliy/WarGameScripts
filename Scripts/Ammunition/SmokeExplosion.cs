using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SmokeExplosion : MonoBehaviour
{
    [SerializeField] private NetworkObject _networkObject;

    [SerializeField] private float lifeTime;

    private void Start()
    {
        //if (!IsOwner) return;

        Explosion();
        Invoke(nameof(DestructionServerRpc), lifeTime);
    }

    private void Explosion()
    {

    }

    [ServerRpc(RequireOwnership = false)]
    private void DestructionServerRpc()
    {
        _networkObject.Despawn(true);
    }
}