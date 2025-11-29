# Integrity SDL2/OpenGL 2D Engine 

A work-in-progress, modern 2D graphics engine built entirely in C# to explore and implement the **OpenGL 3.3 Core Profile** pipeline from the ground up.

Integrity uses **SDL2** solely for cross-platform window management and input handling, separating the rendering concerns completely into a custom OpenGL rendering pipeline.

---

## Core Features

* **SDL2/C# Foundation:** Utilizes the Silk.NET bindings for stable, cross-platform window and context creation.
* **OpenGL 3.3 Core:** Enforces the use of modern VAOs, VBOs, and programmable shaders (GLSL 330).
* **2D Coordinate System:** Implements a top-left origin (Y-down) orthographic projection, ideal for 2D game development.
* **Asset Pipeline:** Dedicated managers (services) that handle loading image files (via **StbImageSharp**) and converting them directly into GPU-ready **GLTexture** resources.

---

## Getting Started

This project is structured as a series of pipelines and managers (`RenderPipeline`, `AssetManager`, etc.) to isolate engine dependencies.

To run the engine, you will need:
1.  **.NET 8+ SDK**
2.  **SDL2 runtime libraries** (managed via Silk.NET/your environment setup)

### Debug & Testing

The current setup includes a simple test in `Engine.cs` that draws a sample asset using the basic **Translate * Scale** Model matrix to confirm the projection and rendering pipeline are functional.
