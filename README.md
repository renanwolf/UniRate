# UniRate

by Renan Wolf Pace

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

#### via Assets Import Package

Import the [.unitypackage](https://github.com/renanwolf/UniRate/releases/latest) from the latest release to your Unity project.

## Rate Manager

The main plugin singleton that allows you to control all the rates and intervals. It is recommended that for anything else than throwaway code, you keep the instance referenced with you while using it.

It manages multiples rate/interval requests and apply the best one.

Just access the `RateManager.Instance` by code and it will be automatically created, or create an empty `GameObject` and attach the `RateManager` component to it.

#### Setting Up

- `UpdateRateMode`: set to `VSyncCount` or `ApplicationTargetFrameRate` to choose how the update rate will be managed.

- `MinimumUpdateRate`: is the minimum allowed update rate that can be applied. Any request bellow this value will be ignored.

- `MinimumFixedUpdateRate`: is the minimum allowed fixed update rate that can be applied. Any request bellow this value will be ignored.

- `MaximumRenderInterval`: is the maximum allowed render interval that can be applied. Any request above this value will be ignored.

## Rate and Interval Requests

To start a request you need to access the `RateManager.Instance` and use one of the following methods:

- `RequestUpdateRate(int)` to start a new update rate request, it returns the `UpdateRateRequest`.

- `RequestFixedUpdateRate(int)` to start a new fixed update rate request, it returns the `FixedUpdateRateRequest`.

- `RequestRenderInterval(int)` to start a new render interval request, it returns the `RenderIntervalRequest`.

All the requests returned from these methods inherits from `RateRequest` and implements `IDisposable`. Keep the requests with you to be able to cancel them later.

To cancel a request just dispose it!

```csharp
using UniRate;
...

private IEnumerator ExampleCoroutineThatPerformsAnimation() {

  // get the RateManager instance
  var rateManager = RateManager.Instance;
  
  // starts the requests you need
  var updateRateRequest = rateManager.RequestUpdateRate(60);
  var fixedUpdateRateRequest = rateManager.RequestFixedUpdateRate(50);
  var renderIntervalRequest = rateManager.RequestRenderInterval(1); // only works on Unity 2019.3 or newer

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

Is the number of `Update` that will take before the game executes a render. A value of 1 means the game will render on every update, a value of 2 on every other update, and so on.

It **only works on Unity 2019.3 or newer**, since its use the new Unity `OnDemandRendering` API. For any previous version the render interval will always be 1, ignoring the requests.

To verify if the current frame will render just access the `WillRender` property inside the `RateManager` instance.

## Ready to use Components

There are a few components already created to manage requests in some circumstances, if they aren't enough you can create yours.

#### RateRequestWhileEnabledComponent

This component will keep the requests active while it is active and enabled.

#### RateRequestTouchComponent

This component will keep the requests active while `Input.touchCount` is greater then zero or `Input.GetMouseButton(0, 1, 2)` is true.

#### RateRequestScrollRectComponent

This component will keep the requests active while the `ScrollRect.velocity` is not zero or when it changes the normalized position.

#### RateRequestInputFieldComponent _and_ RateRequestTMPInputFieldComponent

These components will keep the requests active while the input field is focused or when the text changes.

To enable the `RateRequestTMPInputFieldComponent` you need to add the `TMPRO` define symbol in your player settings.