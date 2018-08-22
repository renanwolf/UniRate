using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption.Internal {

	public abstract class RateRequestBase {

		#region <<---------- Initializers ---------->>

		public RateRequestBase(int rateValue) {
			this._value = rateValue;
		}

		public RateRequestBase() : this(-1) { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		private int _value;

		/// <summary>
		/// Rate value.
		/// </summary>
		public int Value {
			get { return this._value; }
			set {
				if (this._value == value) return;
				this._value = value;
				this.OnChanged();
			}
		}

		/// <summary>
		/// Is a valid rate request?
		/// </summary>
		public virtual bool IsValid {
			get { return this._value > 0; }
		}

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- General ---------->>

		protected abstract void OnChanged();

		#endregion <<---------- General ---------->>
	}
	
	
	
	
	public abstract class RateRequestBase<T> : RateRequestBase where T : RateRequestBase {

		#region <<---------- Initializers ---------->>

		public RateRequestBase(int rateValue) : base(rateValue) {
			this.Changed = null;
		}

		public RateRequestBase() : base() { 
			this.Changed = null;
		}

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		/// <summary>
		/// Event raised when this request changes.
		/// </summary>
		public event Action<T> Changed;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- General ---------->>

		protected override void OnChanged() {
			var evnt = this.Changed;
			if (evnt == null) return;
			evnt(this as T);
		}

		/// <summary>
		/// Change the rate value.
		/// </summary>
		/// <returns>Returns this instance to use as fluent interface.</returns>
		public T WithRate(int rateValue) {
			this.Value = rateValue;
			return this as T;
		}

		#endregion <<---------- General ---------->>
	}
}