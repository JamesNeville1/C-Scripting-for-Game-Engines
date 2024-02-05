using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SCR_master_stats_display : MonoBehaviour {
    private static SCR_master_stats_display instance;
    public static SCR_master_stats_display returnInstance() {
        return instance;
    }
    private void Awake() {
        instance = this;
    }

    [SerializeField] private TextMeshProUGUI healthUI;
    [SerializeField] private TextMeshProUGUI hungerUI;

    public TextMeshProUGUI returnHealthUI() {
        return healthUI;
    }
    public TextMeshProUGUI returnHungerUI() {
        return hungerUI;
    }

}
