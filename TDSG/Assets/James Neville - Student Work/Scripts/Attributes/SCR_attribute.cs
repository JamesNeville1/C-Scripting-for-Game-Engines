using IzzetUtils.IzzetAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SCR_attribute {
    [SerializeField] [MyReadOnly] private int current;
    [SerializeField] [MyReadOnly] private int max;
    [SerializeField] [MyReadOnly] private UnityEvent onZero = new UnityEvent();
    [SerializeField] [MyReadOnly] private UnityEvent onEndZero = new UnityEvent();
    [SerializeField] [MyReadOnly] private TextMeshProUGUI display;

    public SCR_attribute(int max, UnityAction onZeroDelegate, UnityAction onEndZeroDelegate = null) {
        this.current = max;
        this.max = max;
        
        onZero.AddListener(onZeroDelegate);
        onEndZero.AddListener(onEndZeroDelegate);
    }

    public void AddUI(TextMeshProUGUI display) {
        this.display = display;

        UpdateDisplay();
    }

    public void ZeroTrigger() { 
        onZero.Invoke();
    }

    public void EndZeroTrigger() {
        onEndZero.Invoke();
    }

    public void Adjust(int value) {
        if(value > 0) { 
            if(current == 0) {
                EndZeroTrigger();
            }
        }
        current = Mathf.Clamp(current + value, 0, max);

        if(current <= 0) {
            ZeroTrigger();
        }
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        display.text = current.ToString();
    }

    public int ReturnCurrent() {
        return current;
    }
}