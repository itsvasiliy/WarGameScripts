using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft : MonoBehaviour
{
    [SerializeField] private Transform aircraftTransform;

    [SerializeField] private float moveSpeed;

    private void FixedUpdate()
    {
        aircraftTransform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
