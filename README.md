# TextMotion Pro
[![Unity](https://img.shields.io/badge/unity-6%2B-green.svg?style=flat-square)](https://unity.com/)
[![Last Commit](https://img.shields.io/github/last-commit/bluepixeldev/textmotion-pro?style=flat-square)](https://github.com/bluepixeldev/textmotion-pro/commits/main)
[![Stars](https://img.shields.io/github/stars/bluepixeldev/textmotion-pro?style=flat-square)](https://github.com/bluepixeldev/textmotion-pro/stargazers)
[![Issues](https://img.shields.io/github/issues/bluepixeldev/textmotion-pro?style=flat-square)](https://github.com/bluepixeldev/textmotion-pro/issues)

**TextMotion Pro** is a modular and performant animation system for [TextMeshPro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@latest) components in Unity 6 and above. It allows you to author complex, character-based text animations using reusable `ScriptableObject` assets in a profile-driven setup inspired by Unity's Volume system.

## Features

- Modular text effects defined as `ScriptableObject`s
- Lightweight and performant — ideal for UI and in-game text
- Works with Unity 6.0 and above
- Tag-based animation control similar to HTML or BBCode
- Compatible with TextMeshPro components
- Runtime and editor-time updates supported

## Installation

1. Clone or download this repository into your Unity project under `Assets/textmotion-pro`.
2. Ensure **TextMeshPro** is installed in your project via Unity Package Manager.
3. Use Unity 6.0 or later for full compatibility.

## Usage

### 1. Create a Motion Profile

- Right-click in the Project window → **Create → textmotion-pro → MotionProfile**
- Add or configure `TextEffect` assets to this profile.

### 2. Attach `TextMotionPro`

- Add the `TextMotionPro` component to a GameObject with a `TMP_Text` component.
- Assign a `MotionProfile` to the renderer.

### 3. Use Tags in Text

- Use tags like `<Wave>Text</Wave>` where `"Wave"` matches the `EffectTag` of a `TextEffect`.


## Extending

To implement a new animation:

1. Create a class inheriting from `TextEffect`.
2. Implement the `ApplyEffect()` method.
3. Define an `EffectTag` and optionally override validation or reset behavior.
4. Add the effect to a MotionProfile.

## Example

```csharp
[TextEffect("Wave", "Wavy motion effect for characters")]
public class WaveEffect : TextEffect
{
    public override string EffectTag => "Wave";

    public override void ApplyEffect(MotionRenderContext context)
    {
        // Animation logic here
    }
}
````
