using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

    [CreateAssetMenuAttribute(menuName="PWR/Power Profiles/Render Interval",order=1002)]
	[Obsolete]
    public class PowerProfileRenderInterval : PowerProfile {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField] private RenderIntervalManagerPointer _managerPointer;

        [SerializeField][Range(RenderIntervalRequest.MinInterval, 60)] private int _interval = RenderIntervalRequest.MinInterval;

        /// <summary>
		/// Render interval value.
		/// </summary>
		public int Interval {
			get { return this._interval; }
			set {
				this._interval = value;
				if (!Application.isPlaying || this._request.Interval == this._interval) return;
				this.OnIsRetainedChanged(this.IsRetained);
			}
		}

		/// <summary>
		/// Manager pointer.
		/// </summary>
		public RenderIntervalManagerPointer ManagerPointer {
			get {
				if (this._managerPointer == null) {
					this._managerPointer = new RenderIntervalManagerPointer();
					if (Application.isPlaying) {
						var mngr = this.ManagerPointer.GetManager();
						int myManagerInstanceID = mngr == null ? -1 : mngr.GetInstanceID();
						if (myManagerInstanceID != this._request.ManagerInstanceID) {
							this.OnIsRetainedChanged(this.IsRetained);
						}
					}
				}
				return this._managerPointer;
			}
		}

        private RenderIntervalRequest _request;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- SriptableObject ---------->>

		protected override void OnEnable() {
			base.OnEnable();
			this.ManagerPointer.Changed += (pointer) => {
				if (!Application.isPlaying || this == null || !this.IsRequestValuesDifferentFromFields()) return;
				this.OnIsRetainedChanged(this.IsRetained);
			};
		}
        
        #if UNITY_EDITOR

        protected virtual void OnValidate() {
            if (!Application.isPlaying) return;
            this.OnIsRetainedChanged(this.IsRetained);
        }

        protected virtual void Reset() {
            if (!Application.isPlaying) return;
            this.OnIsRetainedChanged(this.IsRetained);
        }

        #endif
        
        #endregion <<---------- SriptableObject ---------->>




        #region <<---------- Callbacks ---------->>
        
        protected override void OnIsRetainedChanged(bool isRetained) {
            var mngr = this.ManagerPointer.GetManager();
			if (mngr == null) return;
			mngr.StopRequest(this._request);
			if (!isRetained) {
				this._request = RenderIntervalRequest.Invalid;
				return;
			}
			this._request = mngr.StartRequest(this._interval);
        }
        
        #endregion <<---------- Callbacks ---------->>




		#region <<---------- General ---------->>
		
		private bool IsRequestValuesDifferentFromFields() {
			if (this._request.Interval != this._interval) return true;
			var mngr = this.ManagerPointer.GetManager();
			int myManagerInstanceID = mngr == null ? -1 : mngr.GetInstanceID();
			return this._request.ManagerInstanceID != myManagerInstanceID;
		}
		
		#endregion <<---------- General ---------->>
    }
}