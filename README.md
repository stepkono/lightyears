# Lightyears

## Project Overview

Lightyears is an app designed to help users invest time in their hobbies over the long term. By visualizing hobbies as planets and letting each user create a unique solar system over time, the app motivates users to keep engaging with their hobbies and skills in a playful, engaging way.

## Important Setup Instructions (Unity Editor)

To interact with the app via touch input during Play Mode in the Unity Editor:

1. Open Window > Analysis > Input Debugger.
2. In the Input Debugger window, go to Options (top-left corner).
3. Enable “Simulate Touchscreen” — this allows mouse input to be interpreted as touch.

Additionally:
The app is designed for long-term use. Normally, visible changes (e.g. planet evolution) occur over extended real-world time periods.
To make the app testable for demo and evaluation purposes, the internal time is accelerated by a factor of 5000.
You can adjust this scalar inside the Utils class under _Scripts/Miscellaneous/Utils.cs.

## _Scripts Folder Structure

```
_Scripts/
├── ArrowPulse.cs
├── Controllers/
│   ├── Controller.cs
│   └── InputFieldController.cs
├── Input/
│   ├── ButtonReminderInput.cs
│   ├── ButtonSelectFriend.cs
│   ├── ButtonsManager.cs
│   ├── FiendsListButton.cs
│   ├── FriendListWindowInput.cs
│   ├── InputSystem.cs
│   ├── LaunchSlider.cs
│   ├── PinchToZoom.cs
│   ├── ScrollSelector.cs
│   ├── ShareButtonManager.cs
│   ├── SwipeDetection.cs
│   ├── TimerSlider.cs
│   └── TouchManager.cs
├── Managers/
│   ├── CamerasManager.cs
│   ├── FriendListManager.cs
│   ├── HobbyCreator.cs
│   ├── HobbyManager.cs
│   ├── HobbyNameFollower.cs
│   ├── PlanetManager.cs
│   ├── SystemManager.cs
│   ├── ViewsManager.cs
│   └── VolumeManager.cs
├── Miscellaneous/
│   ├── AppEvents.cs
│   ├── Buffer.cs
│   ├── Singleton.cs
│   └── Utils.cs
└── Types/
    ├── HobbyData.cs
    └── Stage.cs
```

## Core Concepts

- **Hobbies as Planets**: Each hobby is represented visually as a planet with its own orbit and stages. The more you commit, the richer your solar system becomes.
- **Solar System Visualization**: Every new hobby adds a planet, orbiting within your own evolving solar system.
- **Progress & Motivation**: This playful visualization rewards sustained engagement with your hobbies or skills.

## Code Organization

### Controllers
- **Controller.cs**: The master controller, initializes and wires up subsystems, handles central events, and global logic.
- **InputFieldController.cs**: Handles data capture for hobby names, hours, intervals, etc.

### Managers
- **SystemManager.cs**: Maintains the solar system (the planet list), handles orbital calculations, and hobby insertion order.
- **HobbyManager.cs**: Lifecycle, progression, and state transitions for a single hobby/planet.
- **HobbyCreator.cs**: Creation logic (instantiation, saving, initial setup) for new hobby planets.
- **FriendListManager.cs**: Integrates friends/collaborators into hobbies.
- **PlanetManager.cs, ViewsManager.cs, VolumeManager.cs, CamerasManager.cs, HobbyNameFollower.cs**: Support managers (UI, scene, camera, and visual utility logic).

### Input
- **TimerSlider.cs, LaunchSlider.cs**: Advanced drag and time interaction.
- **TouchManager.cs, PinchToZoom.cs, SwipeDetection.cs**: Handles touch, gesture, and swipe input.
- **FriendListWindowInput.cs, ShareButtonManager.cs, ButtonSelectFriend.cs, FiendsListButton.cs, ScrollSelector.cs, InputSystem.cs, ButtonReminderInput.cs, ButtonsManager.cs**: All button and UI-specific logic, including friend selection.

### Miscellaneous
- **AppEvents.cs**: Global event definitions for loosely coupled inter-system communication.
- **Singleton.cs**: Singleton base class for project-wide single-instance logic.
- **Buffer.cs, Utils.cs**: General helpers and utility methods.

### Types
- **HobbyData.cs**: Data structure capturing name, creation date, friends list, interval, etc.
- **Stage.cs**: Models a planet's development or evolution stage.

## Main Architectural Flow

1. **User creates a new hobby (planet) with HobbyCreator**
2. **HobbyManager controls its lifecycle, progression, and state**
3. **SystemManager maintains the planet's place among all hobbies in the "solar system"**
4. **User interactions are processed via Input modules**
5. **Views and screens change via ViewsManager**

## UI & View System

- **ViewsManager** centralizes all UI view/screen changes. It is responsible for switching, animating, and showing/hiding screens via API and events.
- It works closely with the Cinemachine system (which is tied to the CinemachineBrain object in the Unity project setup) to power camera and visual transitions between screens.
- UI responses and all transitions are routed through the ViewsManager and Cinemachine, ensuring smooth, context-aware navigation throughout the app.

## Communication

- The project relies on both direct method calls (Controller/Manager patterns) and an event system for decoupled, broadcasted messages.
- AppEvents and custom events allow for robust communication without tight coupling between modules.

---
