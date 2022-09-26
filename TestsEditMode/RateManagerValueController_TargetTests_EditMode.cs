using NUnit.Framework;
using UniRate.Internals;

namespace UniRate.TestsEditMode {

    public class RateManagerValueController_TargetTests_EditMode {

        private class RequestMock : RateRequest {

            public RequestMock(RateManagerValueController controller, int value) : base(RateRequestType.UpdateRate, controller) {
                this._value = value;
            }

            protected override int Value => this._value;
            private readonly int _value;
        }

        private class ControllerMock : RateManagerValueController {

            public ControllerMock(int limitValue, bool limitByMax, string valueName = "mock rate") : base(null, limitValue, limitByMax, valueName) {

            }

            protected override void ApplyTargetValueToUnitySettings() {
                // do nothing
            }

            public RequestMock RequestMock(int mockValue) {
                return this.BaseRequest(new RequestMock(this, mockValue), 1);
            }
        }

        [Test]
        public void LimitByMax_NoRequests_TargetIsLimitValue() {
            // given
            bool limitByMax = true;
            int limitValue = 10;

            // when
            var controller = new ControllerMock(limitValue, limitByMax);

            // then
            Assert.AreEqual(limitValue, controller.Target);
        }

        [Test]
        public void LimitByMin_NoRequests_TargetIsLimitValue() {
            // given
            bool limitByMax = false;
            int limitValue = 10;

            // when
            var controller = new ControllerMock(limitValue, limitByMax);

            // then
            Assert.AreEqual(limitValue, controller.Target);
        }


        [Test]
        public void LimitByMax_RequestAboveLimit_RequestBelowLimit_DisposeRequests_TargetIsLimitValue() {
            // given
            bool limitByMax = true;
            int limitValue = 10;
            int requestValueAbove = limitValue + 2;
            int requestValueBelow = limitValue - 2;

            // when
            var controller = new ControllerMock(limitValue, limitByMax);
            using (var requestAbove = controller.RequestMock(requestValueAbove)) {
                using (var requestBelow = controller.RequestMock(requestValueBelow)) {

                }
            }

            // then
            Assert.AreEqual(limitValue, controller.Target);
        }

        [Test]
        public void LimitByMin_RequestAboveLimit_RequestBelowLimit_DisposeRequests_TargetIsLimitValue() {
            // given
            bool limitByMax = false;
            int limitValue = 10;
            int requestValueAbove = limitValue + 2;
            int requestValueBelow = limitValue - 2;

            // when
            var controller = new ControllerMock(limitValue, limitByMax);
            using (var requestAbove = controller.RequestMock(requestValueAbove)) {
                using (var requestBelow = controller.RequestMock(requestValueBelow)) {

                }
            }

            // then
            Assert.AreEqual(limitValue, controller.Target);
        }


        [Test]
        public void LimitByMax_RequestAboveLimit_TargetIsLimitValue() {
            // given
            bool limitByMax = true;
            int limitValue = 10;
            int requestValue = limitValue + 2;

            // when
            var controller = new ControllerMock(limitValue, limitByMax);
            using (var request = controller.RequestMock(requestValue)) {

                // then
                Assert.AreEqual(limitValue, controller.Target);
            }
        }

        [Test]
        public void LimitByMin_RequestAboveLimit_TargetIsRequestValue() {
            // given
            bool limitByMax = false;
            int limitValue = 10;
            int requestValue = limitValue + 2;

            // when
            var controller = new ControllerMock(limitValue, limitByMax);
            using (var request = controller.RequestMock(requestValue)) {

                // then
                Assert.AreEqual(requestValue, controller.Target);
            }
        }


        [Test]
        public void LimitByMax_RequestBelowLimit_TargetIsRequestValue() {
            // given
            bool limitByMax = true;
            int limitValue = 10;
            int requestValue = limitValue - 2;

            // when
            var controller = new ControllerMock(limitValue, limitByMax);
            using (var requestAbove = controller.RequestMock(requestValue)) {

                // then
                Assert.AreEqual(requestValue, controller.Target);
            }
        }

        [Test]
        public void LimitByMin_RequestBelowLimit_TargetIsLimitValue() {
            // given
            bool limitByMax = false;
            int limitValue = 10;
            int requestValue = limitValue - 2;

            // when
            var controller = new ControllerMock(limitValue, limitByMax);
            using (var requestAbove = controller.RequestMock(requestValue)) {

                // then
                Assert.AreEqual(limitValue, controller.Target);
            }
        }


        [Test]
        public void LimitByMax_RequestLow_RequestHigh_TargetIsRequestLow() {
            // given
            bool limitByMax = true;
            int limitValue = 10;
            int requestLowValue = limitValue - 2;
            int requestHighValue = requestLowValue + 1;

            // when
            var controller = new ControllerMock(limitValue, limitByMax);
            using (var requestLow = controller.RequestMock(requestLowValue)) {
                using (var requestHigh = controller.RequestMock(requestHighValue)) {

                    // then
                    Assert.AreEqual(requestLowValue, controller.Target);
                }
            }
        }

        [Test]
        public void LimitByMin_RequestLow_RequestHigh_TargetIsRequestHigh() {
            // given
            bool limitByMax = false;
            int limitValue = 10;
            int requestLowValue = limitValue + 2;
            int requestHighValue = requestLowValue + 1;

            // when
            var controller = new ControllerMock(limitValue, limitByMax);
            using (var requestLow = controller.RequestMock(requestLowValue)) {
                using (var requestHigh = controller.RequestMock(requestHighValue)) {

                    // then
                    Assert.AreEqual(requestHighValue, controller.Target);
                }
            }
        }
    }
}