# Match 3D Puzzle

> Unity 3D matching puzzle game with ScriptableObject levels, booster mechanics, DOTween animations, and a custom level editor.

This project implements a mobile-style Match 3D puzzle loop: players tap collectable 3D objects, send them to a limited stack, and clear objects by forming triples of the same type. Levels are data-driven through ScriptableObjects and can be edited from a custom Unity editor window.

## Highlights

- 3D collectable-object matching gameplay with a 7-slot stack
- Triple-match resolution with animated object movement
- Timer-based fail condition and level completion flow
- Hint, undo, and freeze boosters
- ScriptableObject level data for object type, count, and duration
- Custom editor window for level setup and PlayerPrefs tools
- DOTween-powered movement, matching, and booster feedback
- 23 collectable object types with prefab/model/editor thumbnail assets
- 10 bundled levels with repeat/random fallback after the authored range

## Gameplay

Players select objects in the scene by tapping/clicking them. Selected objects move into the stack, are grouped by object type, and are removed when three matching objects arrive. The level ends when all objects are cleared, the timer reaches zero, or the stack runs out of space.

| System | Behavior |
| --- | --- |
| Stack | Holds up to seven collected objects |
| Matching | Removes the first available triple by object type |
| Timer | Counts down per level and can be frozen temporarily |
| Hint | Finds a useful object or completes a likely match |
| Undo | Returns the latest stack object back to the game area |
| Level flow | Advances through authored levels and restarts with fresh object positions |

## Editor Tools

The project includes a custom editor under `Level/Editor`:

- `Level Editor` edits `LevelInfo` assets, object types, object counts, and level duration.
- `Settings` allows quick PlayerPrefs level changes and progress reset.

Level data lives in `Assets/Resources/Levels`, and collectable prefabs live in `Assets/Resources/CollectableObjects`.

## Project Structure

```text
Assets/
  Models/                     3D source models for collectable objects
  Resources/
    CollectableObjects/       Runtime collectable prefabs
    Levels/                   ScriptableObject level definitions
    Editor/                   Editor thumbnails used by the level editor
  Scenes/
    GameScene.unity
  Scripts/
    Editor/                   Custom level editor and settings tools
    Game/                     Gameplay, input, boosters, UI, camera, timer
    Scriptable/               LevelInfo and CollectableObject data
    Utility/                  Singleton, loading helpers, app settings
  Sprites/                    UI sprites and atlas
```

## Tech Stack

- Unity `2021.3.1f1`
- C#
- DOTween
- TextMesh Pro
- Unity UI
- ScriptableObject-based content data

## Running

1. Open the repository root with Unity `2021.3.1f1`.
2. Open `Assets/Scenes/GameScene.unity`.
3. Press Play.
4. Use `Level/Editor` to inspect or edit level data.

## Notes

The repository is source-focused. Local Android builds should be generated outside git or kept ignored by the included `.gitignore`.
