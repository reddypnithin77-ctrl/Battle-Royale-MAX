# INDIAN FIRE world map

The procedural scene builder creates an original, stylized India-shaped mainland layout. This is a fictional game world, includes the northern region, western and eastern mainland, Northeast extension, and full southern peninsula; it is not a precise geographic or navigational representation.

## Districts

- Snowcrest Gate — highland district
- Victory Arch — capital-inspired district
- Moonstone Palace — desert heritage-inspired district
- Liongate Port — western coastal district
- Four Arch Bazaar — old-city market district
- River Crown — eastern river-region district
- Brahmaputra Frontier — Northeast river-region district
- Eastern Highlands — Northeast highland district
- Golden Sanctuary — northwestern heritage-inspired district
- Sunspire Coast — eastern coastal district
- Tech Valley — southern technology district
- Lotus Reach — central-southern district
- Backwater Haven — southwestern waterway district

All place names and structures are original. The layout does not reproduce a real-world map or any proprietary game map.

## What the greybox generator adds

- A full stylized mainland India outline with terrain tiles and surrounding ocean, including the Northeast extension and southern peninsula
- Simple connecting roads between mainland districts
- Three basic enterable greybox buildings around each named district: a market house, storehouse, and residence
- A landmark plaza and district label
- Health and ammo pickup examples in each district

The buildings are built from primitive geometry so they are easy to edit. Their appearance is intentionally simple and should later be replaced with optimized, original models, textures, props, and lighting.

## Landscape mobile layout

The generated scene adds `BRLandscapeMode`, which disables portrait orientation and allows both landscape directions. The mobile HUD and touch controls use a 1920×1080 reference canvas so their anchored positions are designed for horizontal screens.

## Generate the scene

1. Open the Unity project.
2. Wait for script compilation to finish.
3. Select **Tools → INDIAN FIRE → Build World Scene**.
4. Unity saves the generated scene to `Assets/Scenes/TrainingArena.unity`.
5. Open that scene and press Play to test the practice prototype.

If the menu does not appear, check the Unity Console for compile errors. A package compilation error can prevent Unity from compiling Editor scripts and registering the menu item.

## Prototype limitations

This is a greybox practice environment, not a finished production map. Loot is local prototype pickup behavior; it is not server-authoritative multiplayer loot. The map and landscape layout have not yet been visually or performance-tested on Android. Replace primitive geometry with original or properly licensed assets and profile the scene on target devices before release.
