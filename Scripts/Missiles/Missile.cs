using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Missile : NetworkBehaviour, IUnit, IAirDefenceTarget, IDamageable
{
    #region Data

    private enum MissileState { idle, launch, flight, explosion, }

    [SerializeField] private NetworkObject missileNetworkObject;

    [SerializeField] private GameObject selector;

    [SerializeField] private Transform missileTransform;

    //[Header("ParticleSystems")]
    //[SerializeField] private ParticleSystem smokeExplosion;
    //[SerializeField] private ParticleSystem explosionLight;
    //[SerializeField] private ParticleSystem sparkExplosion;
    [SerializeField] private ParticleSystem exhaustParticleSystem;
    [SerializeField] private ParticleSystem missileExplosionParticleSystem;

    [Header("Floats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    private MissileState _missileState = MissileState.idle;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private float requiredHeight = 30f;
    private float startDistance;

    private bool isBroken = false;

    #endregion

    private void Start()
    {
        startPosition = missileTransform.position;
        exhaustParticleSystem.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        switch (_missileState)
        {
            case MissileState.idle:

                break;
            case MissileState.launch:

                missileTransform.Translate(Vector3.up * moveSpeed * Time.fixedDeltaTime);
                startDistance = Vector3.Distance(missileTransform.position, startPosition);

                if (startDistance >= requiredHeight)
                    _missileState = MissileState.flight;

                break;
            case MissileState.flight:
                missileTransform.Translate(Vector3.up * moveSpeed * Time.fixedDeltaTime);

                Vector3 targetVector = targetPosition - missileTransform.position;
                missileTransform.up = Vector3.Slerp(missileTransform.up, targetVector, rotateSpeed * Time.fixedDeltaTime);

                if (targetVector.magnitude < 1f)
                    _missileState = MissileState.explosion;

                break;
            case MissileState.explosion:
                MissileExplosionServerRpc();
                break;
        }
    }

    [ServerRpc]
    private void ExhaustActivationServerRpc() => ExhaustActivationClientRpc();

    [ClientRpc]
    private void ExhaustActivationClientRpc()
    {
        exhaustParticleSystem.gameObject.SetActive(true);
    }

    [ServerRpc]
    private void MissileExplosionServerRpc()
    {
        MissileExplosionClientRpc();

        missileNetworkObject.Despawn();
    }

    [ClientRpc]
    private void MissileExplosionClientRpc()
    {
        Instantiate(missileExplosionParticleSystem, missileTransform.position, Quaternion.identity);
    }

    #region Selection

    public void SelectUnit()
    {
        if (!IsOwner) return;

        if (_missileState.Equals(MissileState.idle))
            selector.SetActive(true);
    }

    public void DeselectUnit()
    {
        if (!IsOwner) return;

        selector.SetActive(false);
    }

    public void SetTarget(Vector3 _targetPosition)
    {
        if (!IsOwner) return;

        if (_missileState.Equals(MissileState.idle))
        {
            ExhaustActivationServerRpc();

            targetPosition = _targetPosition;
            _missileState = MissileState.launch;
        }
    }

    #endregion

    public void GetDamage(float damage)
    {
        if (!isBroken)
        {
            Destruction();
            isBroken = true;
        }
    }

    public void Destruction()
    {
       MissileExplosionServerRpc();
    }
}