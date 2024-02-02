using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.Events;

public class SCR_tick_system : MonoBehaviour {

    [SerializeField] private Transform timerParent;

    private Dictionary<float, timer> timeEvents = new Dictionary<float, timer>(); //Holds the current timers

    private static SCR_tick_system instance;

    private void Awake() {
        instance = this;
    }

    public static SCR_tick_system returnTickSystem() {
        return instance;
    }

    public void subscribe(float maxTimer, UnityAction onFinish) {
        if(!timeEvents.ContainsKey(maxTimer)) {
            timer newTimer = new GameObject($"{maxTimer} Timer", typeof(timer)).GetComponent<timer>();
            newTimer.gameObject.transform.parent = timerParent;
            newTimer.setup(maxTimer);
            timeEvents.Add(maxTimer, newTimer);
        }
        timeEvents[maxTimer].subscribe(onFinish);
    }
    public void unsubscribe(float maxTimer, UnityAction remove) {
        timeEvents[maxTimer].unsubscribe(remove);
    }

    public class timer : MonoBehaviour {
        float currentTimer;
        float maxTimer;
        UnityEvent uniEvent = new UnityEvent();
        bool paused = false;

        public void setup(float maxTimer) {
            this.maxTimer = maxTimer;
            this.currentTimer = maxTimer;
        }

        public void subscribe(UnityAction onFinish) {
            uniEvent.AddListener(onFinish);
        }
        public void unsubscribe(UnityAction remove) {
            uniEvent.RemoveListener(remove);
        }
        public void pauseTimer() {
            paused = true;
        }
        public void unpauseTimer() {
            paused = false;
        }
        public void resetTimer() {
            currentTimer = maxTimer;
        }

        public void Update() {
            if(!paused) {
                if (currentTimer <= 0) {
                    uniEvent.Invoke();
                    currentTimer = maxTimer;
                }
                else {
                    currentTimer -= Time.deltaTime;
                }
            }
        }
    }

}
