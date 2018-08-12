//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PWR.LowPowerMemoryConsumption {

	public class FrameRateRequester : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private FrameRateType _type = FrameRateType.FPS;

		[SerializeField] private int _rate = 30;

		public FrameRateType Type {
			get { return this._type; }
			set {
				this._type = value;
				if (Application.isPlaying) this.Request.Type = value;
			}
		}

		public int Rate {
			get { return this._rate; }
			set {
				this._rate = value;
				if (Application.isPlaying) this.Request.Value = value;
			}
		}

		private FrameRateRequest _request;
		protected FrameRateRequest Request {
			get {
				if (this._request == null) {
					this._request = new FrameRateRequest(this._type, this._rate);
				}
				return this._request;
			}
		}

		private bool _isApplicationQuitting = false;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void OnEnable() {
			this.Request.Start();
		}

		protected virtual void OnDisable() {
			if (this._isApplicationQuitting) return;
			this.Request.Stop();
		}

		protected virtual void OnApplicationQuit() {
			this._isApplicationQuitting = true;
		}

		#if UNITY_EDITOR
		protected virtual void OnValidate() {
			if (!Application.isPlaying) return;
			this.Request.Type = this._type;
			this.Request.Value = this._rate;
		}
		protected virtual void OnReset() {
			if (!Application.isPlaying) return;
			this.Request.Type = this._type;
			this.Request.Value = this._rate;
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Custom Inspector ---------->>
		#if UNITY_EDITOR
		[CustomEditor(typeof(FrameRateRequester), true)]
		[CanEditMultipleObjects]
		protected class CustomInspectorBase : Editor {
			
			private SerializedProperty propType;
			private SerializedProperty propRate;

			protected virtual void OnEnable() {

				this.propType = this.serializedObject.FindProperty("_type");
				this.propRate = this.serializedObject.FindProperty("_rate");
			}

			public override void OnInspectorGUI() {
				this.serializedObject.Update();
				this.DrawDefaultInspector();

				if (propType.hasMultipleDifferentValues || this.propRate.hasMultipleDifferentValues) return;

				var rateType = (FrameRateType)this.propType.enumValueIndex;
				int minRate = FrameRateRequest.MinValueForType(rateType);
				if (this.propRate.intValue >= minRate) return;

				//EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Minimum value for " + rateType.ToString() + " is " + minRate, MessageType.Warning);
				EditorGUILayout.Space();
			}
		}
		#endif
		#endregion <<---------- Custom Inspector ---------->>	
	}
}