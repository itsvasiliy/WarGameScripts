using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private NetworkObject _player;
    [SerializeField] private NetworkObject missilePrefab;

    [SerializeField] private Transform playerCameraTransform;

    [SerializeField] private PlayerBalance playerBalance;

    private static int ConnectedPlayersCount = 0;

    private void Awake()
    {
        ConnectedPlayersCount++;
    }

    private void Start()
    {
        if (!_player.IsLocalPlayer)
            gameObject.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        //IResourcesMiner[] miners = (IResourcesMiner[])FindObjectsOfType<MonoBehaviour>().OfType<IResourcesMiner>();
        //IResourcesMiner[] miners = FindObjectsOfType<IResourcesMiner>();

        //var miners = (IResourcesMiner[])FindObjectsOfType<MonoBehaviour>().OfType<IResourcesMiner>();

        IResourcesMiner[] miners = FindObjectsOfType<MonoBehaviour>().OfType<IResourcesMiner>().ToArray();

        switch (ConnectedPlayersCount)
        {
            case 1:

                playerCameraTransform.position = new Vector3(0f, 70f, -100f);
                playerCameraTransform.rotation = Quaternion.Euler(45f, 0f, 0f);

                foreach (IResourcesMiner miner in miners)
                {
                    if (miner.GetTransformPosition().z < 0f)
                    {
                        miner.SetNewOwner(playerBalance);
                        miner.IncomePerSecond();
                    }
                }

                break;
            case 2:

                playerCameraTransform.position = new Vector3(0f, 70f, 100f);
                playerCameraTransform.rotation = Quaternion.Euler(45f, 180f, 0f);

                foreach (IResourcesMiner miner in miners)
                {
                    print("FFF");
                    if (miner.GetTransformPosition().z > 0f)
                    {
                        miner.SetNewOwner(playerBalance);
                        //miner.IncomePerSecond();
                    }

                    //miner.IncomePerSecond();
                }

                break;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(1))
        {
            spawnMisileServerRpc();
        }
    }

    [ServerRpc]
    private void spawnMisileServerRpc()
    {
        NetworkObject networkObject = Instantiate(missilePrefab);
        networkObject.SpawnWithOwnership(OwnerClientId);
    }
}
