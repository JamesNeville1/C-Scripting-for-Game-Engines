using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class SCR_master_timers : MonoBehaviour {

    [SerializeField] private Transform timerParent;

    private Dictionary<string, timer> timeEvents = new Dictionary<string, timer>(); //Holds the current timers

    private static SCR_master_timers instance;

    private void Awake() {
        instance = this;
    }

    public static SCR_master_timers returnTickSystem() {
        return instance;
    }

    public void subscribe(string id, Action onFinish, float maxTimer = 0) {
        if(!timeEvents.ContainsKey(id)) {
            timer newTimer = new GameObject($"{id} Timer", typeof(timer)).GetComponent<timer>();
            
            newTimer.gameObject.transform.parent = timerParent;
            newTimer.setup(maxTimer);
            
            timeEvents.Add(id, newTimer);
        }
        timeEvents[id].subscribe(onFinish);
    }
    public void removeAll(string id) {
        Destroy(timeEvents[id].gameObject);
        timeEvents.Remove(id);
    }

    public class timer : MonoBehaviour {
        float currentTimer;
        float maxTimer;
        event Action myEvent;
        bool paused = false;

        public void setup(float maxTimer) {
            this.maxTimer = maxTimer;
            this.currentTimer = maxTimer;
        }

        public void subscribe(Action onFinish) { myEvent += onFinish; }
        public void unsubscribe(Action remove) { myEvent -= remove; }
        public void pauseTimer() { paused = true; }
        public void unpauseTimer() { paused = false; }
        public void resetTimer() { currentTimer = maxTimer; }
        public bool hasSubs() { return myEvent.GetInvocationList().Length > 0; }

        public void Update() {
            if(!paused) {
                if (currentTimer <= 0) {
                    currentTimer = maxTimer;
                    myEvent.Invoke();
                }
                else {
                    currentTimer -= Time.deltaTime;
                }
            }
        }
    }

}
