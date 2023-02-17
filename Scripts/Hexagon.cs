using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    private enum HexagonStates { player1, neutral, player2 }

    [SerializeField] private Transform hexagonTransform;

    [SerializeField] private Renderer hexagonRenderer;

    [SerializeField] private Material hexagonRedMaterial;
    [SerializeField] private Material hexagonBlueMaterial;
    [SerializeField] private Material hexagonGrayMaterial;

    private HexagonStates _hexagonState = HexagonStates.neutral;

    private void Start()
    {
        StartCoroutine(enumeratorName());
    }

    IEnumerator enumeratorName()
    {
        while (true)
        {
            RaycastHit raycastHit;

            bool m_HitDetect = Physics.BoxCast(hexagonTransform.position, hexagonTransform.localScale * 3.3f,
                Vector3.up, out raycastHit, Quaternion.identity, 5f);

            if (m_HitDetect && raycastHit.collider.TryGetComponent<LandVehicle>(out LandVehicle _landVehicle))
            {
                switch (_hexagonState)
                {
                    case HexagonStates.player1:

                        if(!_landVehicle.IsOwnedByServer)
                        {
                            hexagonRenderer.material = hexagonRedMaterial;
                            _hexagonState = HexagonStates.player2;
                        }

                        break;
                    case HexagonStates.neutral:

                        if(_landVehicle.IsOwnedByServer)
                        {
                            _hexagonState = HexagonStates.player1;
                            hexagonRenderer.material = hexagonBlueMaterial;
                        }
                        else
                        {
                            _hexagonState = HexagonStates.player2;
                            hexagonRenderer.material = hexagonRedMaterial;
                        }

                        break;
                    case HexagonStates.player2:

                        if(_landVehicle.IsOwnedByServer)
                        {
                            hexagonRenderer.material = hexagonBlueMaterial;
                            _hexagonState = HexagonStates.player1;
                        }

                        break;
                }
            }

            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private void OnDisable()
    {
        StopCoroutine(enumeratorName());
    }
}