# Unity LowPower and Memory Consumption

LowPowerMemoryConsumption is a Unity plugin to reduce device's battery usage, main focused on GUI oriented apps and turn-based games. To optimize energy drain this package provides a framerate management to reduce the CPU usage and a renderrate management to reduce the GPU usage.

## Quick Start

#### Installation

Import the .unitypackage to your project or download the repository and import the files to your project's Assets folder.

#### Namespace

All the scripts are inside the `PWR.LowPowerMemoryConsumption` namespace.

## Frame Rate

#### Manager

FrameRateManager is a singleton that allows to control the game framerate (FPS) and the fixed framerate (PhysicsFPS). Also provides the current FPS and PhysicsFPS values.

It manage multiples framerate requests and apply the highest one, or the fallback framerate if there are no requests.

To create the FrameRateManager instace while editing your scene, just create an empty GameObject and add the FrameRateManager component, after that you can settup it in the Inspector. Otherwise just access `FrameRateManager.Instance` by code and it will be automatically created.

#### Requests

WORK IN PROGRESS

## Camera Render

WORK IN PROGRESS

## Memory

#### Manager

MemoryManager is a sigleton that will handle application low memory callbacks with options to unload unused assets and collect garbage after the callback.

To create the MemoryManager instace while editing your scene, just create an empty GameObject and add the MemoryManager component, after that you can settup it in the Inspector. Otherwise just access `MemoryManager.Instance` by code and it will be automatically created.

#### Low Memory Trigger

In order to get low memory callbacks, you can start listening to MemoryManager by code or use the LowMemoryTrigger component.

##### Start/Stop listening by code

```
using UnityEngine;
using PWR.LowPowerMemoryConsumption;

public class MyHeavyAssets : MonoBehaviour {

  private void OnEnable() {
    MemoryManager.Instance.onLowMemory += MyLowMemoryCallback;
  }
  
  private void OnDisable() {
    MemoryManager.Instance.onLowMemory -= MyLowMemoryCallback;
  }
  
  private void MyLowMemoryCallback() {
    // change your heavy assets to small ones
  }
}
```

##### LowMemoryTrigger Component

This component will be listening to the MemoryManager while active and enable. It has a UnityEvent `OnLowMemoryEvent` that you can register functions to receive the low memory callback.
