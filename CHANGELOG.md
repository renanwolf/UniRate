# Changelog

## [3.0.0] - 2022-09-26
### Added
- New editor tracker window to make it easy to debug requests lifecycle. It's accessible through `Window > UniRate Tracker`.
### Changed
- Refactored rates/interval management to allow easy testability.
- **Breaking:** `RateManager.Instance.UpdateRate` now returns the update rate controller instance instead of the current update rate per seconds value, which is now accessible through `RateManager.Instance.UpdateRate.Current`.
- **Breaking:** `RateManager.Instance.FixedUpdateRate` now returns the fixed update rate controller instance instead of the current fixed update rate per seconds value, which is now accessible through `RateManager.Instance.FixedUpdateRate.Current`.
- **Breaking:** `RateManager.Instance.RenderInterval` now returns the render interval controller instance instead of the current render interval value, which is now accessible through `RateManager.Instance.RenderInterval.Current`.
### Deprecated
- `RateManager.Instance.UpdateRateMode` is now deprecated, use `RateManager.Instance.UpdateRate.Mode` instead.
- `RateManager.Instance.MinimumUpdateRate` is now deprecated, use `RateManager.Instance.UpdateRate.Minimum` instead.
- `RateManager.Instance.TargetUpdateRate` is now deprecated, use `RateManager.Instance.UpdateRate.Target` instead.
- `RateManager.Instance.UpdateRateModeChanged` is now deprecated, use `RateManager.Instance.UpdateRate.ModeChanged` instead.
- `RateManager.Instance.UpdateRateChanged` is now deprecated, use `RateManager.Instance.UpdateRate.CurrentChanged` instead.
- `RateManager.Instance.MinimumFixedUpdateRate` is now deprecated, use `RateManager.Instance.FixedUpdateRate.Minimum` instead.
- `RateManager.Instance.TargetFixedUpdateRate` is now deprecated, use `RateManager.Instance.FixedUpdateRate.Target` instead.
- `RateManager.Instance.FixedUpdateRateChanged` is now deprecated, use `RateManager.Instance.FixedUpdateRate.CurrentChanged` instead.
- `RateManager.Instance.TargetFixedUpdateRateChanged` is now deprecated, use `RateManager.Instance.FixedUpdateRate.TargetChanged` instead.
- `RateManager.Instance.MaximumRenderInterval` is now deprecated, use `RateManager.Instance.RenderInterval.Maximum` instead.
- `RateManager.Instance.TargetRenderInterval` is now deprecated, use `RateManager.Instance.RenderInterval.Target` instead.
- `RateManager.Instance.RenderIntervalChanged` is now deprecated, use `RateManager.Instance.RenderInterval.CurrentChanged` instead.
- `RateManager.Instance.TargetRenderIntervalChanged` is now deprecated, use `RateManager.Instance.RenderInterval.TargetChanged` instead.
- `RateManager.Instance.RenderRate` is now deprecated, use `RateManager.Instance.RenderInterval.CurrentRenderRate` instead.
- `RateManager.Instance.WillRender` is now deprecated, use `RateManager.Instance.RenderInterval.WillRender` instead.
- `RateManager.Instance.IsRenderIntervalSupported` is now deprecated, use `RateManager.Instance.RenderInterval.IsSupported` instead.
- `RateManager.Instance.RequestUpdateRate(int)` is now deprecated, use `RateManager.Instance.UpdateRate.Request(int)` instead.
- `RateManager.Instance.RequestFixedUpdateRate(int)` is now deprecated, use `RateManager.Instance.FixedUpdateRate.Request(int)` instead.
- `RateManager.Instance.RequestRenderInterval(int)` is now deprecated, use `RateManager.Instance.RenderInterval.Request(int)` instead.
### Fixed
- Fixed `ArgumentNullException` when application is quitting ([#2](https://github.com/renanwolf/UniRate/issues/2)).

## [2.2.2] - 2021-07-28
### Fixed
- Fixed compilation error on `RateManager` for Unity versions below 2019.3 ([#1](https://github.com/renanwolf/UniRate/pull/1)), by [@Chrisdbhr](https://github.com/Chrisdbhr).

## [2.2.1] - 2021-05-18
### Fixed
- `RateRequestAnimatorComponent` was not evaluating correctly if `Animator` states are playing.

## [2.2.0] - 2021-01-20
### Added
- `RateRequestAnimationComponent` to activate requests while an `Animation` component is playing.
- `RateRequestAnimatorComponent` to activate requests while an `Animator` component is playing.
### Fixed
- `DelaySecondsToStopRequests` on requests components was not calculating delay since 'should stop requests' time.
- Wrong release years on changelog.md file.

## [2.1.2] - 2020-10-01
### Fixed
- Changes to `RateDebug.ScreenDataBackgroundColor` sometimes had no effect when application was running on device.
- Legacy code not being imported correctly by Unity because assembly definition file was missing.

## [2.1.1] - 2020-08-28
### Fixed
- `package.json` wrong version.

## [2.1.0] - 2020-08-28
### Added
- Customize background color, text color and font size on `RateDebug` screen data.
- Enable/disable verbose screen data on `RateDebug`.
- `RateManager.HasInstance` to check if the instance exists without creating it.
### Changed
- Renamed `Debugger` to `RateDebug`.
- Renamed `LogLevel` to `RateLogLevel`.
- Increased `RateDebug.LogLevel` default value.

[Unreleased]: https://github.com/renanwolf/UniRate/compare/v3.0.0...HEAD
[3.0.0]: https://github.com/renanwolf/UniRate/releases/tag/v3.0.0
[2.2.2]: https://github.com/renanwolf/UniRate/releases/tag/v2.2.2
[2.2.1]: https://github.com/renanwolf/UniRate/releases/tag/v2.2.1
[2.2.0]: https://github.com/renanwolf/UniRate/releases/tag/v2.2.0
[2.1.2]: https://github.com/renanwolf/UniRate/releases/tag/v2.1.2
[2.1.1]: https://github.com/renanwolf/UniRate/releases/tag/v2.1.1
[2.1.0]: https://github.com/renanwolf/UniRate/releases/tag/v2.1.0