using System.Collections;
using System.Collections.Generic;
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
        print(timeEvents[maxTimer].hasSubs());

        timeEvents[maxTimer].unsubscribe(remove);

        if (!timeEvents[maxTimer].hasSubs()) {
            Destroy(timeEvents[maxTimer].gameObject);
            timeEvents.Remove(maxTimer);
            return;
        }
    }

    public class timer : MonoBehaviour {
        float currentTimer;
        float maxTimer;
        IzzetEvent myEvent = new IzzetEvent();
        bool paused = false;

        public void setup(float maxTimer) {
            this.maxTimer = maxTimer;
            this.currentTimer = maxTimer;
        }

        public void subscribe(UnityAction onFinish) { myEvent.addMyListener(onFinish); }
        public void unsubscribe(UnityAction remove) { myEvent.removeMyListener(remove); }
        public void pauseTimer() { paused = true; }
        public void unpauseTimer() { paused = false; }
        public void resetTimer() { currentTimer = maxTimer; }
        public bool hasSubs() { print(myEvent.getListenerCount()); return myEvent.getListenerCount() > 0; }

        public void Update() {
            if(!paused) {
                if (currentTimer <= 0) {
                    myEvent.Invoke();
                    currentTimer = maxTimer;
                }
                else {
                    currentTimer -= Time.deltaTime;
                }
            }
        }
    }

}
