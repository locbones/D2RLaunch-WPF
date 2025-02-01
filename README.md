# Table of Contents

- [About D2RLaunch](#about-d2rlaunch)
    - [Player Features](#-player-feature-list-)
    - [Author Features](#-author-feature-list-)
- [Player Setup Guide](#player-setup-guide)
- [Author Setup Guide](#author-setup-guide)
    - [Step 1: Modinfo.json](#step-1-edit-modinfojson)
    - [Step 2: File Linking](#step-2-setup-mod-file-linking)
    - [Step 3: Optional Features](#step-3-enabling-optional-features)
    - [Step 4: Updating Files](#step-4-updating-mod-news-features-or-appearance)
- [Program Requirements/Specs](#program-requirementsspecifications)
- [Developer Notes](#developer-notes)
- [Credits](#credits)


# About D2RLaunch
This app is designed to be a code-less, open, all-in-one solution for D2R mod management.<br>
It is designed to be used with **Single-Player** mods that utilize the **Latest Versions** of D2R's filebase.<br>
(Please use [D2RLAN](https://github.com/locbones/D2RLAN-WPF), The **Multi-Player**-Enabled fork of this app) (TCP/IP).<br>
It has many features designed to be used by both mod authors and players to enhance their overall experience.<br>
Mod Authors can customize D2RLaunch to their mod by editing their modinfo.json file.<br>
Mod Players can download, update and customize mods in a few button clicks.<br>
Some features require additional author support to function correctly, and will be indicated with a **+**<br>

<img src="https://static.wixstatic.com/media/698f72_11a7eaa882f24e969c85b0ef680746ab~mv2.png" alt="D2RLaunch Home View" width="820">
Publicly ready download can be found at http://d2rmodding.com/d2rlaunch


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
- **Queue-Skipping:** Disables BNET access while app open to skip the queue-check process and extra protection

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
- **Monster Customizations:** Various controls which allow editing of Monster Density, Drop Rates, Experience, etc
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

# Player Setup Guide
The process to setup the launcher should be simple and straight forward, but here's how to do it:<br>
    - **Step 1:** Download and Install the [.NET Desktop Runtime 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-7.0.20-windows-x64-installer) which is required for the app to run.<br>
    - **Step 2:** Download D2RLaunch from [D2RModding](https://d2rmodding.com/d2rlaunch), then drag the *D2RLaunch* folder from the downloaded .zip to your Desktop or other convenient location.<br>
    - **Step 3a:** Browse to the *Launcher* folder and run *D2RLaunch.exe*.<br>
    - **Step 3b:** Depending on your version, an update notification may appear in the bottom left of the app.<br>
    - **Step 4:** Click the *Download New Mod* button and select your desired mod from the dropdown box.<br>
    - **Step 5:** View the *QoL Options* and other settings to customize things to your liking.<br>
    - **Step 6:** Press the *Play Mod* button to start the mod with your chosen configuration and enjoy!

*You may need to run the launcher as Administrator and/or exclude the folder from your Antivirus<br>
**For any issues or questions, please reach out in our [Discord](https://www.discord.gg/pqUWcDcjWF)

# Author Setup Guide
In order to fully support the various features of this launcher, some additional steps will be needed from you.<br>
Some features will require your permission to use, additional files provided or mod download/community links.<br>
As mentioned, no coding is needed for any of these customizations and the instructions are outlined below.<br>

## Step 1: Edit modinfo.json
All of the D2RLaunch customizations are determined by this file, since it is included/required by your mod already.<br>
With that said, please edit your modinfo.json file using the following template, being sure to respect the line counts.<br>
The launcher will compare the players file to the web file to determine mod version status and control features/info.<br>
Change your enabled/disabled options as desired (Expect the Option Controls to be updated in future releases)<br>
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
For a proper link, it needs to be both **static** and **direct**.<br>
    - **Static** - This means when the file has been updated/replaced, the link itself does not change
- **Direct** - This means that when the link is clicked, the file is downloaded directly, not a webpage button download

To setup a link that satisfies both of these requirements, you can use services such as Google Drive, Dropbox, Github, Amazon S3, etc.
Provided below are instructions for some of them I have used previously or currently:
- **Github** - Click the green Code button, then right-click the Download Zip option and select Copy Link Address
- **Google Drive** - Copy your google provided link into this [Online Generator](https://sites.google.com/site/gdocs2direct/) to convert it to a static-direct link.<br>
When updating the file, you must use the **File Properties > File Information > Manage Versions** method<br>
    - **Dropbox** - Replace the **&dl=0** at the end of your dropbox provided url with **&dl=1**

The First time you upload modinfo.json, you will need to use a dummy config link, because you havnt uploaded it yet<br>
I recommend adding your mod to the [Mod Database](https://docs.google.com/spreadsheets/d/1RMqexbqTzxOyjk7tWbLhRYJk9RkzPGJ9cKHSLtsuGII/edit#gid=0), which allows players to easily view and install your mod.<br>
*As long as you follow the above linking rules, then the links provided in your modinfo.json file will never need to be updated between mod or config changes!*<br>

## Step 3: Enabling Optional Features
For some features, additional files must be provided to D2RLaunch, due to the variety of changes/complexity.<br>
As an example, the Event Manager cannot host Special Events if it has no instructions or files provided for this task.<br>
Any feature that is optional or requires file-safekeeping will use a new D2RLaunch folder in your mod directory.<br>
Each feature will be placed in it's own subfolder within it, following these rules:<br>

**Runeword Menu Sorting:** Utilizes a folder named **Runeword Sort** and may contain up to 6 files:
- **runewords-ab.json / helppanelhd-ab.json:** Used to display the runewords sorted **Alphabetically**
- **runewords-it.json / helppanelhd-it.json:** Used to display the runewords sorted **By ItemType**
- **runewords-lv.json / helppanelhd-lv.json:** Used to display the runewords sorted **By Required Level**<br>

*helppanel* files are optional if you've chosen to replace it with your runeword menu (quick-access)

**UI Theme:** Utilizes a folder named **UI Theme** and should contain 2 folders (for now):
- **Retail:** Used to display your own modified UI, based on the Retail Theme.
- **ReMoDDeD:** Used to display the heavily customized UI, based on the ReMoDDeD mod.

**Buff Icons:** Utilizes a folder named **Buff Icons** and should contain X files (explained below, not yet complete):
- **Skill_Names.txt:** Used to provide the launcher with your mods list of *buff skills* (if different from retail).
- **Preview_SkillName.png:** For each custom buff icon you have, include an image the launcher can use for it.

**Custom Mod/App Logo:** Looks for a file named **Logo.png** to be used in the top left of D2RLaunch.<br>
The size of the logo can vary to your liking, but I recommend something around 200x200 or so.

**Merged HUD:** Contains the needed files for the **Merged HUD** Option:

As previously mentioned, some folders will be created automatically by the launcher, mostly for file-safekeeping:
- **Customizations:** Created to store unedited copies of the armor, misc, weapons, levels and treasureclass txt files.<br>
This is needed for the Monster Customizations options to work correctly.<br>
    - **Monster Stats:** Created to store edited copies of the monster hp bar layout files, for various option displays.<br>
This is needed for the Monster Stats option to work correctly.

## Step 4: Updating Mod, News, Features or Appearance
To update your mod or config files, simply replace the .zip or .json file used in w/e you service chose in **Step 2**.<br>
If you followed those instructions carefully, then the link does not need updating and your edits are immediately live.<br>
When D2RLaunch goes to download the mod or access the config file; it will retrieve the updated version(s).<br>
You can use this to dynamically update mod files, allowed features, news messages, app logo, etc.<br>
Keep in mind that you are *pushing* data to the web, and the launcher is automatically *pulling* that data down.<br>

# Program Requirements/Specifications
In order to fully utilize this app, or receive staff support, some requirements must be met.
- **.NET Desktop Runtime 7.0:** This program is included in the D2RLaunch download, but can also be found via the [Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) website. It is needed to run the program itself.<br>
    - **C++ Redistributable 2015-2022:** This program is included in the D2RLaunch download, but can also be found via the [Microsoft](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022) website. It is needed to perform CASC Extraction of internal game storage.
- **Windows OS:** This is the only supported Operating System for the launcher, although emulation layers may also work for it (WINE, Lutris, etc)<br>
    - **Battle.Net Purchased:** This program is only intended for and actively tries to be restricted to, legally purchased D2R copies. If you want to support modding, then purchase the game!<br>
    - **Code Base:** This program was designed using C# and WPF. I am a novice coder, so expect inconsistencies, inefficiencies and general issues.<br>
    - **D2RHUD.dll:** This file is used to enable hotkey controls and advanced monster stats display options [(Source)](https://github.com/locbones/D2RHud)<br>
    - **CASCLib.NET.dll:** This file is used to enable file extraction from your internal game storage [(Source)](https://github.com/ladislav-zezula/CascLib)<br>

# Developer Notes
This app was made because I am passionate about helping everyone get the most out of their D2R experience.<br>
I put much effort into making it easy, open and powerful, while balancing it with author intentions/efforts.<br>
So thanks for everyones patience and support during this free-time side project of mine; some final words on it:

**To The Players:**<br>
Enjoy the many new QoL features, mod controls and hassle-free modding that comes with using D2RLaunch.<br>
I know sometimes the worst part about modding, can be dealing with all the frustration and confusion around them.<br>
I also know that everyone likes to play just a little bit differently, and noone can say no to some added QoL.<br>
So I hope this app helps fill in a large gap of what may have been missing from your mod experience.

**To The Authors**<br>
Enjoy what I hope is a very simple, code-less method to control the launchers various systems to your liking.<br>
I know sometimes it can be frustrating dealing with so many files, troubleshooting with your community, etc.<br>
I also know that many of the same features get requested by players or wish were included by default.<br>
So I hope this app helps provide some of that to your player-base; letting them enjoy your mod even longer.

**To The Developers**<Br>
I respectfully request that you help improve this project rather than fork it, if you wish to modify it.<br>
Much effort was made to allow authors control of the launcher in a dynamic fashion, without coding needed.<br>
I wish for it to be open and hope you might be convinced to help improve it further.<br>
With that said; a standard **GPL License** has been attached to this project.

In order for build deployment, you will need to provide your own Licenses/API Keys for the following services:<br>
**SyncFusion:** Used for various form controls and library functions<br>
**Google Sheets:** Used to retrieve the Mod Listing from the Mod Database and update the Download Dropdown<br>
File should be named *appSettings.json* and placed in the *Resources* folder. An example file has already been included.<br>

# Credits
Special thanks to the following people or groups for their help along this project's journey so far:<br>
    - [Ethan-Braddy](https://github.com/Ethan-Braddy) for helping convert this project from WinForms to WPF, improving stability/performance<br>
    - [Dschu012](https://github.com/dschu012) for being there to answer my dumb questions and D2RHUD's base implementation<br>
    - [D2RModding Community](https://www.discord.gg/pqUWcDcjWF) for being patient, supportive and assisting with bug-reports or improvements<br>

# Changelogs
This changelog list is mainly for reference use, as the project repo and codebase has changed much over time<br>

<details>
  <summary>Changelog Group (2.5.1 - 3.0.0)</summary>
<details><br>
  <summary>2.5.2</summary>
    - Re-Added Popup Warning for Save File Size (Patch 1.6.9, lost in Rebuild)<br>
    - Added "Ultra" rune support for RMD
</details>
<details>
  <summary>2.5.1</summary>
    - Removed D2RHUD.dll as a Project Resource<br>
    - Launcher will now retrieve latest D2RHUD.dll file from Github when missing<br>
    - Fixed an issue that would cause launcher version checking to fail if program crashed during task previously
</details>
</details>
<details>
  <summary>Changelog Group (2.0.1 - 2.5.0)</summary>
<details>
  <summary>2.5.0</summary>
    - Source Code is now available via https://github.com/locbones/D2RLaunch-WPF<br>
    - Code Cleanup performed to comment, organize and simplify functions<br>
    - More logging info added to output logs to indicate options used, function chain status, etc<br>
    - Fixed an issue where Customizations UI Display would not update correctly after reloading it<br>
    - Improved the Auto Backup functionality and logging data<br>
    - Shared Stash Backups will now appear in the Restoral dropdown box<br>
    - Added the ability to restore shared stash files individually instead of grouped with character<br>
    - Cold color has been lightened to make it more readable (when using the Advanced Monster Stats Display option)<br>
    - An "Overlay Fix" option has been added for improved compatibility with MSI Afterburner<br>
    - .NET Runtime package is now included with core package files<br>
    - Core file package has been updated from 2.3.3 to 2.5.0
</details>
<details>
  <summary>2.4.8 - 2.4.9</summary>
    - Fixed an issue with retrieving save files location for "alternate" OS/User configs
</details>
<details>
  <summary>2.4.7</summary>
    - Improved/Reduced code for managing keybind and monster stat display QoL Options<br>
    - Fixed an issue where Monster Stats wouldn't display if no QoL keybinds were set
</details>
<details>
  <summary>2.4.6</summary>
    - Fixed an issue where Item ilvls display option would revert to "Don't Modify" option after changing it<br>
    - Fixed an issue that would prevent extra stash tabs from being unlocked for existing stash files
</details>
<details>
  <summary>2.4.5</summary>
    - Fixed an issue with blank app screen when no mods or user settings were detected
</details>
<details>
  <summary>2.4.4</summary>
    - Added Character Map Seed Reader/Editor
</details>
<details>
  <summary>2.4.3</summary>
    - Added "Don't Modify" option to Item iLvls<br>
    - Added support for newline characters in News Display messages<br>
    - Fixed an issue where author settings weren't refreshing with mod selection change
</details>
<details>
  <summary>2.4.2</summary>
    - Changed Download App Updates method to asynchronous operation for improved performance<br>
    - Fixed an issue with vault update function
</details>
<details>
  <summary>2.4.1</summary>
    - Added Event Manager system to Side Menu (Author Controlled Special Events)<br>
    - Fixed an issue which would cause restore function to open wrong folder on first usage<br>
    - Fixed an issue which would cause restore function to parse timestamps incorrectly
</details>
<details>
  <summary>2.4.0</summary>
    - Fixed an issue that would cause subtitles to disappear partway through cinematic<br>
    - Moved subtitle positioning down to letterbox area<br>
    - Improved Subtitle replacement logic
</details>
<details>
  <summary>2.3.9</summary>
    - Fixed an issue that would result in main screen failing to load if user had no mods installed (again)<br>
    - Added a new QoL Option to toggle Cinematic Subtitle Display Mode (SDH or Standard)
</details>
<details>
  <summary>2.3.8</summary>
    - Fixed an issue that would cause Launch Options box to not display arguments<br>
    - Separated the Monster Stats and Keybind Plugin functions so they can be used independently
</details>
<details>
  <summary>2.3.7</summary>
    - Fixed an issue that would cause hotkey bindings to fail if multiple functions used the same hotkey<br>
    - Added more keybinds to the recognized keys list<br>
    - Added a function to Autosort Cube
</details>
<details>
  <summary>2.3.6</summary>
    - Fixed an issue that would result in main screen failing to load if user had no mods installed<br>
    - Added preliminary support for custom keybind functions: Transmute, and Autosort
</details>
<details>
  <summary>2.3.5</summary>
    - Hotfix for game launch error (game.exe)
</details>
<details>
  <summary>2.3.4</summary>
    - Added Status indicator to CASC Extraction<br>
    - Added an option not to modify UI Theme<br>
    - Improved save folder discovery function<br>
    - Fixed an issue that would cause mod files to extract incorrectly when downloading a mod
</details>
<details>
  <summary>2.3.3</summary>
    - Fixed an issue that caused app blank screen if user settings did not yet exist
</details>
<details>
  <summary>2.3.2</summary>
    - D2R Save Folder should now be dynamically found by GUID instead of default folder structure<br>
    - CASC Extractor will now show status updates as it progresses<br>
    - Fixed an issue that would cause window not to exit when CASC extraction completed<br>
    - Fast Load Status will be updated automatically after extraction completes<br>
    - Fixed an issue that would cause launch arguments not to update when respec option was toggled (display issue only)
</details>
<details>
  <summary>2.3.1</summary>
    - D2R Launch Arguments are now shown and customizable (for seeds and such)<br>
    - DirectTxt Functionality has been altered for individual mods (to help avoid user error)<br>
    - "Fast Load" status will be evaluated and displayed to user (requires CASC extraction)
</details>
<details>
  <summary>2.3.0</summary>
    - The Create-A-Mod option will now use it's own mod folder for saves and copy retail save files to it
</details>
<details>
  <summary>2.2.9</summary>
    - Advanced Monster Stats will now display for player summons and will be hidden for NPC's<br>
    - Exocet font will now be provided by D2RLaunch and will be used for Stats Display<br>
    - The "Fix Stash" option has been removed; it applies automatically (if needed)
</details>
<details>
  <summary>2.2.8</summary>
    - Fixed more issues with Merged HUD option<br>
    - Added D2RLaunch Error Logs Quick Access link to Side Menu
</details>
<details>
  <summary>2.2.7</summary>
    - Fixed another launching issue (typos suck, sorry)
</details>
<details>
  <summary>2.2.6</summary>
    - Fixed an issue which would cause mod unable to launch depending on user settings<br>
    - Disabled the -immunity reduction updating on monster stats display (to avoid confusion until I can better display its quirks)
</details>
<details>
  <summary>2.2.5</summary>
    - Added Monster HP Bar GIF Preview for Stats Display option<br>
    - Fixed an issue with standard hud display being corrupted after using merged hud display and while on controller mode
</details>
<details>
  <summary>2.2.4</summary>
    - Fixed an issue with color dye option being enabled for ReMoDDeD players
</details>
<details>
  <summary>2.2.3</summary>
    - Fixed an issue which would cause show item levels option not to hide levels after enabling
</details>
<details>
  <summary>2.2.2</summary>
    - Added Borderless Window Mode Controls<br>
    - Fixed an issue with monster health files not being swapped correctly when toggling options
</details>
<details>
  <summary>2.2.1</summary>
    - Resource File Updates and Minor Bug Fixes
</details>
<details>
  <summary>2.2.0</summary>
    - Added Monster HP Bar Display Options<br>
    - Merged Dynamic HP Bar option with Monster HP<br>
    - Merged Monster Stats option with Monster HP<br>
    - Updated Monster Stats Display to also show immunity reduction benefits
</details>
<details>
  <summary>2.1.9</summary>
    - Fixed an issue with Color Dye system which would not allow you to remove purple color dyes<br>
    - Fixed an issue with text displaying gold instead of red color on red dyed items<br>
    - Added Color Dye Preview Popup
</details>
<details>
  <summary>2.1.8</summary>
    - Fixed an issue with Expanded Storage Click Areas and Quest Border Panel image<br>
    - Fixed an issue which would cause Hireling Inventory panel border image to break if enabled then disabled
</details>
<details>
  <summary>2.1.6 - 2.1.7</summary>
    - Expanded Storage functions rebuilt to allow individualized side panel entries<br>
    - Updated Expanded function's needed files resource
</details>
<details>
  <summary>2.1.5</summary>
    - Fixed an issue with layout positioning when using the expanded stash feature<br>
    - Updated first-time use message for expanded features for further clarification
</details>
<details>
  <summary>2.1.4</summary>
    - Restored asynchronous behavior of Auto Backups and fixed backup intervals (proper)
</details>
<details>
  <summary>2.1.3</summary>
    - Fixed an issue that would cause Hardcore shared stash files not to be backed up<br>
    - Removed asynchronous behavior from AutoBackups; seemed to be causing irregular backup intervals
</details>
<details>
  <summary>2.1.2</summary>
    - Fixed an issue that would cause monster health bar to be desychronized after repeatedly switching between enabled/disabled status
</details>
<details>
  <summary>2.1.1</summary>
    - Fixed an issue that would cause decimals to be replaced by commas for some languages due to Windows Localization (breaks json file)
</details>
<details>
  <summary>2.1.0</summary>
    - Project DLL's embedded into app for user cleanup<br>
    - Fixed an issue which would cause Dynamic HP Bar's disabled option to break filename
</details>
<details>
  <summary>2.0.9</summary>
    - Dynamic Monster HP Bar feature added - Adds Warning and Critical HP thresholds to the bar, changing bar colors to indicate their life threshold<br>
(At 67% to 100%, bar is green; At 34% to 66%, the bar is orange; At 33% or below, the bar is red.)<br>
    - Adjusted Main Menu layouts for left/right quick toggle buttons<br>
    - Improved error catching and processing for Monster Stats Display option
</details>
<details>
  <summary>2.0.8</summary>
    - Added support for Item Color Dyes in QoL Options- allows changing world view colors of items<br>
(Cube 3x perfect gems of desired color with item to color dye - Skulls are considered black, Antidotes remove dyes)<br>
    - Disabled permanently activated features for ReMoDDeD players (color dyes, expanded storage, etc)
</details>
<details>
  <summary>2.0.7</summary>
    - Added preliminary support for toggleable Expanded Storage Options (QoL Options)<br>
    - Fixed an issue which would cause an object reference error when attempting to make character backups which did not yet exist
</details>
<details>
  <summary>2.0.6</summary>
    - Improved window popups to center and adjust based on main form location<br>
    - Image Previews on QoL Options page improved and match form size<br>
    - Fixed an issue that would cause an "Object reference not set to an instance of an object" error when attempting to backup characters on any mod that used the retail save location
</details>
<details>
  <summary>2.0.5</summary>
    - Fixed an issue that would cause a blank screen/failure for main UI to load for mods whose authors have not added D2RLaunch support<br>
    - Improved the Character Restore function to allow easy restoral of timestamped save files
</details>
<details>
  <summary>2.0.4</summary>
    - Fixed an issue that would cause an error for _profilehd.json to appear when using the HDR Fix option<br>
    - Code Cleanup and Comments in prep for Public Source Code Release
</details>
<details>
  <summary>2.0.3</summary>
    - Fixed an issue which would cause item icons not to toggle correctly<br>
    - Customizations option now available for authorized mods<br>
    - Window UI adjusted to allow smaller element sizing<br>
    - Minor theme/palette updates
</details>
<details>
  <summary>2.0.2</summary>
    - Resolved many issues which would cause QoL options to write incorrectly or in the wrong location<br>
    - Resolved all known and reported crashing issues for various mods
</details>
<details>
  <summary>2.0.1</summary>
    - Temporary hotfix for mod crashing issues
</details>
</details>
<details>
  <summary>Changelog Group (1.5.1 - 2.0.0)</summary>
<details>
  <summary>1.7.3 - 2.0.0</summary>
    - Initial Release of Re-Built Launcher (WinForms -> WPF), primarily spearheaded by [Ethan-Braddy](https://github.com/Ethan-Braddy)<br>
    - Dynamically scalable UI with increased responsiveness and efficiency<br>
    - Improved code execution, function loading, app stability and error reporting<br>
    - Better compatibility and integration with Windows<br>
    - Launcher no longer force updates on startup - can be deferred until user interaction<br>
    - If no D2R install path is found, it can now be manually specified by user<br>
    - Multiple/Infinite mod backups of previous versions may be made when updating<br>
    - Auto-Backups are now enabled by default and characters are saved into folder of their own name
</details>
<details>
  <summary>1.7.3</summary>
    - Mod version will now be read and applied to newly downloaded mods<br>
    - Fixed an issue that would cause mod version to update in a separate file when using the update method
</details>
<details>
  <summary>1.7.2</summary>
    - Updated Vault and Self-Updater links
</details>
<details>
  <summary>1.7.1</summary>
    - ReMoDDeD UI now forced for ReMoDDeD players (temporary)
</details>
<details>
  <summary>1.7.0</summary>
    - Adjusted target filesize for save warning to 7KB
</details>
<details>
  <summary>1.6.9</summary>
    - Fixed an issue which would cause density customizations to be un-revertable<br>
    - Added a save file size warning to the auto-backup feature<br>
    - Disabled item level and icons toggle for ReMoDDeD only (temporary)
</details>
<details>
  <summary>1.6.7 - 1.6.8</summary>
    - Backend fixes to deal with potential link unavailable issues<br>
    - Compatibility update for ReMoDDeD
</details>
<details>
  <summary>1.6.6</summary>
    - Fixed an issue that would cause mod to sometimes launch with incorrect arguments<br>
    - Fixed an issue that would cause customizations option to crash game if it wasn't able to find mod's needed files
</details>
<details>
  <summary>1.6.5</summary>
    - Fixed an issue which would cause some mods using .bins to not launch correctly if using buff icons<br>
    - Fixed an issue which would cause an error when attempting to read user settings when no mods are installed
</details>
<details>
  <summary>1.6.3 - 1.6.4</summary>
    - Added a backup server to avoid Error 429/500 some users were receiving during heavy traffic usage<br>
    - Fixed an issue that would cause an error when using the personalized stash tabs feature if the files already existed
</details>
<details>
  <summary>1.6.2</summary>
    - Added initial support for the "Buff Icon Manager" feature<br>
    - Integrated Assassin Charge Icons into the manager<br>
    - Improved the Download Mod feature to better handle various mod folder structures when extracting<br>
    - Fixed an issue where it would not create the MyUserSettings file in certain scenarios<br>
    - Fixed an issue that prevented ReMoDDeD players from using the -direct launch option
</details>
<details>
  <summary>1.6.1</summary>
    - Fixed a runtime error that would cause the CASC Storage functions to fail for many users
</details>
<details>
  <summary>1.6.0</summary>
    - CASC Storage is now supported<br>
    - Missing files that are required for QoL options will now be extracted from your CASC<br>
    - One-click setup of -direct -txt mode, including extracting files from CASC and overwriting with mod files<br>
    - Optional update option to skip CASC extraction and overwrite with mod files only<br>
    - Skill Icon Pack option added in QoL Options, currently supports default + 2 other icon pack choices
</details>
<details>
  <summary>1.5.8 - 1.5.9</summary>
    - Fixed an issue that caused icons not to apply to rejuvenation potions<br>
    - Fixed an issue that would sometimes cause an access denied error when running the vault<br>
    - Fixed an issue that would prevent correct item names from being displayed when switching back to no icons<br>
    - Fixed a sideloading issue for ReMoDDeD files/features
</details>
<details>
  <summary>1.5.7</summary>
    - Fixed an error that would popup due to item icon feature code changes<br>
    - Fixed an issue that would cause treasureclassex.txt to be incorrectly modified for existing edits
</details>
<details>
  <summary>1.5.6</summary>
    - Credentials update (house-keeping)
</details>
<details>
  <summary>1.5.5</summary>
    - Fixed an issue that prevented Runeword Sorting feature from functioning unless Standard UI theme was selected<br>
    - Fixed an issue which caused the item icons feature to not function for ReMoDDeD players specifically<br>
    - General code cleanup and comments
</details>
<details>
  <summary>1.5.4</summary>
    - Vault updates are now checked only if launching the Vault, instead of on app startup<br>
    - Vault and D2RLaunch User Data can be accessed in the My Files quick menu<br>
    - Fixed an issue which could cause an update loop to occur<br>
    - Fixed an issue that would sometimes stop UserSettings from being updated or read on app launch
</details>
<details>
  <summary>1.5.2 - 1.5.3</summary>
    - Hotfix for check mod updates feature not checking mod version correctly
</details>
<details>
  <summary>1.5.1</summary>
    - Changed how D2RLaunch chooses -bin or -txt launch settings to better deal with Vault compatibility<br>
    - Fixed an error when translating news for mods who have not added D2RLaunch support
</details>
</details>
<details>
  <summary>Changelog Group (1.0.1 - 1.5.0)</summary>
<details>
  <summary>1.5.0</summary>
    - Re-enabled Vault's Quick Access from menu<br>
    - Fixed an issue which caused an error message with the Super Telekinesis feature<br>
    - Fixed an issue that would cause Personalized Stash tabs to reset after UI theme was applied<br>
    - Updater program now shows download progress
</details>
<details>
  <summary>1.4.9</summary>
    - Added a 3rd option for Assassin Charge Icons: Enabled (No Orbs)<br>
    - Added 2 new QoL Options, "Item Display" and "Super Telekinesis"<br>
    - Font Preview image is now hidden by default and user-toggleable<br>
    - Adjustments made to button coloring for active status<br>
    - Monster Customizations layout and display adjusted<br>
    - Removed old assets and compressed images to reduce launcher size<br>
    - D2RLaunch User Files can now be accessed in the My Files sub-menu
</details>
<details>
  <summary>1.4.8</summary>
    - Added Danish Language option by user request<br>
    - Standardized UI theme across the app and added mouseover effects to all buttons<br>
    - Improved junk character garbage collection for auto-translated news display text<br>
    - Extensive code cleanup for monster customizations feature and a few other loading functions<br>
    - Simplified the Download New Mod panel to make it easier to understand<br>
    - Fixed some English language display issues<br>
    - Fixed an error that would occur when cancelling during the rename character process
</details>
<details>
  <summary>1.4.7</summary>
    - Chinese (zh-CN) translations improved by @demo<br>
    - Many UI adjustments for main program screen and some layout bugs fixed<br>
    - Simplified Mod Update coding for increased reliability/stability<br>
    - Mod update progress will now neatly display download speed, file size and estimated time remaining<br>
    - Adjusted button mouseover effects on main program screen<br>
    - Fixed an issue which could sometimes cause a fatal error if no mods were installed
</details>
<details>
  <summary>1.4.6</summary>
    - D2RLaunch's Language can now be manually selected via dropdown<br>
    - Language translations are now performed in real-time instead of requiring app reload<br>
    - UI Theme adjustments to reduce brightness and adjust news panels<br>
    - Optimized approximately 500 lines of code for improved efficiency<br>
    - Small text adjustments for positioning, colors, etc<br>
    - Fixed an issue that would cause the map layouts feature to sometimes fail
</details>
<details>
  <summary>1.4.5</summary>
    - Languages not yet supported will now fallback to English<br>
    - Restructured the app loading process for improved efficiency and performance<br>
    - Fixed an issue which caused the Map Layouts option to appear blank on startup<br>
    - Fixed an issue that caused game install path to not be saved to user profile correctly<br>
    - Fixed an issue which caused an error on startup if no mods were installed (but used to be)
</details>
<details>
  <summary>1.4.4</summary>
    - Localization Support for the following 13 languages added:<br>
German, English, Spanish (Spain), Spanish (Mexico), French, Italian, Japanese, Korean, Polish, Portugese (Brazil), Russian, Chinese (Simplified), Chinese (Taiwan)<br>
    - Adjusted various UI elements to better suit localization
</details>
<details>
  <summary>1.4.3</summary>
    - Added buttons next to QoL options to enable popup feature preview images and animated gifs
</details>
<details>
  <summary>1.4.2</summary>
    - Altered the Update mod files code for improved reliability and adjusted the progress labels during download<br>
    - When downloading a new mod, it will now read and apply the mod version during the install process<br>
    - The check for updates option will now search for the .mpq folder name to correctly rename github generated folders<br>
    - The Monster Stats Display option is now available to all mods and has been relocated in the options panel<br>
    - Fixed an issue which would cause updating mod files to fail if github failed to report content-length of the file<br>
    - Fixed an issue which caused the download mod option to fail if the same mod was currently selected and also had a custom logo applied
</details>
<details>
  <summary>1.4.1</summary>
    - Users can now view total play time (with launcher open) by clicking the mod logo<br>
    - Fixed an issue which caused the personal stash tabs option to fail after multiple edits
</details>
<details>
  <summary>1.4.0</summary>
    - More UI and font adjustments<br>
    - Code improvements and cleanup
</details>
<details>
  <summary>1.3.9</summary>
    - Discord, Wiki and Patreon links can now change with mod selection<br>
    - D2RModding links for Website, Discord, Youtube added as it's own section at bottom<br>
    - Font standardized across program and smaller UI tweaks for display scaling compatibility<br>
    - Time Estimate for Download Mod function improved and a status message added if content-length is empty<br>
    - Fixed an issue which caused a fatal error when renaming stash tabs with less than 8 enabled<br>
    - Resolved a conflict between UI theme and Personal Stash tabs that would cause an error
</details>
<details>
  <summary>1.3.8</summary>
    - Fixed an issue where the "Install Mod" button would disappear when using a windows display scale setting above 100%
</details>
<details>
  <summary>1.3.7</summary>
    - Added extra folder operations to handle github automatically generated folder names<br>
    - Fixed an issue which caused a fatal error when downloading mod files with an unreported size
</details>
<details>
  <summary>1.3.6</summary>
    - Added functionality for the 'D2RModding Mod Database', which allows quick installs for players with author-submitted mods
</details>
<details>
  <summary>1.3.5</summary>
    - Added the ability to install submitted mods via dropdown selector<br>
    - Adjusted the "Protected MPQ" warning message to be more detailed<br>
    - Fixed an issue that prevented program from closing completely if you also installed a new mod
</details>
<details>
  <summary>1.3.4</summary>
    - Adjusted mod update option to better work with the new download mod feature<br>
    - Fixed loading issues for mods using protected MPQ's<br>
    - Added more public code comments/documentation
</details>
<details>
  <summary>1.3.3</summary>
    - Fixed an issue that caused mod update button from functioning correctly<br>
    - Added support for monster customizations to personal mods<br>
    - Fixed an issue that prevented personalized stash tabs from functioning correctly with personal mods<br>
    - Updated mod download option to show progress info as it installs mod
</details>
<details>
  <summary>1.3.2</summary>
    - Additional improvements made to install detection for alternate Windows User configurations<br>
    - Added feature to create a "blank mod" and use all supported launcher features<br>
    - Added ability to download, extract and install mods through launcher
</details>
<details>
  <summary>1.2.4 - 1.3.1</summary>
    - Improvements to mod detection process and error handling for many reported bugs and issues<br>
    - Fixed error which caused many players to receive a "game install location not found" message<br>
    - Fixed error when opening the launcher caused by the HDR opacity fix<br>
    - Stash Fix and My Save Files buttons now load vanilla characters for mods configured as such<br>
    - Stash UI no longer overwrites other mods who aren't using expanded storage
</details>
<details>
  <summary>1.2.3</summary>
    - Fixed an issue when updating mod files (cleaning task interference)<br>
    - Added a new launcher option to fix HDR opacity<br>
    - Enabled new user settings controls for HDR opacity
</details>
<details>
  <summary>1.2.2</summary>
    - Fixed an issue which caused an error message to appear on first startup if no mods are installed
</details>
<details>
  <summary>1.2.1</summary>
    - Improved download operation and further minimized resources used<br>
    - Fixed an issue with personalized stash tabs resetting after launcher restart<br>
    - Fixed an issue which caused runeword sorting to revert to alphabetical after launcher restart<br>
    - Launcher now updates the version tracking for new installs<br>
    - A popup message is displayed informing you to do a scan/repair with blizzard launcher if no game install location is found
</details>
<details>
  <summary>1.2.0</summary>
    - Relocated web server to resolve the download quota limit caused from excessive updates (Sorry!)
</details>
<details>
  <summary>1.1.9</summary>
    - Attempt to resolve updater 403 errors with API use EDIT: unsuccessful
</details>
<details>
  <summary>1.1.8</summary>
    - Fixed an error when editing stash tabs for mods that did not edit the file yet<br>
    - Fixed an issue when editing only some of the tab names and leaving others default
</details>
<details>
  <summary>1.1.7</summary>
    - Fixed an error with game path in Personalized Stash Tabs option
</details>
<details>
  <summary>1.1.6</summary>
    - Added option for Personalized Stash Tabs<br>
    - User Settings re-structured (Please delete folder in C:/Users/Username/AppData/Local/D2RLaunch)
</details>
<details>
  <summary>1.1.5</summary>
    - Fixed an error when using the modified help menu option
</details>
<details>
  <summary>1.1.4</summary>
    - Added an option to toggle helmet display on characters in-game<br>
    - Fixed an error that occurred if the user's "Mods" folder didn't exist<br>
    - Update button now disabled while user is updating files
</details>
<details>
  <summary>1.1.3</summary>
    - Fixed an issue where mod files download wouldn't start when selecting "Yes" to backup option<br>
    - Fixed an issue with error message when trying to update mod version tracker
</details>
<details>
  <summary>1.1.2</summary>
    - ReMoDDed Beta prep
</details>
<details>
  <summary>1.1.1</summary>
    - Add support for mods that use retail folder location for save files
</details>
<details>
  <summary>1.1.0</summary>
    - Fixed an issue which caused folder cleaning to fail if a custom logo was applied
</details>
<details>
  <summary>1.0.9</summary>
    - Fixed an issue when updating mod and selecting "No" to the backup option<br>
    - Fixed an issue where user settings would be reset during a mod update
</details>
<details>
  <summary>1.0.8</summary>
    - Fixed mod downloader functions broken with 1.0.7 loading changes
</details>
<details>
  <summary>1.0.7</summary>
    - Fixed an issue when launching mods using protected MPQ's<br>
    - Correctly loads user settings for mods with protected MPQ's<br>
    - Automatic Mod Updates now disabled for mods with protected MPQ's<br>
    - Additional Options now inform you of disabled features when using a mod with a protected MPQ<br>
    - Fixed an error which caused Vanilla++ to launch with wrong arguments<br>
    - Code optimizations for mod and user settings loading
</details>
<details>
  <summary>1.0.6</summary>
    - Fixed an issue which caused monster stats display option to read incorrect files
</details>
<details>
  <summary>1.0.5</summary>
    - Fixed an issue which caused loot drop setting to not correctly apply edits (Customizations)
</details>
<details>
  <summary>1.0.4</summary>
    - Removed excessive popup messages from auto backups success (debug message)<br>
Added ability for launcher to track your total playtime (Click logo to display, will move later)
</details>
<details>
  <summary>1.0.3</summary>
    - Fixed an issue which sometimes caused download to appear in the wrong folder
</details>
<details>
  <summary>1.0.2</summary>
    - Fixed an issue which caused show item levels to not save to user settings correctly<br>
    - Added 'Download Only' options for new players to ReMoDDeD/Vanilla++
</details>
<details>
  <summary>1.0.1</summary>
    - Fixed error when selecting rune display option multiple times<br>
    - Fixed error when selecting the no background option for monster stats display<br>
    - Fixed issue with playing mods with special characters in the modname
</details>
<details>
  <summary>1.0.0</summary>
    - Initial Release
</details>
</details>