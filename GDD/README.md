# Game Design Document - Pixel Metroidvania

This folder contains the Game Design Document (GDD) for a minimal-scope Metroidvania game based on the PixelIdleGame codebase.

## 📄 Contents

- **index.html** - The main GDD website (open in any web browser)

## 🎯 Purpose

This GDD defines the **minimum viable product (MVP)** scope for transforming the existing PixelIdleGame into a Metroidvania-style game. The focus is on:

- **Minimal scope** - Only essential features
- **Clear ability-gating** - Simple progression system
- **Tight controls** - Leverage existing movement system
- **Quick to implement** - Uses existing codebase architecture

## 🚀 How to View

Simply open `index.html` in any modern web browser. The document is self-contained with embedded CSS and requires no server or build process.

## 📋 Key Features Defined

### Core Metroidvania Requirements
- **4 Interconnected Areas** - Seamless world design with multiple paths
- **3 Core Abilities** - Dash, Double Jump, Down Smash (gate progression)
- **3 Enemy Types** - Basic Grunt, Flying Drone, Boss
- **Ability-Gated Progression** - Clear, visible unlock system
- **Backtracking Rewards** - Meaningful exploration incentives

### Essential Systems
- **Map System** - Fog of war, room tracking, player location
- **Save/Checkpoint System** - Save stations, respawn points, progress tracking
- **Power-Up Progression** - Major abilities + minor upgrades (health, damage)
- **Interconnected World** - Loop-back design, shortcuts, hub-and-spoke
- **Secret Areas** - Hidden rooms, optional bosses, lore fragments
- **Environmental Storytelling** - Visual clues, area themes, background details
- **Quality of Life** - Fast travel (late game), ability UI, damage numbers

## 🎮 Based On

This GDD leverages the existing PixelIdleGame systems:
- Component-based architecture
- State machine system
- Power-up system
- Enemy system
- ScriptableObject data management

## 🎯 Metroidvania Design Pillars

1. **Interconnected World** - No isolated levels, everything connects
2. **Ability-Based Progression** - New abilities unlock new areas
3. **Rewarding Exploration** - Hidden rooms, upgrades, secrets
4. **Non-Linear Path** - Player choice in progression order
5. **Backtracking Incentive** - Always a reason to return to old areas
6. **Clear Visual Language** - Gates and abilities are obvious
7. **Satisfying Loop-Backs** - Shortcuts reward thorough exploration

## 📝 Notes

This is a **living document** - update as the project evolves. The scope is intentionally minimal to ensure a complete, playable game can be delivered quickly while maintaining all essential Metroidvania characteristics.

## 📐 Development Timeline

**Estimated Time:** 9-10 weeks
- Week 1-2: Core systems (abilities, gates, saves)
- Week 2-4: World foundation + Map system
- Week 4-6: Ability implementation (Dash, Double Jump)
- Week 6-7: Progression & upgrades
- Week 7-8: Final area & boss
- Week 8-9: Polish & juice
- Week 9-10: Testing & balance

