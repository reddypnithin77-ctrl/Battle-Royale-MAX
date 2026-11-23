# Battle Royale MAX — Original prototype

An original battle-royale project. It is **not Free Fire MAX** and does not use Garena assets, characters, maps, branding, or proprietary code.

## Browser prototype
A small **single-player browser prototype** lives in `docs/index.html`. It demonstrates movement, mouse aiming, firing at training bots, health, and a shrinking safe zone. It is not a full 3D game and does not connect real players.

After enabling GitHub Pages under **Settings → Pages → Build and deployment → GitHub Actions**, the workflow deploys it at:
`https://reddypnithin77-ctrl.github.io/Battle-Royale-MAX/`

## Unity Android starter
`Assets/Scripts` contains the initial Unity C# controller, camera, health and safe-zone scripts. See [Unity setup](Assets/Scripts/README.md) and [roadmap](Docs/ROADMAP.md). This is not an Android APK.

## Not implemented yet
- Production 3D assets and full Android touch controls
- Real online matchmaking, networking, accounts, or 50-player dedicated servers
- Release signing, anti-cheat, moderation, or production backend

## Run locally
Open `docs/index.html` in a modern browser. Desktop: WASD/arrow keys to move, mouse to aim, click or Space to fire. Mobile: drag to move and tap to fire.
