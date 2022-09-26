# UniRate

Created by Renan Wolf Pace

[![Release](https://img.shields.io/github/v/release/renanwolf/UniRate.svg)](https://github.com/renanwolf/UniRate/releases)
[![OpenUPM](https://img.shields.io/npm/v/com.pflowr.unirate?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.pflowr.unirate/)
[![Changelog](https://img.shields.io/github/release-date/renanwolf/UniRate?color=green&label=changelog)](CHANGELOG.md)
![UnityVersion](https://img.shields.io/badge/dynamic/json?color=green&label=unity&query=%24.unity&suffix=%20or%20later&url=https%3A%2F%2Fraw.githubusercontent.com%2Frenanwolf%2FUniRate%2Frelease%2Fpackage.json)
[![License](https://img.shields.io/github/license/renanwolf/UniRate)](LICENSE.md)

## Overview

A Unity plugin to easily manage the application frame rate and rendering interval.

It's not desirable to keep your game always running at the highest frame rate, especially on mobile platforms where you can quickly consume a lot of battery power and increase device heat.

To help you solve these problems, UniRate provides you a simple solution to control the update rate, fixed update rate and render interval from everywhere in your code without worrying about multiple requests.

## Installation

#### via Package Manager

Add the following dependency to the `Packages/manifest.json` file of your Unity project:
```json
"dependencies": {
    "com.pflowr.unirate": "https://github.com/renanwolf/UniRate.git",
}
```

#### via OpenUPM

This package is available on [OpenUPM](https://openupm.com/packages/com.pflowr.unirate/) registry, you can install it via [openupm-cli](https://github.com/openupm/openupm-cli):
```
openupm add com.pflowr.unirate
```

#### via Assets Import Package

Import the [.unitypackage](https://github.com/renanwolf/UniRate/releases/latest) from the latest release to your Unity project.

## Rate Manager

The main plugin singleton that allows you to control all the rates and intervals. It is recommended that for anything else than throwaway code, you keep the instance referenced with you while using it.

It manages multiples rate/interval requests and apply the best one.

Just access the `RateManager.Instance` by code and it will be automatically created, or create an empty `GameObject` and attach the `RateManager` component to it.

#### Setting Up

- `UpdateRate.Mode`: set to `VSyncCount` or `ApplicationTargetFrameRate` to choose how the update rate should be managed.

- `UpdateRate.Minimum`: is the minimum allowed update rate that can be applied. Any request bellow this value will be ignored.

- `FixedUpdateRate.Minimum`: is the minimum allowed fixed update rate that can be applied. Any request bellow this value will be ignored.

- `RenderInterval.Maximum`: is the maximum allowed render interval that can be applied. Any request above this value will be ignored.

## Rate and Interval Requests

To start a request you need to access the `RateManager.Instance` and use one of the following methods:

- `UpdateRate.Request(int)` to start a new update rate request, it returns an `UpdateRateRequest`.

- `FixedUpdateRate.Request(int)` to start a new fixed update rate request, it returns a `FixedUpdateRateRequest`.

- `RenderInterval.Request(int)` to start a new render interval request, it returns a `RenderIntervalRequest`.

All the requests returned from these methods inherits from `RateRequest` and implements `IDisposable`. Keep the requests with you to be able to cancel them later.

To cancel a request just dispose it!

```csharp
using UniRate;
...

private IEnumerator ExampleCoroutineThatPerformsAnimation() {

  // get the RateManager instance
  var rateManager = RateManager.Instance;
  
  // starts the requests you need
  var updateRateRequest = rateManager.UpdateRate.Request(60);
  var fixedUpdateRateRequest = rateManager.FixedUpdateRate.Request(50);
  var renderIntervalRequest = rateManager.RenderInterval.Request(1); // only works on Unity 2019.3 or newer

  while (isAnimating) {
    ...
    yield return null;
  }

  // cancel them when you are done
  updateRateRequest.Dispose();
  fixedUpdateRateRequest.Dispose();
  renderIntervalRequest.Dispose();
}
```

## Update Rate

Is the number of `Update` per second that the game executes.

Unity uses the update rate to manage the input system, so if it's too low the engine will not detect user's inputs correctly. It should be fine if it stays at least around 18.

## Fixed Update Rate

Is the number of `FixedUpdate` per second that the game executes.

## Render Interval

Is the number of `Update` that takes before the game executes a render. A value of 1 means the game will render on every update, a value of 2 on every other update, and so on.

It **only works on Unity 2019.3 or newer**, since its use the new Unity `OnDemandRendering` API. For any previous version the render interval will always be 1, ignoring the requests.

To verify if the current frame will render just access the `RenderInterval.WillRender` property inside the `RateManager` instance.

## Ready to use Components

There are a few components already created to manage requests in some circumstances, if they aren't enough you can create yours.

#### RateRequestWhileEnabledComponent

This component keeps the requests active while it is active and enabled.

#### RateRequestTouchComponent

This component keeps the requests active while `Input.touchCount` is greater then zero or `Input.GetMouseButton(0, 1, 2)` is true.

#### RateRequestScrollRectComponent

This component keeps the requests active while the `ScrollRect.velocity` is not zero or when it changes the normalized position.

#### RateRequestInputFieldComponent _and_ RateRequestTMPInputFieldComponent

These components keep the requests active while the input field is focused or when the text changes.

To enable the `RateRequestTMPInputFieldComponent` you need to add the `TMPRO` define symbol in your player settings.

#### RateRequestAnimationComponent

This component keeps the requests active while an `Animation` component is playing.

#### RateRequestAnimatorComponent

This component keeps the requests active while an `Animator` component is playing.

## UniRate Tracker

The tracker is useful to debug requests lifecycle, you can open the tracker window through `Window > UniRate Tracker`.

## Debugging

All the debug options can be modified accessing the `RateDebug` static class.

```csharp
using UniRate.Debug;
...

private void SetUniRateDebugSettingsForProduction() {
  RateDebug.LogLevel = RateLogLevel.Warning;
  RateDebug.DisplayOnScreenData = false;
}

private void SetUniRateDebugSettingsForTests() {
  RateDebug.LogLevel = RateLogLevel.Debug;
  RateDebug.ScreenDataBackgroundColor = new Color(0, 0, 0, 0.5f);
  RateDebug.ScreenDataFontSize = 10;
  RateDebug.ScreenDataFontColor = Color.white;
  RateDebug.ScreenDataVerbose = false;
  RateDebug.DisplayOnScreenData = true;
}
```

#### DisplayOnScreenData

If enabled, display on the top-left corner of the screen informations about current rates and intervals.

To modify how the on screen data is displayed, change the following properties `ScreenDataVerbose`, `ScreenDataBackgroundColor`, `ScreenDataFontSize` and `ScreenDataFontColor`.

#### IsDebugBuild

On editor returns `EditorUserBuildSettings.development`, otherwise returns `Debug.isDebugBuild`.

#### LogLevel

Set to one of the following values to filter which logs should be enabled:

- `Trace`: changes to `QualitySettings.vSyncCount`, `Application.targetFrameRate`, `Time.fixedDeltaTime`, `OnDemandRendering.renderFrameInterval` and `RateRequest` creation/cancellation are logged with this level.

- `Debug`: changes to `UpdateRate.Target`, `FixedUpdateRate.Target` and `RenderInterval.Target` are logged with this level.

- `Info`: changes to `UpdateRate.Mode`, `UpdateRate.Minimum`, `FixedUpdateRate.Minimum` and `RenderInterval.Maximum` are logged with this level.

- `Warning`

- `Error`

- `Off`

The default value on editor is `Debug` if `IsDebugBuild` is true, otherwise `Info`. In runtime is `Info` if `IsDebugBuild` is true, otherwise `Warning`.