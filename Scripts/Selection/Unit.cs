using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private void Start()
    {
        SetSelector(false);

        SelectionManager.unitList.Add(this);
    }

    public void SetSelector(bool active)
    {
        if (TryGetComponent<IUnit>(out IUnit _unit))
        {
            if (active)
                _unit.SelectUnit();
            else
                _unit.DeselectUnit();
        }
    }
}