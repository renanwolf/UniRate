using System;
using UnityEngine;
using UniRate;

namespace PWR.LowPowerMemoryConsumption {

    [DisallowMultipleComponent]
	[Obsolete("OBSOLETE, use RateManager inside UniRate namespace instead.")]
	public class RenderIntervalManager : MonoBehaviour {

		#region <<---------- Initializers ---------->>

		protected RenderIntervalManager() { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		[Obsolete("OBSOLETE, use RateManager.Instance.MaximumRenderInterval instead.")]
		public int FallbackRenderInterval {
			get => RateManager.Instance.MaximumRenderInterval;
			set => RateManager.Instance.MaximumRenderInterval = value;
		}

		[Obsolete("OBSOLETE, use RateManager.Instance.RenderInterval.Current instead.")]
		public int RenderInterval => RateManager.Instance.RenderInterval.Current;

		[Obsolete("OBSOLETE, use RateManager.Instance.WillRender instead.")]
		public bool IsRendering => RateManager.Instance.WillRender;

		[Obsolete("OBSOLETE, use RateManager.Instance.RenderIntervalChanged instead.")]
		public event Action<RenderIntervalManager, int> RenderIntervalChanged {
			add {
				this._renderIntervalChanged -= value;
				this._renderIntervalChanged += value;
			}
			remove {
				this._renderIntervalChanged -= value;
			}
		}
		private Action<RenderIntervalManager, int> _renderIntervalChanged;

		[Obsolete(null, true)]
		public event Action<RenderIntervalManager, bool> IsRenderingChanged {
			add { }
			remove { }
		}

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			RateManager.Instance.RenderIntervalChanged += this.OnRenderIntervalChanged;
		}

		protected virtual void OnDestroy() {
			var rateManager = RateManager.Instance;
			if (rateManager == null) return;
			rateManager.RenderIntervalChanged -= this.OnRenderIntervalChanged;
		}

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Callbacks ---------->>

		private void OnRenderIntervalChanged(RateManager manager, int interval) {
			var e = this._renderIntervalChanged;
			if (e == null) return;
			e(this, interval);
		}

		#endregion <<---------- Callbacks ---------->>




		#region <<---------- Requests Management ---------->>

		[Obsolete("OBSOLETE, use RateManager.Instance.RequestRenderInterval() instead")]
		public RenderIntervalRequest StartRequest(int interval) {
			if (interval <= 0) return RenderIntervalRequest.Invalid;
			var request = RateManager.Instance.RequestRenderInterval(interval);
			if (request == null) return RenderIntervalRequest.Invalid;
			return new RenderIntervalRequest(request);
		}

		public void StopRequest(RenderIntervalRequest request) {
			if (request.UniRateRequest == null || request.UniRateRequest.IsDisposed) return;
			request.UniRateRequest.Dispose();
		}

		#endregion <<---------- Requests Management ---------->>
	}
}