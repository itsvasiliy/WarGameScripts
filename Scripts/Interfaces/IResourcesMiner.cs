using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourcesMiner
{
    public void SetNewOwner(PlayerBalance playerBalance);

    public void IncomePerSecond();

    public Vector3 GetTransformPosition();
}
