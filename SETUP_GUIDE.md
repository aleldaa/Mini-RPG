# Mini RPG Unity Project Setup Guide

This guide will walk you through setting up the mini RPG project in Unity step by step.

## Prerequisites
- Unity Hub installed
- Unity 2022.3 LTS or newer
- Basic knowledge of Unity interface

## Step 1: Create New Unity Project
1. Open Unity Hub
2. Click "New Project"
3. Select "2D" template
4. Name your project "MiniRPG" (or any name you prefer)
5. Choose location and click "Create project"

## Step 2: Import Scripts
1. In your Unity project, create the following folder structure in Assets:
   ```
   Assets/
   ├── Scripts/
   │   ├── Player/
   │   ├── Enemy/
   │   ├── Combat/
   │   ├── Inventory/
   │   ├── Quest/
   │   ├── Camera/
   │   └── Items/
   ├── Prefabs/
   ├── Scenes/
   └── Materials/
   ```

2. Copy all the C# scripts from this repository to their respective folders

## Step 3: Set Up the Main Scene
1. In the Hierarchy, create an empty GameObject named "GameManager"
2. Add the `GameManager` script to it
3. Create another empty GameObject named "Player"
4. Add the `PlayerController` script to the Player object

## Step 4: Set Up Player Character
1. Select the Player GameObject
2. Add a Sprite Renderer component
3. Add a Rigidbody2D component:
   - Set Body Type to "Dynamic"
   - Check "Freeze Rotation Z"
4. Add a Collider2D component (Circle Collider 2D or Box Collider 2D)
5. Set the Player tag to "Player"
6. **Add the `Combatant` script** (this enables turn-based combat)
7. **Add the `GridMovement` script** (this enables grid-based movement)
8. Configure the PlayerController script values:
   - Move Speed: 5
   - Sprint Speed: 8
   - Max Health: 100
   - Attack Damage: 20
   - Attack Range: 2
   - Enemy Layer: Set to "Enemy" layer

## Step 5: Set Up Turn-Based Combat System
1. Create an empty GameObject named "TurnManager"
2. Add the `TurnManager` script to it
3. Create an empty GameObject named "CombatUI"
4. Add the `CombatUI` script to it
5. **Configure TurnManager references:**
   - Drag CombatUI to the Combat UI field
   - Drag Player to the Player Movement field
6. **Configure CombatUI references:**
   - Drag TurnManager to the Turn Manager field

## Step 6: Set Up Enemy System
1. Create an empty GameObject named "EnemySpawner"
2. Add the `EnemySpawner` script to it
3. Create an empty GameObject named "Enemy"
4. Add the `Enemy` script to it
5. **Add the `Combatant` script** to the Enemy
6. Set up Enemy components similar to Player:
   - Sprite Renderer
   - Rigidbody2D
   - Collider2D
   - Set tag to "Enemy"
7. Configure Enemy script values:
   - Max Health: 50
   - Damage: 15
   - Move Speed: 2
   - Detection Range: 5
   - Attack Range: 1.5
   - Player Layer: Set to "Player" layer
8. **Configure Combatant script values:**
   - Max Health: 50
   - Attack Damage: 15
   - Defense: 5
   - Speed: 8
   - Move Range: 2
   - Attack Range: 1
9. Drag the Enemy GameObject to the EnemySpawner's Enemy Prefab field

## Step 7: Set Up Camera
1. Select the Main Camera
2. Add the `CameraFollow` script
3. Drag the Player GameObject to the Target field
4. Configure camera settings:
   - Smooth Speed: 5
   - Offset: (0, 0, -10)
   - Look Ahead Distance: 2

## Step 8: Set Up UI System
1. Create a Canvas (Right-click in Hierarchy → UI → Canvas)
2. Set Canvas Scaler to "Scale With Screen Size"
3. Create UI elements:

### Health Bar
- Create UI → Slider
- Position at top-left
- Set Fill Area color to red
- Drag to PlayerController's Health Bar field

### Level and Experience Text
- Create UI → Text
- Position below health bar
- Drag to PlayerController's respective fields

### **Combat UI Panel**
- Create UI → Panel
- Set to inactive by default
- Add the following UI elements:
  - **Turn Display:**
    - Text for current turn
    - Text for current combatant
  - **Action Buttons:**
    - Move Button
    - Attack Button
    - End Turn Button
    - Item Button
  - **Combat Status:**
    - Text for combat status
    - Player health bar
    - Player health text
- Drag to CombatUI's respective fields

### Inventory Panel
- Create UI → Panel
- Set to inactive by default
- Add Grid Layout Group for item slots
- Drag to InventorySystem's Inventory Panel field

### Quest Panel
- Create UI → Panel
- Set to inactive by default
- Add Vertical Layout Group for quest entries
- Drag to QuestSystem's Quest Panel field

## Step 9: Set Up Inventory System
1. Create an empty GameObject named "InventorySystem"
2. Add the `InventorySystem` script
3. Configure the script:
   - Max Inventory Slots: 20
   - Drag UI references to their respective fields

## Step 10: Set Up Quest System
1. Create an empty GameObject named "QuestSystem"
2. Add the `QuestSystem` script
3. Configure the script with UI references

## Step 11: Set Up Layers
1. Go to Edit → Project Settings → Tags and Layers
2. Create new layers:
   - Player (Layer 8)
   - Enemy (Layer 9)
   - Obstacle (Layer 10)
3. Assign objects to appropriate layers

## Step 12: Create Basic Level
1. Create a few platforms/obstacles using Sprite Renderers
2. Add Collider2D components to them
3. Set their layer to "Obstacle"
4. Position them around the scene

## Step 13: Create Move Tile Prefab
1. Create an empty GameObject named "MoveTile"
2. Add a Sprite Renderer component
3. Add a Collider2D component (set as trigger)
4. Add the `MoveTile` script
5. Drag this to the Player's GridMovement Move Tile Prefab field

## Step 14: Test the Game
1. Press Play in Unity
2. **Exploration Mode:**
   - Use WASD to move
   - Approach enemies to start combat
3. **Combat Mode:**
   - Use Move button or WASD to move on grid
   - Use Attack button or left-click to attack
   - Use End Turn button to end your turn
4. Press I to open inventory
5. Press Q to open quests
6. Press ESC to pause

## **Turn-Based Combat Controls**
- **Move Button**: Click to enable grid movement, then use WASD/Arrow keys
- **Attack Button**: Click to enable attack mode, then left-click on enemies
- **End Turn Button**: End your turn early
- **Item Button**: Open inventory to use items
- **WASD/Arrow Keys**: Move on grid when movement is enabled
- **Left Click**: Attack when attack mode is enabled
- **Right Click**: Cancel current action

## Troubleshooting Common Issues

### Player not moving
- Check if Rigidbody2D is set to Dynamic
- Verify PlayerController script is attached
- Check if Input Manager has WASD keys configured

### Turn-based combat not working
- Ensure TurnManager is set up with proper references
- Check that both Player and Enemy have Combatant components
- Verify CombatUI is properly configured

### Enemies not spawning
- Verify EnemySpawner has Enemy Prefab assigned
- Check if Player tag is set correctly
- Ensure EnemySpawner script is enabled

### UI not working
- Check if Canvas is set to Screen Space - Overlay
- Verify UI references are properly assigned in scripts
- Ensure UI elements are active/inactive as expected

### Scripts not found
- Make sure all scripts are in the correct folders
- Check for compilation errors in Console
- Verify script names match exactly

## Next Steps for Enhancement
1. Add sprites and animations
2. Implement sound effects
3. Add more enemy types
4. Create multiple levels
5. Add save/load system
6. Implement more complex quests
7. Add particle effects
8. Create a main menu scene
9. **Add special abilities and spells**
10. **Implement status effects**
11. **Add terrain effects on movement**

## Controls Reference
- **WASD**: Move player (exploration) / Move on grid (combat)
- **Left Shift**: Sprint (exploration only)
- **Left Click**: Attack
- **E**: Interact/Collect items
- **I**: Toggle inventory
- **Q**: Toggle quests
- **ESC**: Pause/Resume game

## Performance Tips
- Use object pooling for enemies
- Limit the number of active enemies
- Use sprite atlases for better performance
- Optimize UI updates to only happen when needed
- **Limit grid calculations to visible area only**

Happy Turn-Based RPG development! 🎮⚔️♟️
