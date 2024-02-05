using IzzetUtils.IzzetAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SCR_attribute {
    [SerializeField] [MyReadOnly] private int current;
    [SerializeField] [MyReadOnly] private int max;
    [SerializeField] [MyReadOnly] private UnityEvent onZero = new UnityEvent();
    [SerializeField] [MyReadOnly] private UnityEvent onEndZero = new UnityEvent();

    public SCR_attribute(int max, UnityAction onZeroDelegate, UnityAction onEndZeroDelegate = null) {
        this.current = max;
        this.max = max;
        
        onZero.AddListener(onZeroDelegate);
        onEndZero.AddListener(onEndZeroDelegate);
    }

    public void zeroTrigger() { 
        onZero.Invoke();
    }

    public void endZeroTrigger() {
        onEndZero.Invoke();
    }

    public void reduce(int value) { 
        if(current == 1 && value > 0) {
            Debug.Log("Trigger");
            zeroTrigger();
        }

        current -= value;
    }

    public void increase(int value) {
        if(current == 0 && value > 0) {
            endZeroTrigger();
        }

        current += value;
    }

    public int returnCurrent() {
        return current;
    }
}