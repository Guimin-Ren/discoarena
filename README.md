# XRoom README
### Update Notes:

##### OCT 11:

add a UI sample scene in the project:
Open UISample scene to inspect the changes. operate Score manager to process the score UI.

 

# Environment Settings

#### Unity version: 

2020.1.11f1

#### Unity package (already included in the repo):

##### AR:

AR Foundation: 4.1.0

ARCore XR Plugin: 4.1.0

##### Network:

Mirror: 23.0.3

##### Debug:

Lunar Mobile Console: 1.7.0

Runtime Inspector & Hierarchy: 1.5.4



# Installation and build Instruction

### Unity:

1. Download repository 
2. Open project with unity 2020.1.11f1
3. Open `Scenes/playtest` 

### APK Build:

1. Click `File/Build Settings`
2. Switch platform to Android
3. Add `Scenes/playtest` to scenes in build
4. click `Build`

### QR code:

This image can also be found under `Assets/ARTexture/QRcode`

![kdqSZ](https://github.com/ETC-2020-Fall-XRoom/XRoom/blob/main/Documents/kdqSZ.jpg)



# System Overview

![StructureMap](https://github.com/ETC-2020-Fall-XRoom/XRoom/blob/main/Documents/StructureMap.PNG)

<center> Structure Map</center>

The main system logic is as follow:

1. Players scan QRcode to place virtual space scene prefab
2. Clients send self relative position information back to server for synchronization
3. Server calculates ray behavior, player status information
4. Server send relative calculated position and rotation information back to client
5. Clients retrieve calculated relative transform data and set virtual objects in local world accordingly
6. Back to step2 



# Script manual

### `LunarConsole/`

Folder of lunar console Android debugging package

### `Mirror/`

Folder of Mirror Networking package

### `Scripts/ `

##### `AR Transform/`

1. ###### `Mirror/`

   `MirrorBehavior`: Behavior of all mirrors both handhold and generated by players

2. ###### `Player/`

   `RelativeTransformController`: Control player transform transformation (relative  ---- local), mainly about camera position and rotation

   `RelativeTransformRespawner`: Control player behavior of respawning mirror 

3. ###### `Ray/`

   `RayBehavior`: Control behavior of a single ray

   `RelativeRaySetter`: Set ray local transform, color and material information (must use with ray behavior)

4. ###### `Scene/`

   `ARMarkerAdjustController`: Control the adjustment of AR marker and virtual scene transform

   `ARSceneRespawner`:  Respawn virtual scene prefab with image tracking process

   `ARSpaceController`:  Control light source initialization

##### `HelperClass/`

1. `CommandLine`: Helper class for run build in command line mode
2. `WayPoint`:  Helper class for display object with gizmo

##### `IdentityAndStatus/`

1. `MarkerIdentity` : Identity for AR marker 
2. `PlayerIdentity` : Identity for players
3. `PlayerStatus` : Status of players

##### `Manager/`

1. `RayManager`: Manager tracks ray cluster information
2. `AudioManager`: Manager audio effect and back ground music
3. `LightSourceNetController`: Manager light generation and destroy
4. `ScoreManagerNetwork`: Manager score counting for each player

##### `MirrorOverride/`

1. `CustomHUD` : Override version of Mirror Network HUD
2. `NetworkFlashLight` : Override version of Mirror network manager 

# Deploy game server on AdvantEdge

### What you need to do before the deployment:

1. install AdvantEdge Edge Computing platform from here:
   https://github.com/InterDigitalInc/AdvantEDGE/wiki
2. Download the dockerize.sh, scenario.yaml, Dockerfile, commands.sh from Edge File folder.
3. Build your game server in linux(with the server build on) and replace your game's name in commands.sh.
4. Copy the dockerize.sh, scenario.yaml, Dockerfile, commands.sh to your server build folder.

### How to deploy the game

1. Run the AdvantEdge and import the scenario.yaml to through the platform dashboard.
2. Replace the ip address to your own in dockerize.sh
3. Running the following command in your game server build directory:
   ./dockerize.sh
4. Build a new sandbox and deploy the edge platform.
5. Now the server should be running.