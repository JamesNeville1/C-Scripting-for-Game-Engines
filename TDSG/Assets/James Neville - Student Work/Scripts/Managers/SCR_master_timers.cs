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
    public static SCR_master_timers instance { get; private set; }

    private void Awake() {
        instance = this;
    }
    #endregion

    #region Public
    public void Subscribe(timerID id, Action onFinish) {
        if(!timeEvents.ContainsKey(id)) {
            timer newTimer = new GameObject($"{id} Timer", typeof(timer)).GetComponent<timer>();
            
            newTimer.gameObject.transform.parent = timerParent;
            newTimer.Setup(timerLengths[id]);
            
            timeEvents.Add(id, newTimer);
        }

        timeEvents[id].Subscribe(onFinish);
    }
    public void RemoveAll(timerID id) {
        if(timeEvents.ContainsKey(id)) {
            Destroy(timeEvents[id].gameObject);
            timeEvents.Remove(id);
        }
    }
    public void Pause(timerID id) {
        timeEvents[id].PauseTimer();
    }
    public void Unpause(timerID id) {
        timeEvents[id].UnpauseTimer();
    }
    #endregion

    #region Timer Class
    public class timer : MonoBehaviour {
        [SerializeField] [MyReadOnly] private float currentTimer;
        [SerializeField] [MyReadOnly] private float maxTimer;
        private event Action myEvent;
        [SerializeField] [MyReadOnly] private bool paused = false;

        public void Setup(float maxTimer) {
            this.maxTimer = maxTimer;
            this.currentTimer = maxTimer;
        }

        public void Subscribe(Action onFinish) { myEvent += onFinish; }
        public void Unsubscribe(Action remove) { myEvent -= remove; }
        public void PauseTimer() { paused = true; }
        public void UnpauseTimer() { paused = false; }
        public void ResetTimer() { currentTimer = maxTimer; }
        public bool HasSubs() { return myEvent.GetInvocationList().Length > 0; }

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
