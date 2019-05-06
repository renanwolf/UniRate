# Unity LowPower and Memory Consumption

LowPowerMemoryConsumption is a Unity plugin to reduce device's battery usage, main focused on GUI oriented apps and turn-based games. To optimize energy drain this package provides a framerate management to reduce the CPU usage and a render interval management to reduce the GPU usage.

## Quick Start

#### Installation

Import the .unitypackage to your project or download the repository and import the files to your project's Assets folder.

#### Namespace

All the scripts are inside the `PWR.LowPowerMemoryConsumption` namespace.

## Frame Rate

#### Manager

FrameRateManager is a singleton that allows to control the game framerate (FPS) and the fixed framerate (PhysicsFPS). Also provides the current FPS and PhysicsFPS values.

It manages multiples framerate requests and apply the highest one, or the fallback framerate if there are no requests.

To create the FrameRateManager instace while editing your scene, just create an empty GameObject and add the FrameRateManager component, after that you can settup it in the Inspector. Otherwise just access `FrameRateManager.Instance` by code and it will be automatically created.

#### Requests

To request a framerate to the manager you can use a `FrameRateRequest` by code or the `FrameRateRequestComponent`.

##### FrameRateRequestComponent

This component will start the framerate request at `OnEnable` and stop at `OnDisable`.

In the inspetor you can choose the framerate value and the type between FPS and FixedFPS.

##### Request by code

```csharp
using UnityEngine;
using PWR.LowPowerMemoryConsumption;

public class MyCodeAnimation : MonoBehaviour {

  public bool animateOnFixedUpdate;

  private FrameRateRequest requestFPS;
  private FrameRateRequest requestFixedFPS;

  void OnEnable() {
    this.requestFPS = FrameRateManager.Instance.StartRequest(FrameRateType.FPS, 60);
    this.requestFixedFPS = FrameRateManager.Instance.StartRequest(FrameRateType.FixedFPS, 50);
  }

  void OnDisable() {
    FrameRateManager.Instance.StopRequest(this.requestFPS);
    FrameRateManager.Instance.StopRequest(this.requestFixedFPS);
  }

  void Update() {
    if (animateOnFixedUpdate) return;
    DoAnimationStep();
  }

  void FixedUpdate() {
    if (!animateOnFixedUpdate) return;
    DoAnimationStep();
  }
  
  void DoAnimationStep() { ... }
}
```

#### Callbacks and Triggers

In order to get callbacks from the FrameRateManager, you can start listening to its events by code or use the `FrameRateTrigger` component.

#### FrameRateLooper

It's a class to help you to execute heavy loops while keeping the desired frame rate, running the loop inside a coroutine and when necessary waiting a frame.

```csharp
using UnityEngine;
using System.Collections.Generic;
using PWR.LowPowerMemoryConsumption;

public class ListItemsUI : MonoBehaviour {

  public GameObject itemPrefab;

  private FrameRateRequest requestFPS;
  private FrameRateLooper looperFPS;

  public List<MyItemData> datasource;

  void Awake() {
    this.datasource = this.FetchDataSource();

    this.looperFPS = new FrameRateLooper() {
      FrameRateToKeep = 30,
      MinCyclesPerFrame = 2,
      MaxCyclesPerFrame = 10,
      TimeoutWaitForFrameRate = 1f
    };
    this.looperFPS.Count = this.datasource.Count;

    this.looperFPS.LoopStarted += (looper) => {
      looper.Index = 0;
      this.requestFPS = FrameRateManager.Instance.StartRequest(FrameRateType.FPS, 60);
    };

    this.looperFPS.LoopFinished += (looper) => {
      FrameRateManager.Instance.StopRequest(this.requestFPS);
    };

    this.looperFPS.LoopCycle(looper => {

      var item = Instantiate(this.itemPrefab);
      item.transform.SetParent(this.transform);
      item.GetComponent<MyItemComponent>().LoadData(this.datasource[looper.Index]);

      looper.Index += 1;
    });

    this.looperFPS.LoopWhile(looper => looper.Index < looper.Count);
  }

  void OnEnable() {
    this.looperFPS.Start(this);
  }

  void OnDisable() {
    this.looperFPS.Stop();
  }
  
  List<MyItemData> FetchDataSource() { ... }
}
```

## Render Interval

#### Manager

RenderIntervalManager is a component that allows to control the render interval of a camera. Interval of 1 means the camera will render on every `Update`, interval of 2 means the camera will render on every 2 `Update`s and so on.

It manages multiples render interval requests and apply the lowest one, or the fallback interval if there are no requests.

Add this component to your cameras to start managing the render interval of them.

#### Requests

To request a render interval to a camera you can use a `RenderIntervalRequest` by code or the `RenderIntervalRequestComponent`.

##### RenderIntervalRequestComponent

This component will start the render interval request at `OnEnable` and stop at `OnDisable`.

In the inspetor you can choose the interval value and the target `RenderIntervalManager`.

##### Request by code

```csharp
using UnityEngine;
using PWR.LowPowerMemoryConsumption;

public class MyOtherCodeAnimation : MonoBehaviour {

  public RenderIntervalManager manager;

  void OnEnable() {
    StartCorouitne(AnimCoroutine());
  }

  IEnumarator AnimCoroutine() {
     var request = this.manager.StartRequest(1);
     var finished = false;
     while (!finished) {
       //anim logic
       yield return null;
     }
     this.manager.StopRequest(request);
  }
}
```

#### Callbacks and Triggers

In order to get callbacks from a RenderIntervalManager, you can start listening to its events by code or use the `RenderIntervalTrigger` component.

## Memory

#### Manager

MemoryManager is a sigleton that will handle application low memory callbacks with options to unload unused assets and collect garbage after the callback.

To create the MemoryManager instace while editing your scene, just create an empty GameObject and add the MemoryManager component, after that you can settup it in the Inspector. Otherwise just access `MemoryManager.Instance` by code and it will be automatically created.

#### Callbacks and Triggers

In order to get low memory callbacks, you can start listening to the MemoryManager by code or use the `LowMemoryTrigger` component.

##### LowMemoryTrigger Component

This component will be listening to the MemoryManager while active and enable. It has a UnityEvent `LowMemory` that you can register functions to receive the low memory callback.

##### Start/Stop listening by code

```csharp
using UnityEngine;
using PWR.LowPowerMemoryConsumption;

public class MyHeavyAssets : MonoBehaviour {

  void OnEnable() {
    MemoryManager.Instance.LowMemory += OnLowMemory;
  }
  
  void OnDisable() {
    MemoryManager.Instance.LowMemory -= OnLowMemory;
  }
  
  void OnLowMemory() {
    // change your heavy assets to small ones
  }
}
```