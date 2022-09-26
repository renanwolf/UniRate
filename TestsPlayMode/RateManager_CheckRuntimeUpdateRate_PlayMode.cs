using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UniRate.TestsPlayMode {

    public class RateManager_CheckRuntimeUpdateRate_PlayMode {

        private IEnumerator AssertAreEqualInMaxFrames(int expected, Func<int> actualProvider, int maxFrames) {
            int frames = 0;
            int actual;
            do {
                yield return null;
                frames += 1;
                actual = actualProvider();
            } while (frames < maxFrames && actual != expected);
            Assert.AreEqual(expected, actual, $"assert occured in {frames.ToString()} frames");
        }


        [UnityTest]
        public IEnumerator VSyncCount_Min15_NoRequests_CurrentIsMin_In5FramesMax() {
            // given
            var rateManager = RateManager.Instance;
            rateManager.UpdateRate.Mode = UpdateRateMode.VSyncCount;
            rateManager.UpdateRate.Minimum = 15;

            // when
            rateManager.ApplyTargetsIfDirty();

            // then
            yield return this.AssertAreEqualInMaxFrames(rateManager.UpdateRate.Minimum, () => rateManager.UpdateRate.Current, 5);
        }

        [UnityTest]
        public IEnumerator ApplicationTargetFrameRate_Min15_NoRequests_CurrentIsMin_In5FramesMax() {
            // given
            var rateManager = RateManager.Instance;
            rateManager.UpdateRate.Mode = UpdateRateMode.ApplicationTargetFrameRate;
            rateManager.UpdateRate.Minimum = 15;

            // when
            rateManager.ApplyTargetsIfDirty();

            // then
            yield return this.AssertAreEqualInMaxFrames(rateManager.UpdateRate.Minimum, () => rateManager.UpdateRate.Current, 5);
        }


        [UnityTest]
        public IEnumerator VSyncCount_Min15_Request2_CurrentIsMin_In5FramesMax() {
            // given
            var rateManager = RateManager.Instance;
            rateManager.UpdateRate.Mode = UpdateRateMode.VSyncCount;
            rateManager.UpdateRate.Minimum = 15;
            int requestValue = 2;

            // when
            using (var request = rateManager.UpdateRate.Request(requestValue)) {
                rateManager.ApplyTargetsIfDirty();

                // then
                yield return this.AssertAreEqualInMaxFrames(rateManager.UpdateRate.Minimum, () => rateManager.UpdateRate.Current, 5);
            }
        }

        [UnityTest]
        public IEnumerator ApplicationTargetFrameRate_Min15_Request2_CurrentIsMin_In5FramesMax() {
            // given
            var rateManager = RateManager.Instance;
            rateManager.UpdateRate.Mode = UpdateRateMode.ApplicationTargetFrameRate;
            rateManager.UpdateRate.Minimum = 15;
            int requestValue = 2;

            // when
            using (var request = rateManager.UpdateRate.Request(requestValue)) {
                rateManager.ApplyTargetsIfDirty();

                // then
                yield return this.AssertAreEqualInMaxFrames(rateManager.UpdateRate.Minimum, () => rateManager.UpdateRate.Current, 5);
            }
        }


        [UnityTest]
        public IEnumerator VSyncCount_Min5_Request15_CurrentIsRequest_In5FramesMax() {
            // given
            var rateManager = RateManager.Instance;
            rateManager.UpdateRate.Mode = UpdateRateMode.VSyncCount;
            rateManager.UpdateRate.Minimum = 5;
            int requestValue = 15;

            // when
            using (var request = rateManager.UpdateRate.Request(requestValue)) {
                rateManager.ApplyTargetsIfDirty();

                // then
                yield return this.AssertAreEqualInMaxFrames(requestValue, () => rateManager.UpdateRate.Current, 5);
            }
        }

        [UnityTest]
        public IEnumerator ApplicationTargetFrameRate_Min5_Request15_CurrentIsRequest_In5FramesMax() {
            // given
            var rateManager = RateManager.Instance;
            rateManager.UpdateRate.Mode = UpdateRateMode.ApplicationTargetFrameRate;
            rateManager.UpdateRate.Minimum = 5;
            int requestValue = 15;

            // when
            using (var request = rateManager.UpdateRate.Request(requestValue)) {
                rateManager.ApplyTargetsIfDirty();

                // then
                yield return this.AssertAreEqualInMaxFrames(requestValue, () => rateManager.UpdateRate.Current, 5);
            }
        }


        [UnityTest]
        public IEnumerator VSyncCount_Min5_Request15_Wait3Frames_DisposeRequest_CurrentIsMin_In5FramesMax() {
            // given
            var rateManager = RateManager.Instance;
            rateManager.UpdateRate.Mode = UpdateRateMode.VSyncCount;
            rateManager.UpdateRate.Minimum = 5;
            int requestValue = 15;
            int waitFrames = 3;

            // when
            using (var request = rateManager.UpdateRate.Request(requestValue)) {
                rateManager.ApplyTargetsIfDirty();
                for (int i = 0; i < waitFrames; i++) {
                    yield return null;
                }
            }
            rateManager.ApplyTargetsIfDirty();

            // then
            yield return this.AssertAreEqualInMaxFrames(rateManager.UpdateRate.Minimum, () => rateManager.UpdateRate.Current, 5);
        }

        [UnityTest]
        public IEnumerator ApplicationTargetFrameRate_Min5_Request15_Wait3Frames_DisposeRequest_CurrentIsMin_In5FramesMax() {
            // given
            var rateManager = RateManager.Instance;
            rateManager.UpdateRate.Mode = UpdateRateMode.ApplicationTargetFrameRate;
            rateManager.UpdateRate.Minimum = 5;
            int requestValue = 15;
            int waitFrames = 3;

            // when
            using (var request = rateManager.UpdateRate.Request(requestValue)) {
                rateManager.ApplyTargetsIfDirty();
                for (int i = 0; i < waitFrames; i++) {
                    yield return null;
                }
            }
            rateManager.ApplyTargetsIfDirty();

            // then
            yield return this.AssertAreEqualInMaxFrames(rateManager.UpdateRate.Minimum, () => rateManager.UpdateRate.Current, 5);
        }
    }
}