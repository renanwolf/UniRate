using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PWR.LowPowerMemoryConsumption.Examples {

    public class PWRExample_DelayEventVoid : MonoBehaviour {

        [SerializeField] private float _delayedSeconds;

        [SerializeField] private UnityEvent _delayedEvent;

        public void StartDelay() {
            this.StartCoroutine(this.DelayedCoroutine());
        }

        private IEnumerator DelayedCoroutine() {
            float start = Time.realtimeSinceStartup;
            do {
                yield return null;
            } while ( (Time.realtimeSinceStartup - start) < this._delayedSeconds );

            if (this._delayedEvent != null) {
                this._delayedEvent.Invoke();
            }
        }
    }
}