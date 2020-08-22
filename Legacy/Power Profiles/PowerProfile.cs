using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PWR.LowPowerMemoryConsumption {

    [Serializable]
    [Obsolete]
    public abstract class PowerProfile : ScriptableObject {
        
        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] private string _identifier;

        /// <summary>
		/// Identifier to help find this object on <see cref="Instances"/> enumerable.
		/// </summary>
		public string Identifier {
			get { return this._identifier; }
			set { this._identifier = value; }
		}

        /// <summary>
        /// Indicates if retain count is greater than zero.
        /// </summary>
        public bool IsRetained {
            get { return this._isRetained; }
            private set {
                if (this._isRetained == value) return;
                this._isRetained = value;
                this.OnIsRetainedChanged(this._isRetained);
            }
        }
        private bool _isRetained;

        /// <summary>
        /// Retain count.
        /// </summary>
        public int RetainCount {
            get { return this._retainCount; }
        }
        private int _retainCount = 0;

        /// <summary>
		/// All enabled instances.
		/// </summary>
		public static IEnumerable<PowerProfile> Instances {
			get {
				if (_instances != null) return _instances;
				return Enumerable.Empty<PowerProfile>();
			}
		}
        private static HashSet<PowerProfile> _instances;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Scriptable Object ---------->>
        
        #if UNITY_EDITOR
        protected virtual void Awake() {
            EditorApplication.playModeStateChanged += (state) => {
                if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingEditMode) {
                    this._retainCount = 0;
                }
            };
        }
        #endif
        
        protected virtual void OnEnable() {
            this.AddToStaticInstances();
        }

        protected virtual void OnDisable() {
            this.RemoveFromStaticInstances();
        }
        
        #endregion <<---------- Scriptable Object ---------->>




        #region <<---------- Callbacks ---------->>
        
        protected abstract void OnIsRetainedChanged(bool isRetained);
        
        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>
        
        /// <summary>
        /// Increase retain count to keep profile requests active.
        /// </summary>
        public void Retain() {
            #if UNITY_EDITOR
            if (!Application.isPlaying) {
                Debug.LogWarning("[" + this.GetType().Name + "/" + this.name + "] Retain() is only allowed while in play mode", this);
            }
            #endif
            this._retainCount += 1;
            if (this._retainCount < 0) {
                Debug.LogWarning("[" + this.GetType().Name + "/" + this.name + "] retain count is " + this._retainCount, this);
            }
            this.IsRetained = this._retainCount > 0;
        }

        /// <summary>
        /// Decrease retain count. When reach zero, profile requests will be stoped.
        /// </summary>
        public void Release() {
            #if UNITY_EDITOR
            if (!Application.isPlaying) {
                Debug.LogWarning("[" + this.GetType().Name + "/" + this.name + "] Release() is only allowed while in play mode", this);
            }
            #endif
            this._retainCount -= 1;
            if (this._retainCount < 0) {
                Debug.LogWarning("[" + this.GetType().Name + "/" + this.name + "] retain count is " + this._retainCount, this);
            }
            this.IsRetained = this._retainCount > 0;
        }

        /// <summary>
        /// Retain or release.
        /// </summary>
        /// <param name="retain">If true will retain, otherwise release.</param>
        public void RetainOrRelease(bool retain) {
            if (retain) this.Retain();
            else this.Release();
        }

        /// <summary>
        /// Release or retain.
        /// </summary>
        /// <param name="release">If true will release, otherwise retain.</param>
        public void ReleaseOrRetain(bool release) {
            if (release) this.Release();
            else this.Retain();
        }

        /// <summary>
        /// All enabled instances of type T.
        /// </summary>
        public static IEnumerable<T> InstancesOfType<T>() where T : PowerProfile {
            return Instances.Where(i => i.GetType() == typeof(T)).Select(i => (T)i);
        }

        private void AddToStaticInstances() {
			if (_instances == null) _instances = new HashSet<PowerProfile>();
			_instances.Add(this);
		}

		private void RemoveFromStaticInstances() {
			if (_instances == null) return;
			_instances.Remove(this);
		}
        
        #endregion <<---------- General ---------->>
    }
}