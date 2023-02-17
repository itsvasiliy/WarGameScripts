using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class VehicleDragDrop : NetworkBehaviour, IEndDragHandler, IDragHandler
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private NetworkObject vehiclePrefab;

    [SerializeField] private Transform modelOfVehicle;

    private bool spawnCanceled = false;

    private void Awake() => modelOfVehicle.gameObject.SetActive(false);

    public void OnDrag(PointerEventData eventData)
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 500))
        {
            modelOfVehicle.gameObject.SetActive(true);
            modelOfVehicle.position = hit.point;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!spawnCanceled)
        {
            VehicleSpawnServerRpc(modelOfVehicle.position);
        }

        modelOfVehicle.gameObject.SetActive(false);
    }

    public void PointerEnterOrExit()
    {
        spawnCanceled = !spawnCanceled;
    }

    [ServerRpc]
    private void VehicleSpawnServerRpc(Vector3 spawnPosition)
    {
        NetworkObject spawnedVehicle = Instantiate(vehiclePrefab, spawnPosition, Quaternion.identity);
        spawnedVehicle.SpawnWithOwnership(OwnerClientId);
    }
}