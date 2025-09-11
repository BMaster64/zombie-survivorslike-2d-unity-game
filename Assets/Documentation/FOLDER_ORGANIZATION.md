# Assets Folder Organization Guide

## Structure Overview

```
Assets/
├── Art/                    # All visual assets
│   ├── Animations/        # Animation clips and controllers
│   ├── Sprites/           # All sprite assets (enemies, players, environment)
│   ├── UI/                # User interface graphics
│   └── Fonts/             # Font assets
├── Audio/                 # Sound effects and music (empty for now)
├── Core/                  # Essential game components
│   ├── Scripts/           # All C# scripts
│   ├── Prefabs/           # Game object prefabs organized by category
│   └── Scenes/            # Unity scenes
├── Data/                  # Configuration and settings
│   └── Settings/          # Unity settings and configurations
├── Documentation/         # Project documentation and guides
└── ThirdParty/           # External assets and plugins
    ├── NavMeshPlus-master/
    ├── SlimUI/
    └── TextMesh Pro/
```

## Folder Purposes

### Art/
- **Animations/**: Animation clips (.anim) and Animator Controllers (.controller)
- **Sprites/**: All image assets organized by category (Enemy/, Player/, Fx/, etc.)
- **UI/**: User interface related graphics
- **Fonts/**: Font files and font assets

### Core/
- **Scripts/**: All C# game scripts
- **Prefabs/**: Game object prefabs organized by type (Characters/, Enemies/, Weapons/, etc.)
- **Scenes/**: Unity scene files

### Data/
- **Settings/**: Unity project settings, input actions, render pipeline settings
- Configuration files and data assets

### ThirdParty/
- External packages and assets from Unity Asset Store or other sources
- Keep third-party assets separate to easily identify and update them

## Best Practices

1. **Keep similar assets together**: Group related files in the same subfolder
2. **Use descriptive names**: Make folder and file names clear and consistent
3. **Separate third-party assets**: Keep external assets in ThirdParty folder
4. **Organize by function, not file type**: Group by what the assets do, not their file extension
5. **Maintain this structure**: When adding new assets, place them in the appropriate existing folders

## Adding New Assets

- **New sprites**: Add to `Art/Sprites/` in appropriate subfolder
- **New scripts**: Add to `Core/Scripts/`
- **New prefabs**: Add to `Core/Prefabs/` in appropriate category folder
- **New scenes**: Add to `Core/Scenes/`
- **Third-party packages**: Add to `ThirdParty/`
