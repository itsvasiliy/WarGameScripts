using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    public void SelectUnit();

    public void DeselectUnit();

    public void SetTarget(Vector3 _targetPosition);
}

