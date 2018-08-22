using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class RenderRateManager : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		private Camera _attachedCamera;

		public Camera AttachedCamera {
			get {
				if (this._attachedCamera == null) {
					this._attachedCamera = this.GetComponent<Camera>();
				}
				return this._attachedCamera;
			}
		}

		private bool _isRendering = false;
		public bool IsRendering {
			get { return this._isRendering; }
			private set {
				if (this._isRendering == value) return;
				this._isRendering = value;
				this.OnIsRenderingChanged();
			}
		}

		private int _framesSinceLastRender = int.MaxValue;

		public int TargetRenderRate = 50;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void OnEnable() {
			this._framesSinceLastRender = 0;
			this.AssertIsRenderingFlag();
		}

		protected virtual void Update() {

			this._framesSinceLastRender += 1;
			if (this._framesSinceLastRender < this.TargetRenderRate) {
				if (this._isRendering) this.StopRendering();
				return;
			}

			if (!this._isRendering) this.StartRendering();
		}

		protected virtual void OnDisable() {
			this.AssertIsRenderingFlag();
		}

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Camera Messages ---------->>

		protected virtual void OnPostRender() {
			Debug.Log("OnPostRender");
			//this.StopRendering();
			this._framesSinceLastRender = 0;
		}

		#endregion <<---------- Camera Messages ---------->>




		#region <<---------- General ---------->>

		protected virtual void OnIsRenderingChanged() {

		}

		protected void AssertIsRenderingFlag() {
			this.IsRendering = this.isActiveAndEnabled && this.AttachedCamera.enabled;
		}

		private void StartRendering() {
			this.AttachedCamera.enabled = true;
			this.AssertIsRenderingFlag();
		}

		private void StopRendering() {
			this.AttachedCamera.enabled = false;
			this.AssertIsRenderingFlag();
		}

		#endregion <<---------- General ---------->>
	}
}