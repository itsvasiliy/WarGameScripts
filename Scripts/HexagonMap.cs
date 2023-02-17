using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonMap : MonoBehaviour
{
    [SerializeField] private Hexagon hexagonPrefab;

    [SerializeField] private Transform hexagonMapTransform;

    [SerializeField] private Material hexagonRedMaterial;
    [SerializeField] private Material hexagonBlueMaterial;
    [SerializeField] private Material hexagonGrayMaterial;

    private void Start()
    {
        Vector3 hexagonSpawnPosition = new Vector3(-250f, 0f, 250f);
        bool isSecondLine = false;

        for (int i = 0; i < 59; i++)
        {
            for (int j = 0; j < 51; j++)
            {
                Hexagon hexagonClone = Instantiate(hexagonPrefab,
                    hexagonSpawnPosition, Quaternion.identity, hexagonMapTransform);

                if (hexagonSpawnPosition.z >= 0)
                {

                    if (hexagonSpawnPosition.z < 1f)
                    {
                        hexagonClone.GetComponent<Renderer>().material = hexagonGrayMaterial;

                    }
                    else
                    {
                        hexagonClone.GetComponent<Renderer>().material = hexagonRedMaterial;

                    }
                }
                else
                {
                    hexagonClone.GetComponent<Renderer>().material = hexagonBlueMaterial;

                }

                hexagonSpawnPosition.x += 10f;
            }

            if (isSecondLine)
                hexagonSpawnPosition.x = -250f;
            else
                hexagonSpawnPosition.x = -245f;

            isSecondLine = !isSecondLine;
            hexagonSpawnPosition.z -= 8.6f;
        }
    }
}
