using IzzetUtils.IzzetAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class SCR_master_timers : MonoBehaviour {

    [SerializeField] private Transform timerParent;

    [System.Serializable] public enum timerID {
        HUNGER_TICK,
        HUNGER_DAMAGE_TICK,
        WAIT_AFTER_DEATH,
    }

    [SerializedDictionary("ID", "Length")] [SerializeField]
    private SerializedDictionary<timerID, float> timerLengths = new SerializedDictionary<timerID, float>();
    private Dictionary<timerID, timer> timeEvents = new Dictionary<timerID, timer>(); //Holds the current timers

    #region Set Instance
    private static SCR_master_timers instance;

    private void Awake() {
        instance = this;
    }

    public static SCR_master_timers returnInstance() {
        return instance;
    }
    #endregion
    #region Public
    public void subscribe(timerID id, Action onFinish) {
        if(!timeEvents.ContainsKey(id)) {
            timer newTimer = new GameObject($"{id} Timer", typeof(timer)).GetComponent<timer>();
            
            newTimer.gameObject.transform.parent = timerParent;
            newTimer.setup(timerLengths[id]);
            
            timeEvents.Add(id, newTimer);
        }
        timeEvents[id].subscribe(onFinish);
    }
    public void removeAll(timerID id) {
        if(timeEvents.ContainsKey(id)) {
            Destroy(timeEvents[id].gameObject);
            timeEvents.Remove(id);
        }
    }
    public void pause(timerID id) {
        timeEvents[id].pauseTimer();
    }
    public void unpause(timerID id) {
        timeEvents[id].unpauseTimer();
    }
    #endregion
    #region Timer Class
    public class timer : MonoBehaviour {
        [SerializeField] [MyReadOnly] private float currentTimer;
        [SerializeField] [MyReadOnly] private float maxTimer;
        private event Action myEvent;
        [SerializeField] [MyReadOnly] private bool paused = false;

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
    #endregion
}
