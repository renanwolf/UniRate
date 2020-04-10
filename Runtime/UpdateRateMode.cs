using UnityEngine;

namespace UniRate {

    public enum UpdateRateMode {

        /// <summary>
        /// Uses the <see cref="Application.targetFrameRate"/> to manage the update rate.
        /// </summary>
        ApplicationTargetFrameRate,

        /// <summary>
        /// Uses the <see cref="QualitySettings.vSyncCount"/> to manage the update rate.
        /// </summary>
        VSyncCount,

        /// <summary>
        /// Uses an end of frame coroutine with throttling to manage the update rate.
        /// </summary>
        ThrottleEndOfFrame
    }
}