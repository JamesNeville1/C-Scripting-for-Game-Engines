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

    public SCR_attribute(int max, UnityAction listener) {
        this.current = max;
        this.max = max;
        
        onZero.AddListener(listener);
    }

    public void trigger() { 
        onZero.Invoke();
    }

    public void reduce(int value) { 
        current -= value;

        if(current <= 0) {
            trigger();
        }
    }

    public int returnCurrent() {
        return current;
    }
}