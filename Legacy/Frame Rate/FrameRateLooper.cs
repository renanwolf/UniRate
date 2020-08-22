using System;
using System.Collections;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

    [Obsolete("OBSOLETE, this package will no longer provide the FrameRateLooper on next versions")]
    public class FrameRateLooper {

        #region <<---------- Properties and Fields ---------->>

        /// <summary>
        /// Indicates if loop is running.
        /// </summary>
        public bool IsLooping {
            get { return this._isLooping; }
            private set {
                if (this._isLooping == value) return;
                this._isLooping = value;
                this.OnIsLoopingChanged(this._isLooping);
            }
        }
        private bool _isLooping = false;

        /// <summary>
        /// Is modifiable if is not looping.
        /// </summary>
        public bool IsModifiable {
            get { return !this._isLooping; }
        }
        
        /// <summary>
        /// Minimum frame rate to keep while looping. (default: 30)
        /// </summary>
        public int FrameRateToKeep {
            get { return this._frameRateToKeep; }
            set {
                if (!this.IsModifiable) {
                    Debug.LogError("[" + this.GetType().Name + "] cannot set 'FrameRateToKeep' because 'IsModifiable' has returned false");
                    return;
                }
                this._frameRateToKeep = value;
            }
        }
        private int _frameRateToKeep = 30;

        /// <summary>
        /// Minimum loop cycles to execute per frame. (default: 2)
        /// </summary>
        public int MinCyclesPerFrame {
            get { return this._minCyclesPerFrame; }
            set {
                if (!this.IsModifiable) {
                    Debug.LogError("[" + this.GetType().Name + "] cannot set 'MinCyclesPerFrame' because 'IsModifiable' has returned false");
                    return;
                }
                this._minCyclesPerFrame = value;
            }
        }
        private int _minCyclesPerFrame = 2;

        /// <summary>
        /// Maximum loop cycles to execute per frame. (default: 10)
        /// </summary>
        public int MaxCyclesPerFrame {
            get { return this._maxCyclesPerFrame; }
            set {
                if (!this.IsModifiable) {
                    Debug.LogError("[" + this.GetType().Name + "] cannot set 'MaxCyclesPerFrame' because 'IsModifiable' has returned false");
                    return;
                }
                this._maxCyclesPerFrame = value;
            }
        }
        private int _maxCyclesPerFrame = 10;

        /// <summary>
        /// Timeout to wait to reach the <see cref="FrameRateToKeep"/>. Zero or less menas no timeout. (default: 1)
        /// </summary>
        public float TimeoutWaitForFrameRate {
            get { return this._timeoutWaitForFrameRate; }
            set {
                if (!this.IsModifiable) {
                    Debug.LogError("[" + this.GetType().Name + "] cannot set 'TimeoutWaitForFrameRate' because 'IsModifiable' has returned false");
                    return;
                }
                this._timeoutWaitForFrameRate = value;
            }
        }
        private float _timeoutWaitForFrameRate = 1f;

        /// <summary>
        /// Helper index to be used as you want!
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Helper count to be used as you want!
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Helper flag to be used as you want!
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        /// Event raised when loop starts.
        /// </summary>
        public event Action<FrameRateLooper> LoopStarted {
            add {
                this._loopStarted -= value;
                this._loopStarted += value;
            }
            remove {
                this._loopStarted -= value;
            }
        }
        private Action<FrameRateLooper> _loopStarted;

        /// <summary>
        /// Event raised when loop finish.
        /// </summary>
        public event Action<FrameRateLooper> LoopFinished {
            add {
                this._loopFinished -= value;
                this._loopFinished += value;
            }
            remove {
                this._loopFinished -= value;
            }
        }
        private Action<FrameRateLooper> _loopFinished;

        /// <summary>
        /// Event raised when <see cref="IsLooping"/> changes.
        /// </summary>
        public event Action<FrameRateLooper, bool> IsLoopingChanged {
            add {
                this._isLoopingChanged -= value;
                this._isLoopingChanged += value;
            }
            remove {
                this._isLoopingChanged -= value;
            }
        }
        private Action<FrameRateLooper, bool> _isLoopingChanged;

        private Func<FrameRateLooper, bool> _loopWhile;

        private Action<FrameRateLooper> _loopCycle;

        private MonoBehaviour _behaviour;

        private Coroutine _coroutine;

        private FrameRateRequest _frameRateRequest;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Callbacks ---------->>
        
        protected virtual void OnIsLoopingChanged(bool isLooping) {
            var evnt = this._isLoopingChanged;
            if (evnt != null) {
                evnt(this, isLooping);
            }
            
            if (isLooping) this.OnLoopStarted();
            else this.OnLoopFinished();
        }

        protected virtual void OnLoopStarted() {
            this._frameRateRequest = FrameRateManager.Instance.StartRequest(FrameRateType.FPS, this.GetMinFrameRateToKeep());

            var evnt = this._loopStarted;
            if (evnt == null) return;
            evnt(this);
        }

        protected virtual void OnLoopFinished() {
            FrameRateManager.Instance.StopRequest(this._frameRateRequest);

            var evnt = this._loopFinished;
            if (evnt == null) return;
            evnt(this);
        }
        
        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>

        /// <summary>
        /// Start the loop.
        /// </summary>
        public void Start(MonoBehaviour behaviour) {
            this.Stop();
            this._behaviour = behaviour;
            this._coroutine = this._behaviour.StartCoroutine(this.Coroutine());
        }

        /// <summary>
        /// Stop the loop.
        /// </summary>
        public void Stop() {
            if (this._behaviour != null && this._coroutine != null) {
                this._behaviour.StopCoroutine(this._coroutine);
            }
            this._behaviour = null;
            this._coroutine = null;
            this.IsLooping = false;
        }

        /// <summary>
        /// Loop will be perfomed while this handler returns true.
        /// </summary>
        public void LoopWhile(Func<FrameRateLooper, bool> loopWhile) {
            if (!this.IsModifiable) {
                Debug.LogError("[" + this.GetType().Name + "] cannot set 'LoopWhile' because 'IsModifiable' has returned false");
                return;
            }
            this._loopWhile = loopWhile;
        }

        /// <summary>
        /// Loop step/cycle handler.
        /// </summary>
        public void LoopCycle(Action<FrameRateLooper> loopCycle) {
            if (!this.IsModifiable) {
                Debug.LogError("[" + this.GetType().Name + "] cannot set 'LoopCycle' because 'IsModifiable' has returned false");
                return;
            }
            this._loopCycle = loopCycle;
        }
        
        protected IEnumerator Coroutine() {
            if (this._minCyclesPerFrame < 1) {
                throw new InvalidOperationException("'MinCyclesPerFrame' needs to be greater then 1");
            }
            if (this._maxCyclesPerFrame < this._minCyclesPerFrame) {
                throw new InvalidOperationException("'MaxCyclesPerFrame' needs to be greater or equals to 'MinCyclesPerFrame'");
            }
            if (this._loopWhile == null) {
                throw new NullReferenceException("'LoopWhile' handler is null");
            }
            if (this._loopCycle == null) {
                throw new NullReferenceException("'LoopCycle' handler is null");
            }

            this.IsLooping = true;

            int minFrameRate = this.GetMinFrameRateToKeep();
            float maxDeltaTime = 1.0f / (float)minFrameRate;
            int cyclesPerFrame = this._minCyclesPerFrame;
            int cycles;
            bool loopWhile;

            do {

                //perform loop
                cycles = 0;
                do {
                    this._loopCycle(this);
                    cycles += 1;
                    loopWhile = this._loopWhile(this);
                } while (this._isLooping && loopWhile && cycles < cyclesPerFrame);

                if (!loopWhile || !this._isLooping) break;
                yield return null;

                //recalculate cycles per frame
                float deltaTime = Time.unscaledDeltaTime;
                cyclesPerFrame = Mathf.FloorToInt((float)cycles * maxDeltaTime / deltaTime);
                if (cyclesPerFrame > this._maxCyclesPerFrame) cyclesPerFrame = this._maxCyclesPerFrame;
                else if (cyclesPerFrame < this._minCyclesPerFrame) cyclesPerFrame = this._minCyclesPerFrame;

                //wait to reach minimum frame rate or timeout
                float startWait = Time.realtimeSinceStartup;
                float elapsedWait;
                while (deltaTime > maxDeltaTime) {
                    yield return null;
                    deltaTime = Time.unscaledDeltaTime;
                    //check if timeout is enabled
                    if (this._timeoutWaitForFrameRate > 0f) {
                        elapsedWait = Time.realtimeSinceStartup - startWait;
                        if (elapsedWait >= this._timeoutWaitForFrameRate) break;
                    }
                }

            } while(this._isLooping && loopWhile);
            this.IsLooping = false;
        }

        protected int GetMinFrameRateToKeep() {
            return Mathf.Max(1, this._frameRateToKeep);
        }

        #endregion <<---------- General ---------->>
    }
}