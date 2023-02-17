using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBalance : MonoBehaviour
{
    [SerializeField] private Text _playerBalanceValueText;

    private int _playerBalanceValue = 0;

    public void GetIncome(int income)
    {
        _playerBalanceValue += income;
        UpdateBalanceTextValue();
    }

    public void UpdateBalanceTextValue()
    {
        _playerBalanceValueText.text = "$" + _playerBalanceValue.ToString();
    }
}
