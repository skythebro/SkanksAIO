# ☁ SkanksAIO - Gloomrot Update
Forked from [phillipsOG](https://github.com/phillipsOG/SkanksAIO) and updated to gloomrot.

A V-Rising mod that connects your server to discord and a webserver plus some extra stuff.
Please report any issues [here](https://github.com/skythebro/SkanksAIO/issues).


## Installation
* Extract and put the **_SkanksAIO_** folder into your `(Vrising Server)\BepInEx\plugins folder.`
* Start the server once to generate the config file and json files.
* Then edit the config in `(Vrising Server)\BepInEx\config` and the Json files in `(Vrising Server)\BepInEx\config\SkanksAIO`
* Be sure to check out the [configurations](#configurations) section for more info on the config and json files.
* The database will generate in `plugins\SkanksAIO\config`
* If you for some reason want to edit the database you'll need [LiteDB.Studio](https://github.com/mbdavid/LiteDB.Studio/releases) and database management knowledge.

## Commands and Other Features
**Notes**:
- 🔒 Requires admin permissions
- <ins>_Underlined_</ins> keys come from the config file.

---

### webserver example (template included)
Make sure <ins>_**Enable**_</ins> is set to `true` in your server's `SkanksAIO.cfg` file. And that the <ins>_**port**_</ins> is port forwarded and allowed through your firewall.
If you get access denied error 5 for the webserver make sure you run the server as admin.
![img.png](https://i.imgur.com/6dY9qUG.png)


### webserver interactive map
Make sure <ins>_**EnableInteractiveMap**_</ins> is set to `true` in your server's `SkanksAIO.cfg` file. You can access the map by going to `http://yourip:port/map` in your browser.
![img.png](https://imgur.com/VLF10mW.png)

#### features
- [x] Player tracking
- [x] Custom marker placement using admin commands
- [x] Player stats per region WIP (total amount of players atm)

Player tracking example:
![img.png](https://i.imgur.com/Lms3r1D.png)

Custom markers example: 
(these are all manually placed, so use them for special locations, other marker options will be added later)

![img.png](https://i.imgur.com/96gZYZk.png)

Player stats example:

![img.png](https://imgur.com/EEWy74t.png)

Check out the map.html.twig file for icon color and size customization (WIP everything is in one file atm, will be split up later)

---

### Global chat discord link
Bot image in example is from [Decaprime's](https://github.com/decaprime) LeadAHorseToWater mod. (this image is just used for an example and is not included in this mod)

![img.png](https://i.imgur.com/vPHFD7E.png)

Make sure <ins>_**AllowGlobalChat**_</ins> is set to `true` in your server's `ServerGameSettings.json` file.
And for the individual user that wants to use this that _**Show Global Chat**_ is enabled in the games _**HUD**_ settings.
![img.png](https://i.imgur.com/wPA5k04.png)

---

### Commands are easily viewable using <ins>**/**</ins> on discord, All users will be able to see all commands (discord limitation) but won't be able to use all depending on permissions.
![img.png](https://i.imgur.com/Uwy0B2V.png)

#### `/leaderboard [playername=]`
This command shows a leaderboard card for the player specified. If no player is specified it will show the leaderboard for the server.

![img.png](https://imgur.com/CGqAsRa.png)

#### `/time`
This command shows the current time on the server (Day/Night).

![img.png](https://imgur.com/cMvV1iV.png)

#### `/status`
This command shows the current uptime, online players, total PVP kills and Average Player Rating of the server.

![img.png](https://i.imgur.com/GKHVVGM.png)

#### 🔒 `/ban [playername=]`
This command bans the specified player.

![img.png](https://i.imgur.com/dJVk26G.png)

#### 🔒 `/unban [playername=]`
This command unbans the specified player.

![img.png](https://i.imgur.com/STCRP4g.png)

#### 🔒 `/kick [playername=]`
This command kicks the specified player.

![img.png](https://i.imgur.com/FSCW684.png)

---



---
## Ingame commands
Im using the default command prefix here but you can change it in the config.

#### `!playercount`
This command lists the amount of players on the server.

#### 🔒 `!reloadskanks`
This command reloads the server config and json files.

#### 🔒 `!listcommands`
This command lists all the available commands.

#### 🔒 `!addmarker [markername] [type]`
This command adds a custom marker to the map from where you're standing. For a list of types, use the command <ins>_**listmarkertypes**_</ins>

#### 🔒 `!removemarker [markername]`
This command removes marker with the given name.

#### 🔒 `!listmarkertypes`
This command lists all the usable marker types.

#### 🔒 `!listplacedmarkers`
This command lists all of the placed markers names, types and locations.

# Configurations
- <ins>**_ShowUserConnectedInDC_**</ins> This is a boolean if set to true the bot will announce when a user connects to the server.
- <ins>**_ShowUserDisConnectedInDC_**</ins> This is a boolean if set to true the bot will announce when a user disconnects from the server.
- <ins>**_AnnounceTimer_**</ins> This is the time between messages in seconds.
- <ins>**_AnnounceEnabled_**</ins> This is a boolean if set to true the messages will be announced.
- <ins>**_AnnounceRandomOrder_**</ins> This is a boolean if set to true the messages will be announced in a random order.
- <ins>**_Token_**</ins> This is your discord bot token you'll have to make [one](https://discord.com/developers/applications) if you haven't already and then u can get it from your bot.
- <ins>**_ChannelId_**</ins> This is the channel id of the channel you want the bot to post in.
- <ins>**_AnnounceChannelId_**</ins> This is the channel id of the channel you want the bot to post the announcements in. (leave default if you want it to be the same channel as <ins>**_ChannelId_**</ins>)
- <ins>**_AdminRoleId_**</ins> This is the role id of the adminrole, only users with the admin role will be able to run admin commands.
- <ins>**_Title_**</ins> This is the title text of the embed it will show up in the `/status`, `/time` and `/leaderboard` command.
- <ins>**_Footer_**</ins> This is the footer text of the embed it will show up in the `/status`, `/time` and `/leaderboard` command.
- <ins>**_FooterIcon_**</ins> This is the footer icon of the embed it will show up in the `/status`, `/time` and `/leaderboard` command (Make sure the link you put in there ends with any image file extension like `.png` or `.jpg`).
- <ins>**_ShowLeaderboardAsList_**</ins> This is a boolean if set to true the leaderboard will be shown as a list instead of a grid.
- <ins>**_EnablePvPkillTracker_**</ins> This is a boolean if set to true PvP kill tracking will be enabled (more info in config).
- <ins>**_EnableVIP_**</ins> This is a boolean if set to true the VIP functionality will be enabled. Just like you would add your admins steamid64 to the adminlist.txt now you add the steamid64 to the Vips.json file (check config for more info). For your VIP users. This allows VIP's to ignore the servers player limit.
- <ins>**_Enable_**</ins> This is a boolean if set to true the webserver will run.
- <ins>**_Port_**</ins> This is the port the webserver will run on (leave this as <ins>**_default_**</ins> if you don't use this port for any other service on your computer!).
- <ins>**_EnableInteractiveMap_**</ins> This is a boolean if set to true the interactive map will be enabled (WIP).
- <ins>**_InteractiveMapUpdateInterval_**</ins> This is the interval in seconds for the interactive map to update (WIP).

## Json file configurations
### Announcements.json
This file contains a list of announcement messages. These messages will randomly or not (<ins>**_AnnounceRandomOrder_**</ins>) be sent every (<ins>**_AnnounceTimer_**</ins> seconds) in the announcement discord channel or default channel (if <ins>**_AnnounceEnabled_**</ins> is true). You can add as many as you want.
 
```json
["Announcement 1","Announcement 2","Announcement 3"]
```

### Markers.json
This file contains a all the marker info for the interactive map. "testMarker" is the name that will show up on the map. Type is an enum. X and Y are the ingame coordinates for the location of the marker.
I recommend using the ingame command to add markers to this file.
```json
{
  "testMarker": {
    "Type": 15,
    "X": 0,
    "Y": 0
  },
  "testMarker2": {
    "Type": 21,
    "X": -100,
    "Y": -100
  }
}
```

### Messages.json
This file contains the default messages for each of the logon/logoff events. They'll get sent to the discord channel when a user connects or disconnects.
- %user% will get replaced with the username of the user that connected/disconnected.
- The newUserOnline and newUserOffline options cannot have %user% because the player won't have a name yet.
- The newUserCreatedCharacter message gets sent when a user finishes character creation.

```json
{
  "online":"%user% is online!", 
  "offline":"%user% has gone offline!",
  "newUserCreatedCharacter":"%user% has finished character creation!",
  "newUserOnline":"A new vampire just joined!",
  "newUserOffline":"A vampire left before finishing character creation"
}
```

### OfflineMessage.json
This file can be filled with custom logoff messages for specific users.
- %user% will be replaced with the username of the user that disconnected.

```json
{
  "CharacterName":"%user% went offline...",
  "CharacterName2":"Where did %user% go? They went offline..."
}
```

### OnlineMessage.json
This file can be filled with custom logon messages for specific users.
- %user% will be replaced with the username of the user that connected.

```json
{
  "CharacterName":"%user% is back baby!",
  "CharacterName2":"The best V rising player: %user% is here!"
}
```

### Vips.json
This file can be filled with steamid64's of VIP users. (Previously known as SkanksAIO.VIPlist.txt, be sure to add the steamid64's to the json instead of the old txt one!)

```json
[
  76561197960000000,
  76561197960000001,
  76561197960000002
]
```

---

# Video tutorial for setting up discord bot and extra info
- This [Youtube](https://youtu.be/4XswiJ1iUaw) video shows how to create and invite a bot to your server 
- In the **_URL generator_** section make sure you click on `Send Messages`, `Read Message History`, `Read Messages/View Channels` and `Manage Messages`
- Then the only thing you have to do is put your bot token into the config file and get the id's of the channel and the admin server role and put those in the config too.
- Make sure you have discord developer mode _**enabled**_ otherwise you cannot copy any ID's.
- How to copy channel ID:

![img.png](https://i.imgur.com/cFOGfeY.png)

- Open your server settings and go to Roles then click on the 3 dots and click copy Role ID to copy the ID:

![img.png](https://i.imgur.com/7kWduGW.png)

- This Should be all you need

---

### DISCLAIMER
- I cannot guarantee it will work on docker hosted servers!

```ini
[Announcements]

## Show in discord chat when users connect
# Setting type: Boolean
# Default value: true
ShowUserConnectedInDC = true

## Show in discord chat when users disconnect
# Setting type: Boolean
# Default value: true
ShowUserDisConnectedInDC = true

## Time between messages in seconds
# Setting type: Single
# Default value: 180
AnnounceTimer = 180

## Enable auto messages system
# Setting type: Boolean
# Default value: false
AnnounceEnabled = false

## Random order for announcement messages
# Setting type: Boolean
# Default value: false
AnnounceRandomOrder = false

[Chat]

## Prefix for all chat commands
# Setting type: String
# Default value: !
CommandPrefix = !

[Discord]

## Bot Token from https://discord.com/developers/applications
# Setting type: String
# Default value: 
Token = 

## Channel ID of the channel to post messages to
# Setting type: UInt64
# Default value: 0
ChannelId = 0

## Channel ID of the channel to post Announcements to (leave default if you want it to be in the same channel as the ChannelId) setting
# Setting type: UInt64
# Default value: 0
AnnounceChannelId = 0

## ID of an Administrative role in your discord server.
# Setting type: UInt64
# Default value: 0
AdminRoleId = 0

## Title for embedded message
# Setting type: String
# Default value: 
Title = 

## Footer for embedded message
# Setting type: String
# Default value: 
Footer = 

## Footer icon for embedded message
# Setting type: String
# Default value: 
FooterIcon = 

## If true, the leaderboard will be shown as a table instead of a grid.
# Setting type: Boolean
# Default value: false
ShowLeaderboardAsList = false

[Server]

## Enables the PvP Kill Tracker. Warning: if disabled ELO wont update when killing other players (because it doesnt track kills anymore (only applicable to PvP)).
# Setting type: Boolean
# Default value: true
EnablePvPKillTracker = true

[VIP]

## Enables the VIP functionality. the txt file will generate in the bepInEx config folder SkanksAIO/SkanksAIO.VIPlist.txt folder after restart. This txt file will be read at startup or when reloading. To add a user to VIP you need to add their steamid64 to the file. (1 per line)
# Setting type: Boolean
# Default value: false
EnableVIP = false

[Web]

## Enable the webserver
# Setting type: Boolean
# Default value: false
Enable = false

## Port the webserver will run on
# Setting type: Int32
# Default value: 8080
Port = 8080

## Enables the interactive map
# Setting type: Boolean
# Default value: false
EnableInteractiveMap = false

## Enables tracking of players on the interactive map
# Setting type: Boolean
# Default value: false
TrackPlayersOnMap = false

## Interval in seconds for the interactive map to update. Don't set this too low or you might get rate limited.
# Setting type: Int32
# Default value: 10
InteractiveMapUpdateInterval = 10
```

---

# Developer & credits
<details>

### V rising modding [Discord](https://discord.gg/XY5bNtNm4w)
### Current Developer
- `skythebro/skyKDG` - Also known as realsky on discord

### Original Creator & Developer
- [phillipsOG](https://github.com/phillipsOG/SkanksAIO)

</details>
