# ☁ SkanksAIO - Gloomrot Update
Forked from [phillipsOG](https://github.com/phillipsOG/SkanksAIO) and updated to gloomrot.

A V-Rising mod that conects your server to discord and a webserver.

## Installation
* Extract and put the **_SkanksAIO_** folder into your `(Vrising Server)\BepInEx\plugins folder.`
* Start the server once to generate the config file.
* Then edit the config file to your liking.
* The database will generate in `plugins\SkanksAIO\config`
* If you for some reason want to edit the database you'll need [LiteDB.Studio](https://github.com/mbdavid/LiteDB.Studio/releases) and database management knowledge.

## Discord commands and Other Features
**Notes**:
- 🔒 Requires admin permissions
- <ins>_Underlined_</ins> keys come from the config file.

---

### webserver example (template included)
![img.png](https://i.imgur.com/6dY9qUG.png)

---

### Global chat discord link
Example:

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

# Configuration file
- <ins>**_ShowUserConnectedInDC_**</ins> This is a boolean if set to true the bot will announce when a user connects to the server.
- <ins>**_ShowUserDisConnectedInDC_**</ins> This is a boolean if set to true the bot will announce when a user disconnects from the server.
- <ins>**_AnnounceTimer_**</ins> This is the time between messages in seconds.
- <ins>**_AnnounceEnabled_**</ins> This is a boolean if set to true the messages will be announced.
- <ins>**_AnnounceRandomOrder_**</ins> This is a boolean if set to true the messages will be announced in a random order.
- <ins>**_AnnounceMessages1-5_**</ins> These are the messages that'll get announced in the discord channel.
- <ins>**_Token_**</ins> This is your discord bot token you'll have to make [one](https://discord.com/developers/applications) if you haven't already and then u can get it from your bot.
- <ins>**_ChannelId_**</ins> This is the channel id of the channel you want the bot to post in.
- <ins>**_AdminRoleId_**</ins> This is the role id of the adminrole, only users with the admin role will be able to run admin commands.
- <ins>**_Title_**</ins> This is the title text of the embed it will show up in the `/status`, `/time` and `/leaderboard` command.
- <ins>**_Footer_**</ins> This is the footer text of the embed it will show up in the `/status`, `/time` and `/leaderboard` command.
- <ins>**_FooterIcon_**</ins> This is the footer icon of the embed it will show up in the `/status`, `/time` and `/leaderboard` command (Make sure the link you put in there ends with any image file extension like `.png` or `.jpg`).
- <ins>**_ShowLeaderboardAsList_**</ins> This is a boolean if set to true the leaderboard will be shown as a list instead of a grid.
- <ins>**_Port_**</ins> This is the port the webserver will run on (leave this as <ins>**_default_**</ins> if you don't use this port for any other service on your computer!).

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
- I cannot guarantee it will work with online hosters like G-Portal!

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
# Default value: 60
AnnounceTimer = 60

## Enable auto messages system
# Setting type: Boolean
# Default value: false
AnnounceEnabled = false

## Random order for announcement messages
# Setting type: Boolean
# Default value: false
AnnounceRandomOrder = false

## Message that will be announced
# Setting type: String
# Default value: 
AnnounceMessage1 =

## Message that will be announced
# Setting type: String
# Default value: 
AnnounceMessage2 =

## Message that will be announced
# Setting type: String
# Default value: 
AnnounceMessage3 =

## Message that will be announced
# Setting type: String
# Default value: 
AnnounceMessage4 =

## Message that will be announced
# Setting type: String
# Default value: 
AnnounceMessage5 =

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

[Web]

## Enable the webserver
# Setting type: Boolean
# Default value: false
Enable = false

## Port the webserver will run on
# Setting type: Int32
# Default value: 8080
Port = 8080

```

---

# Support me!
I have a Patreon now! So please support me [Here](https://www.patreon.com/user?u=97347013) You'll get early access to dev builds like this one!

# Developer & credits
<details>

### V rising modding [Discord](https://discord.gg/XY5bNtNm4w)
### Current Developer
- `skythebro/skyKDG` - Also known as realsky on discord

### Original Creator & Developer
- [phillipsOG](https://github.com/phillipsOG/SkanksAIO)

</details>
