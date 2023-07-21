# ☁ SkanksAIO - Gloomrot Update
Forked from [phillipsOG](https://github.com/phillipsOG/SkanksAIO) and updated to gloomrot.

A V-Rising mod that conects your server to discord and a webserver.

## Installation
* Extract both folders into your (Vrising Server)\BepInEx\plugins folder.
* Start the server once to generate the config file.
* Then edit the config file to your liking.

## Discord commands and Other Features
**Notes**:
- 🔒 Requires admin permissions
- <ins>_Underlined_</ins> keys come from the config file.

---

### webserver example (template included)
![img.png](https://i.imgur.com/6dY9qUG.png)

### Commands are easily viewable using <ins>**/**</ins> on discord
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


# Configuration file
- <ins>**_Token_**</ins> This is your discord bot token you'll have to make [one](https://discord.com/developers/applications) if you haven't already and then u can get it from your bot.
- <ins>**_ChannelId_**</ins> This is the channel id of the channel you want the bot to post in.
- <ins>**_AdminRoleId_**</ins> This is the role id of the adminrole, only users with the admin role will be able to run admin commands.
- <ins>**_Title_**</ins> This is the title text of the embed it will show up in the `/status`, `/time` and `/leaderboard` command.
- <ins>**_Footer_**</ins> This is the footer text of the embed it will show up in the `/status`, `/time` and `/leaderboard` command.
- <ins>**_FooterIcon_**</ins> This is the footer icon of the embed it will show up in the `/status`, `/time` and `/leaderboard` command.
- <ins>**_ShowLeaderboardAsList_**</ins> This is a boolean if set to true the leaderboard will be shown as a list instead of a grid.
- <ins>**_Port_**</ins> This is the port the webserver will run on.


```ini
[Chat]

## Prefix for all chat commands
# Setting type: String
# Default value: .
CommandPrefix = .

[Discord]

## Bot Token from https://discord.com/developers/applications
# Setting type: String
# Default value: 
Token = 

## Channel ID of the channel to post messages to
# Setting type: UInt64
# Default value: 0
ChannelId = 

## ID of an Administrative role in your discord server.
# Setting type: UInt64
# Default value: 0
AdminRoleId = 

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

## Port the webserver will run on
# Setting type: Int32
# Default value: 8080
Port = 8080
```

# Support me!
I have a Patreon now! So please support me [Here](https://www.patreon.com/user?u=97347013) You'll get early access to dev builds like this one!

# Developer & credits
<details>

### [Discord](https://discord.gg/XY5bNtNm4w)
### Current Developer
- `skythebro/skyKDG` - Also known as realsky on discord

### Original Creator & Developer
- `[phillipsOG](https://github.com/phillipsOG/SkanksAIO)`

</details>