using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class RenderIntervalRequest {

		#region <<---------- Initializers ---------->>

		public RenderIntervalRequest(int interval) {
			this._interval = interval;
		}

		public RenderIntervalRequest() : this(MinInterval - 1) { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Static Creators ---------->>

		/// <summary>
		/// Create a new render interval.
		/// </summary>
		/// <returns>Returns the new created request.</returns>
		public static RenderIntervalRequest WithInterval(int interval) {
			return new RenderIntervalRequest(interval);
		}		

		#endregion <<---------- Static Creators ---------->>




		#region <<---------- Properties and Fields ---------->>

		public RenderIntervalManager Manager { get; private set; }

		/// <summary>
		/// Render interval value.
		/// </summary>
		public int Interval {
			get { return this._interval; }
			set {
				if (this._interval == value) return;
				this._interval = value;
				this.OnChanged();
			}
		}
		private int _interval;

		/// <summary>
		/// Event raised when this request changes.
		/// </summary>
		public event Action<RenderIntervalRequest> Changed {
			add {
				this._changed -= value;
				this._changed += value;
			}
			remove {
				this._changed -= value;
			}
		}
		private Action<RenderIntervalRequest> _changed;

		/// <summary>
		/// Is valid if interval value is greather or equals to <see cref="MinInterval"/>.
		/// </summary>
		public bool IsValid {
			get { return this._interval >= MinInterval; }
		}

		/// <summary>
		/// Minimum interval valid value.
		/// </summary>
		public const int MinInterval = 1;

		#endregion <<---------- Properties and Fields ---------->>
		
		
		
		
		#region <<---------- General ---------->>

		protected virtual void OnChanged() {
			var evnt = this._changed;
			if (evnt == null) return;
			evnt(this);
		}

		/// <summary>
		/// Start the render interval request.
		/// </summary>
		/// <param name="manager">The render interval manager to add the request.</param>
		/// <returns>Returns this instance to use chaining pattern.</returns>
		public RenderIntervalRequest Start(RenderIntervalManager manager) {
			if (manager == null) return null;
			if (this.Manager != null && this.Manager != manager) this.Stop();
			this.Manager = manager;
			return this.Manager.AddRequest(this);
		}

		/// <summary>
		/// Stop the render interval request on <see cref="Manager"/>.
		/// </summary>
		public void Stop() {
			if (this.Manager == null) return;
			this.Manager.RemoveRequest(this);
			this.Manager = null;
		}

		#endregion <<---------- General ---------->>
	}
}