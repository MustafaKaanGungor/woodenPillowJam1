using System;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI flareAmountText;

    private void Update()
    {
        flareAmountText.text = Player.Instance.GetFlareAmount().ToString();
    }
}
