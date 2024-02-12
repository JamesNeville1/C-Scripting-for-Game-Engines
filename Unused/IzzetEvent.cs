using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IzzetEvent : UnityEvent {
    private int listenerCount = 0;

    public int getListenerCount() {
        Debug.Log($"Listeners: {listenerCount}");
        return listenerCount;
    }

    public void addMyListener(UnityAction action) {
        AddListener(action);
        listenerCount++;
    }

    public void removeMyListener(UnityAction action) {
        RemoveListener(action);
        listenerCount--;
    }
}
