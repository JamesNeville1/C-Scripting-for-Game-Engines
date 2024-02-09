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

    public void addUI(TextMeshProUGUI display) {
        this.display = display;

        updateDisplay();
    }

    public void zeroTrigger() { 
        onZero.Invoke();
    }

    public void endZeroTrigger() {
        onEndZero.Invoke();
    }

    public void adjust(int value) {
        if(value > 0) { 
            if(current == 0) {
                endZeroTrigger();
            }
        }
        current = Mathf.Clamp(current + value, 0, max);

        if(current <= 0) {
            zeroTrigger();
        }
        updateDisplay();
    }

    private void updateDisplay() {
        display.text = current.ToString();
    }

    public int returnCurrent() {
        return current;
    }
}