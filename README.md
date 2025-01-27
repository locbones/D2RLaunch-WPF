# About D2RLaunch
This app is designed to be a code-less, open, all-in-one solution for D2R mod management. It has many features designed to be used by either mod authors or players to help enhance their overall experience while creating and playing mods.
Mod Authors can customize D2RLaunch to their mod by editing their modinfo.json file.
Mod Players can download, update and customize mods in a few button clicks.
Some features require additional author **support** to function correctly, and will be indicated with a **+**

### -Player Feature List-
*Mod Manager-Related Features*
- **+Mod Downloader:** One-Click download and install of many popular mods
- **+Mod File Updater:** One-Click updating to the most recent version's mod files
- **Mod Creator:** One-Click creation of a blank, D2R compatible mod
- **Mod Launcher:** Quickly switch and control startup settings for installed mods
- **+News Display:** Read the recent mod news from author with real-time updating
- **Audio/Text Languages:** Change in-game text or audio languages individually
- **Game Data Extractor:** Built-In CASC Extractor to retrieve needed/missing files from your internal game storage
- **Fast Load Option:** Fast Load option which extracts ALL game files for slightly improved loading times
- **Queue-Skipping:** Disables access to BNET while app open to skip the queue-check process and for extra protection

*Quality-Of-Life Features*
- **Automatic Backup/Restore:** Automatically backs up your save and stash files, with quick restoral option
- **Advanced Monster Stats:** Ability to display real-time HP and Resistances for monsters and mercenaries
- **Dynamic HP Display:** Control monster HP bar colors for various life% threshholds
- **Stash Tab Naming:** Ability to individually rename stash tabs to your liking
- **+Buff Icon Display:** Design your own Buff Icon layout for tracking timed buffs (no timer, icons only)
- **Hotkey Controls:** Set hotkeys for Cube Transmuting or Auto-Sorting items
- **Merc Identifier:** Add a glowing indicator above your mercenary to help identify them in large crowds
- **Rune Identifier:** Add special visual effects to mid and high runes when dropped on the ground
- **Item Display:** Simplify common items, such as potions, scrolls, etc to use icons instead of text (screen clutter)
- **Hide Helmets:** Ability to Hide all Helmets from the world view display
- **Item Level Display:** Toggleable Item Levels Display (appears next to item name)

*Gameplay-Changing Features* (Author may enable/disable as desired)
- **Monster Customizations:** Customization controls that allow editing of Monster Density, Drop Rates, Experience, etc
- **Expanded Storage:** Ability to toggle various Expanded Storage functionality for Inventory, Merc, etc
- **More Shared Stash Tabs:** Unlocks 4 additional Shared Stash Tabs (1 personal, 7 shared)
- **Super Telekinesis:** Upgraded Telekinesis skill which can pick up any* item instead of just pots/scrolls
- **Special Event System:** Join Mod Author Hosted Special Events with the Event Management System

*Miscellaneous Features*
- **+Vault Access:** Quick-Access to our external app, *The Vault*, which allows infinite item storage and grail-tracking
- **Quick File Access:** Quickly access mod, save and app config files in the side menu
- **+Community Access:** Quick-Access to the mods wiki, discord or patreon sites
- **Map Seeds:** Quickly force map seeds with pre-defined map layouts (or use your own)
- **Font Switching:** Change in-game font to one of 12 currently supported fonts
- **Color Dyes:** Item Color Dye System for the world view display
- **+UI Themes:** Change UI Theme to the one used in the popular mod, ReMoDDeD
- **+Merged HUD Display:** Merged HUD Design which can be toggled on or off
- **Skill Icon Pack:** Choose one of 3 Skill Icon Packs currently available
- **+Runeword Menu Sorting:** Sort in-game runeword menu by name, type or level
- **Character Renaming:** Rename your character (in-game name also)
- **Character Map Seeds:** Edit your characters map seed directly from save file
- **Cinematic Subtitles:** Improved subtitle text and no longer formatted for the deaf/hard-of-hearing

### -Author Feature List-
- **Code-less:** No code needed to add D2RLaunch support; control news, features, community links or appearance
- **Easy Installs/Advertisement:** Add your mod to the database; players can easily view and install with one-click
- **Mod Updating:** Allow players to upgrade to your most recent mod version (with backups) in one-click
- **Real-Time News:** Control the news feed displayed to dynamically address your player-base
- **Feature Controls:** Enable or Disable certain mod features for your mod specifically
- **Player Experience:** Add frequently requested features and QoL perks to your mod instantly
- **Make It Your Own:** Control the displayed app logo and community links (discord, wiki, patreon)
- **Event Manager:** Easily setup real-time events for your player-base using the Event Manager System

# Adding D2RLaunch Support
In order to fully support the various features of this launcher, some additional steps will be needed from you.
Some features will require your permission to use, additional files provided or mod download/community links to reference.
As mentioned, no coding is needed for any of these customizations and the instructions for **full integration** are outlined below.

## Step 1: Edit modinfo.json
All of the D2RLaunch customizations are determined by this file, since it is included/required by your mod already.
With that said, please edit your modinfo.json file using the following template, being sure to respect the line counts.
The launcher will compare the players file to the web file to determine mod version status (for updating) and control features/info accordingly.
Change your enabled/disabled options as desired (Expect the Option Controls to be updated in future launcher releases)
```
{
    "name": "MyModName",
    "savepath": "MyModName/" 

/*
--My Mod Details--
Mod Download: https://MyModFilesLink.zip
Mod Config Download: https://MyModInfo.json
Mod Version: 0.1.2.3
News 1 Title: "My Mod was Updated! (Version 0.1.2.3)"
News 1 Message: "This is some news message that I like"
News 2 Title: "More News for the Community!"
News 2 Message: "This is some other news message that I like"

--General Options--
Map Layouts: Enabled
UI Themes: Enabled
Customizations: Enabled
Vault Access: Enabled

--Additional Options--
Item Icons: Enabled
Runeword Sorting: Enabled
HUD Display: Enabled
Monster Stats Display: Enabled

--Author Links--
Discord: https://MyDiscordLink.com
Wiki: https://MyWikiLink.com
Patreon: https://MyPatreonLink.com
*/

}
```

## Step 2: Setup Mod File Linking
In order to provide downloading, updating and configuration changes to be made dynamically by the launcher, we need to setup proper links.
For a proper link, it needs to be both **static** and **direct**. I will describe and outline my preferred methods for this, but use what you prefer.
- **Static** - This means when the file contents have been update, the link itself does not change
- **Direct** - This means that when the link is clicked, the file is downloaded directly, instead of a webpage button to download it

To setup a link that satisfies both of these requirements, you can use services such as Google Drive, Dropbox, Github, Amazon S3, etc.
Provided below are instructions for some of them I have used previously or currently:
- **Github** - Click the green Code button, then right-click the Download Zip option and select Copy Link Address (or similar)
- **Google Drive** - Copy your google provided link and paste into this [Online Generator](https://sites.google.com/site/gdocs2direct/) to convert it to a static-direct link.
When updating the file, it is important that you go to the **File Properties > File Information > Manage Versions** and update it using this method, to retain the same base link.
- **Dropbox** - Replace the **&dl=0** at the end of your dropbox provided url with **&dl=1**

Finally, as an optional substep, I recommend adding your mod to the [Mod Database](https://docs.google.com/spreadsheets/d/1RMqexbqTzxOyjk7tWbLhRYJk9RkzPGJ9cKHSLtsuGII/edit#gid=0), which allows players to easily view and install your mod.
*As long as you follow the above linking rules, then the links provided in your modinfo.json file will never need to be updated between mod or config changes!*

## Step 3: Enabling Optional Features
For some features, additional files must be provided to the launcher, due to the variety of changes that might be involved.
As an example, the Event Manager cannot host Special Events if it has no instructions or files provided for this task.
Any optional feature, or feature that requires file-safekeeping will use a new folder in your mods base directory called D2RLaunch. Each feature will be placed in a subfolder within.
Provided below will be instructions and details on how to setup and configure the folder structure for these optional features:

**Runeword Menu Sorting:** Utilizes a folder named **Runeword Sort** and may contain up to 6 files:
- **runewords-ab.json / helppanelhd-ab.json:** Used to display the runewords sorted **Alphabetically**, optionally includes help panel replacement file
- **runewords-it.json / helppanelhd-it.json:** Used to display the runewords sorted **By ItemType**, optionally includes help panel replacement file
- **runewords-lv.json / helppanelhd-lv.json:** Used to display the runewords sorted **By Required Level**, optionally includes help panel replacement file

**UI Theme:** Utilizes a folder named **UI Theme** and should contain 2 folders (for now):
- **Retail:** Used to display your own modified UI, based on the Retail Theme.
- **ReMoDDeD:** Used to display the heavily customized UI, based on the ReMoDDeD mod.

**Merged HUD:** Utilizes a folder named **Merged HUD** and should contain 2 folders (for now):
- **Retail:** Used to display your own modified HUD UI, based on the Retail Theme.
- **Merged:** Used to display the heavily customized HUD UI, based on the ReMoDDeD mod.

**Buff Icons:** Utilizes a folder named **Buff Icons** and should contain X files (explained below, not yet complete):
- **Skill_Names.txt:** This file is used to provide the launcher with your mods list of specifically buff skills (if different from retail).
- **Preview_SkillName.png:** For each custom buff icon you have, you will need to include an image the launcher can use.

**Custom Mod/App Logo:** Looks for a file named **Logo.png** to be used in the top left of D2RLaunch, to help showcase/customize it to your mod.
The size of the logo can vary to your liking, but I recommend something around 200x200 or so.

As previously mentioned, some folders will be created automatically by the launcher, mostly for file-safekeeping:
- **Customizations:** This folder is created to store unedited copies of the armor, misc, weapons, levels and treasureclass txt files. This is needed for the Monster Customizations options to work correctly.
- **Monster Stats:** This folder is created to store edited copies of the monster hp bar layout files, for various option displays. This is needed for the Monster Stats option to work correctly.

## Step 4: Updating Mod, News, Features or Appearance
To update your mod or config files, simply replace the .zip or .json file used in w/e you service chose in **Step 2**.
If you followed those instructions carefully, then the link does not need updating and your edits are immediately live.
The next time D2RLaunch goes to download your mod or access the config file for controls, news, links, etc customization; it will retrieve the updated version(s).
You can use this to dynamically update mod files, allowed features, news messages, app logo, etc.
Keep in mind that you are *pushing* data to the web, and the launcher is *pulling* that data down. It cannot update what it does not have.

# Program Requirements/Specifications
In order to fully utilize this app, or receive staff support, some requirements must be met.
- **.NET Desktop Runtime 7.0:** This program is included in the D2RLaunch download, but can also be found via the [Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) website. It is needed to run the program itself.
- **C++ Redistributable 2015-2022:** This program is included in the D2RLaunch download, but can also be found via the [Microsoft](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022) website. It is needed to perform CASC Extraction of internal game storage.
- **Windows OS:** This is the only supported Operating System for the launcher, although emulation layers may also work for it (WINE, Lutris, etc)
- **Battle.Net Purchased:** This program is only intended for and actively tries to be restricted to, legally purchased D2R copies. If you want to support modding, then purchase the game!
- **Code Base:** This program was designed using C# and WPF. I am a novice coder, so expect inconsistencies, inefficiencies and general issues. I am constantly improving it also.

# Developer Notes
This app was made because I am passionate about helping everyone get the most out of their D2R experience.
I put much effort into making it hassle-free, openly editable and powerful, while balancing it with author intentions, efforts and respect.
Over the last few years of my free time, it's slowly grown into a powerfully capable app. So here's what I'd like to say now that it's been made open-source:

**To The Players:**
Please enjoy all of the added QoL features, mod controls and hassle-free file management that comes with using D2RLaunch.
I know sometimes the worst part about modding, can be dealing with all the file-updating/conflicts and general confusion around them.
I also know that everyone likes to play just a little bit differently, and noone can say no to some added Quality of Life.
So I hope this app, while not perfect, helps fill in a large gap in what may have been missing from your mod experience.

**To The Authors**
Please enjoy what I consider to be a very simple, code-less method to control the launchers various systems to your liking.
I know sometimes the worst part about modding, can be dealing with all the file-updating/conflicts; troubleshooting with your community, etc.
I also know that many of the same features get requested repeatedly by the players or wish were included as a part of the normal experience.
So I hope this app, while not perfect, helps provide some of that to your player-base; letting them enjoy your mod even longer or more deeply.

**To The Developers**
I respectfully request that you help me continue to improve this project rather than fork it, if you wish to make your own modifications.
Much effort was made to this keep app free of any perceived gate-keeping, while also allowing authors to control the launcher in a more dynamic fashion, without needing coding knowledge.
That said, I will not be attaching any licenses or any other form of red tape to this project. I wish for it to be open and hope you might be convinced to help improve it further.

In order to build this app for your own deployment, you will need to provide your own Licenses/API Keys for the following services:
**SyncFusion:** Used for various form controls and library functions
**Google Sheets:** Used to retrieve the Mod Listing from the Mod Database and update the Download Dropdown

This file should be named *appSettings.json* and placed in the *Resources* folder. An example file has already been included.
