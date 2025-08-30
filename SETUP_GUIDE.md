# Mini RPG Unity Project Setup Guide (Unity 6)

This guide will walk you through setting up the mini RPG project in Unity 6 with the new interface and features.

## Prerequisites
- Unity Hub installed
- Unity 6.0 or newer
- Basic knowledge of Unity interface

## Step 1: Create New Unity Project
1. **Open Unity Hub**
2. **Click "New Project"**
3. **Select "2D" template** (Unity 6 has improved 2D tools)
4. **Name your project** "MiniRPG" (or any name you prefer)
5. **Choose location** and click "Create project"
6. **Wait for Unity 6 to open** - you'll see the new interface

## Step 2: Import Scripts
1. **In Unity 6, use the new Asset Browser** (usually on the left side)
2. **Navigate to Assets** in the Asset Browser
3. **Right-click in Assets → Create → Folder** and name it "Scripts"
4. **Inside Scripts folder, create subfolders**:
   - Right-click → Create → Folder for each:
     - Player
     - Enemy
     - Combat
     - Inventory
     - Quest
     - Camera
     - Items
5. **Copy your C# scripts** into the appropriate folders using one of these methods:
   - **Method A**: Drag .cs files from File Explorer directly into Unity's Asset Browser
   - **Method B**: Copy .cs files to your Unity project folder in File Explorer
   - **Method C**: In Unity, right-click in folder → Create → C# Script, then copy-paste the code

## Step 3: Set Up the Main Scene
1. **In the Hierarchy window** (usually on the left side)
2. **Right-click in empty space → Create Empty**
3. **Name it "GameManager"**
4. **Select the GameManager GameObject**
5. **In the Inspector** (right side), click "Add Component"
6. **Search for "GameManager"** and add the script

## Step 4: Set Up Player Character
1. **Create another empty GameObject** in the Hierarchy
2. **Name it "Player"**
3. **Add the `PlayerController` script** to it (same method as above)
4. **Add the `Combatant` script** (this enables turn-based combat)
5. **Add the `GridMovement` script** (this enables grid-based movement)

### Configure Player Components:
1. **Select the Player GameObject**
2. **In the Inspector, add these components**:
   - **Sprite Renderer**: Add Component → Rendering → Sprite Renderer
   - **Rigidbody 2D**: Add Component → Physics 2D → Rigidbody 2D
   - **Collider 2D**: Add Component → Physics 2D → Circle Collider 2D or Box Collider 2D
3. **Set the Player tag**: In Inspector, click the Tag dropdown → Add Tag → Create "Player"
4. **Configure the PlayerController script values**:
   - Move Speed: 5
   - Sprint Speed: 8
   - Max Health: 100
   - Attack Damage: 20
   - Attack Range: 2
   - Enemy Layer: Set to "Enemy" layer

## Step 5: Set Up Turn-Based Combat System
1. **Create an empty GameObject** named "TurnManager"
2. **Add the `TurnManager` script** to it
3. **Create an empty GameObject** named "CombatUI"
4. **Add the `CombatUI` script** to it
5. **Configure TurnManager references**:
   - Select TurnManager in Hierarchy
   - In Inspector, drag CombatUI to the "Combat UI" field
   - Drag Player to the "Player Movement" field
6. **Configure CombatUI references**:
   - Select CombatUI in Hierarchy
   - In Inspector, drag TurnManager to the "Turn Manager" field

## Step 6: Set Up Enemy System
1. **Create an empty GameObject** named "EnemySpawner"
2. **Add the `EnemySpawner` script** to it
3. **Create an empty GameObject** named "Enemy"
4. **Add the `Enemy` script** to it
5. **Add the `Combatant` script** to the Enemy
6. **Set up Enemy components** similar to Player:
   - Sprite Renderer
   - Rigidbody 2D
   - Collider 2D
   - Set tag to "Enemy"
7. **Configure Enemy script values**:
   - Max Health: 50
   - Damage: 15
   - Move Speed: 2
   - Detection Range: 5
   - Attack Range: 1.5
   - Player Layer: Set to "Player" layer
8. **Configure Combatant script values**:
   - Max Health: 50
   - Attack Damage: 15
   - Defense: 5
   - Speed: 8
   - Move Range: 2
   - Attack Range: 1
9. **Drag the Enemy GameObject** to the EnemySpawner's "Enemy Prefab" field

## Step 7: Set Up Camera
1. **Select the Main Camera** in the Hierarchy
2. **Add the `CameraFollow` script** to it
3. **Drag the Player GameObject** to the "Target" field
4. **Configure camera settings**:
   - Smooth Speed: 5
   - Offset: (0, 0, -10)
   - Look Ahead Distance: 2

## Step 8: Set Up UI System
1. **Right-click in Hierarchy → UI → Canvas** (Unity 6 has improved UI tools)
2. **Set Canvas Scaler** to "Scale With Screen Size"
3. **Create UI elements**:

### Health Bar
- **Right-click Canvas → UI → Slider**
- **Position at top-left** using the Rect Transform
- **Set Fill Area color to red**
- **Drag to PlayerController's "Health Bar" field**

### Level and Experience Text
- **Right-click Canvas → UI → Text**
- **Position below health bar**
- **Drag to PlayerController's respective fields**

### **Combat UI Panel**
- **Right-click Canvas → UI → Panel**
- **Set to inactive by default** (uncheck the checkbox in Inspector)
- **Add the following UI elements**:
  - **Turn Display**:
    - Right-click Panel → UI → Text for current turn
    - Right-click Panel → UI → Text for current combatant
  - **Action Buttons**:
    - Right-click Panel → UI → Button for Move
    - Right-click Panel → UI → Button for Attack
    - Right-click Panel → UI → Button for End Turn
    - Right-click Panel → UI → Button for Items
  - **Combat Status**:
    - Right-click Panel → UI → Text for combat status
    - Right-click Panel → UI → Slider for player health bar
    - Right-click Panel → UI → Text for player health text
- **Drag to CombatUI's respective fields**

### Inventory Panel
- **Right-click Canvas → UI → Panel**
- **Set to inactive by default**
- **Add Grid Layout Group** for item slots
- **Drag to InventorySystem's "Inventory Panel" field**

### Quest Panel
- **Right-click Canvas → UI → Panel**
- **Set to inactive by default**
- **Add Vertical Layout Group** for quest entries
- **Drag to QuestSystem's "Quest Panel" field**

## Step 9: Set Up Inventory System
1. **Create an empty GameObject** named "InventorySystem"
2. **Add the `InventorySystem` script**
3. **Configure the script**:
   - Max Inventory Slots: 20
   - Drag UI references to their respective fields

## Step 10: Set Up Quest System
1. **Create an empty GameObject** named "QuestSystem"
2. **Add the `QuestSystem` script**
3. **Configure the script** with UI references

## Step 11: Set Up Layers
1. **Go to Edit → Project Settings → Tags and Layers**
2. **Create new layers**:
   - Player (Layer 8)
   - Enemy (Layer 9)
   - Obstacle (Layer 10)
3. **Assign objects to appropriate layers**

## Step 12: Create Basic Level
1. **Create a few platforms/obstacles** using Sprite Renderers
2. **Add Collider 2D components** to them
3. **Set their layer to "Obstacle"**
4. **Position them around the scene**

## Step 13: Create Move Tile Prefab
1. **Create an empty GameObject** named "MoveTile"
2. **Add a Sprite Renderer component**
3. **Add a Collider 2D component** (set as trigger)
4. **Add the `MoveTile` script**
5. **Drag this to the Player's GridMovement "Move Tile Prefab" field**

## Step 14: Test the Game
1. **Press Play** in Unity (or use the new Play button in Unity 6)
2. **Exploration Mode**:
   - Use WASD to move
   - Approach enemies to start combat
3. **Combat Mode**:
   - Use Move button or WASD to move on grid
   - Use Attack button or left-click to attack
   - Use End Turn button to end your turn
4. **Press I** to open inventory
5. **Press Q** to open quests
6. **Press ESC** to pause

## **Turn-Based Combat Controls**
- **Move Button**: Click to enable grid movement, then use WASD/Arrow keys
- **Attack Button**: Click to enable attack mode, then left-click on enemies
- **End Turn Button**: End your turn early
- **Item Button**: Open inventory to use items
- **WASD/Arrow Keys**: Move on grid when movement is enabled
- **Left Click**: Attack when attack mode is enabled
- **Right Click**: Cancel current action

## **Unity 6 Specific Notes**

### **New Interface Elements:**
- **Asset Browser**: Replaces the old Project window
- **New Inspector**: More organized component interface
- **Improved 2D tools**: Better sprite and animation handling
- **Enhanced UI system**: More intuitive UI creation

### **Navigation Tips:**
- **Use the Asset Browser** to find and organize your scripts
- **The Inspector is more organized** with better component management
- **Right-click context menus** are more comprehensive
- **Search functionality** is improved throughout the interface

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
- **In Unity 6, check the Asset Browser** for proper organization

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
- **Unity 6 has improved performance** - take advantage of new optimizations

Happy Turn-Based RPG development in Unity 6! 🎮⚔️♟️✨
