using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] private GameObject partOfGridRed;
    [SerializeField] private GameObject partOfGridBlue;
    [SerializeField] private GameObject partOfGridGray;

    private void Start()
    {
        for (int z = -120; z <= 120; z += 10)
        {
            for (int x = -120; x <= 120; x += 10)
            {
                if (z > 0f)
                    Instantiate(partOfGridRed, new Vector3(x, 0.01f, z), Quaternion.identity, transform);
                else if (z == 0f)
                    Instantiate(partOfGridGray, new Vector3(x, 0.01f, z), Quaternion.identity, transform);
                else
                    Instantiate(partOfGridBlue, new Vector3(x, 0.01f, z), Quaternion.identity, transform);
            }
        }
    }
}