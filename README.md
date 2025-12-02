# Integrity 2D - SDL2/Opengl Engine

A work-in-progress, modern 2D graphics engine built entirely in **C#** to explore and implement the **OpenGL 3.3 Core Profile** pipeline from the ground up.

Integrity utilizes **SDL2** (via Silk.NET) solely for cross-platform window management and input handling, separating the rendering concerns completely into a custom OpenGL rendering pipeline.

---

## Core Architecture & Design

The engine is built on a strong **Service-Oriented Architecture (SOA)**, relying on the **Service Locator** pattern to manage engine subsystems (`AssetManager`, `RenderPipeline`, `SceneManager`, etc.). This promotes loose coupling and testability.

### Key Components:

* **Service Locator:** The central `Service.Get<T>()` mechanism is used to resolve all engine dependencies, including:
    * `IAssetManager`: Handles asset caching and loading.
    * `IRenderPipeline`: Manages all OpenGL interaction.
    * `ICameraManager`: Controls the current world view.
* **Main Loop (`Engine.Run()`):** Explicitly handles timing (`deltaTime`), profiling (`IProfiler`), and delegating work to the separate `HandleInput()`, `Update()`, and `Render()` methods.

---

## Modern Rendering Pipeline

Integrity's greatest focus is on high-performance 2D rendering using modern graphics techniques.

### Core Features:

* **OpenGL 3.3 Core:** Enforces the use of modern VAOs, VBOs, and programmable shaders (GLSL 330).
* **Instanced Rendering (Batching):** All sprite rendering is performed using hardware **instancing**. During the `Render()` loop, objects are grouped by their **`GLTexture`** into the `m_RenderingBatchMap`, and then drawn in a single, highly efficient `DrawSpritesInstanced` call.
* **2D Coordinate System:** Implements a top-left origin (Y-down) orthographic projection, ideal for 2D development.
* **Asset Pipeline:** Dedicated managers handle loading image files (via **StbImageSharp**) and converting them directly into GPU-ready **`GLTexture`** resources.
* **Debugging/GUI:** Integrated **ImGui** (via `IImGuiPipeline`) for real-time debugging tools and engine statistics visualization (FPS, Profiler data).

---

## Getting Started

This project is structured as a series of pipelines and managers.

### Requirements

1.  **.NET 8+ SDK**
2.  **SDL2 runtime libraries** (managed via Silk.NET/your environment setup)

### Example: Scene Setup

New game objects are instantiated via the factory and registered with the active scene to be rendered automatically.


```csharp
// Inside Engine.Initialize() (Will become IGame.Initialize() later)
// Create a new scene
Scene defaultScene = new Scene("DefaultScene");

// Get the factory service from the engine's Service Locator
var factory = Service.Get<IGameObjectFactory>();
var sceneManager = Service.Get<ISceneManager>();

// Load the asset and create the SpriteObject
// The factory handles loading the texture via IAssetManager internally.
string assetPath = "/path/to/your/assets/my_sprite.png";
var playerSprite = factory.CreateSpriteObject("PlayerCharacter", assetPath);

// Adjust the object's transform (position, scale, rotation)
playerSprite.Transform.X = 250.0f;
playerSprite.Transform.Y = 150.0f;
playerSprite.Transform.ScaleX = 0.5f;

// Register the object with the current scene
// Once registered, the Engine's main Render() loop will automatically draw it batched and instanced.
sceneManager.CurrentScene.RegisterGameObject(playerSprite);

// Load and set the scene
m_SceneManager.AddScene(defaultScene);
m_SceneManager.LoadScene(defaultScene);
```