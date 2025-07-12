using System;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;
    [SerializeField] private TextMeshProUGUI flareAmountText;

    private void Awake() {
        Instance = this;
    }
    
    private void Update()
    {
        flareAmountText.text = Player.Instance.GetFlareAmount().ToString();
    }
}
