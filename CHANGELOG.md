# Changelog

## [Unreleased]
### Added
- `RateRequestAnimationComponent` to activate requests while an `Animation` component is playing.
- `RateRequestAnimatorComponent` to activate requests while an `Animator` component is playing.
### Changed
### Deprecated
### Removed
### Fixed
- `DelaySecondsToStopRequests` on requests components was not calculating delay since 'should stop requests' time.
### Security

## [2.1.2] - 2018-10-01
### Fixed
- Changes to `RateDebug.ScreenDataBackgroundColor` sometimes had no effect when application was running on device.
- Legacy code not being imported correctly by Unity because assembly definition file was missing.

## [2.1.1] - 2018-08-28
### Fixed
- `package.json` wrong version.

## [2.1.0] - 2018-08-28
### Added
- Customize background color, text color and font size on `RateDebug` screen data.
- Enable/disable verbose screen data on `RateDebug`.
- `RateManager.HasInstance` to check if the instance exists without creating it.
### Changed
- Renamed `Debugger` to `RateDebug`.
- Renamed `LogLevel` to `RateLogLevel`.
- Increased `RateDebug.LogLevel` default value.