# ZA Shiny Warper
For warping or shiny hunting with your character in Pokemon Legends: Z-A.

Requires a hacked Switch console running either [sys-botbase](https://github.com/olliz0r/sys-botbase/releases) or [usb-botbase](https://github.com/Koi-3088/USB-Botbase/releases).

![Main window](ZAShinyWarper_Window.png?raw=true "The program itself")
![Webhook window](ZAShinyWarper_Webhook.png?raw=true "Webhook Settings")
![Success window](ZAShinyWarper_Success.png?raw=true "Finding a shiny")

## Shiny Hunting Setup
1) Ensure you have sys-botbase or an equivalent installed on your Switch running atmosphere, then create a save backup using something like JKSV.
2) Open the program and connect to your console. You'll see all your "stashed" shinies.
3) Find one or more nearby Pokemon spawn locations, for each one press "Set Pos", move away from it a little, then press "Restore Pos" to verify that it is a stable position.
4) Set a final position nearby that is considered "safe" with little to no spawns, where you can easily save, and do the same. You want to be far enough away that the Pokémon has despawned.
5) Optionally set any filters for the shiny Pokemon you want.
6) Press "Begin Warping"

While warping, the camera in game will rotate as you warp between all of your locations, then save to retain your current "shiny stash". If a Shiny was found in your cache matching your selected filters, the game will pausethe game and show a message if a shiny is found. You must go and catch the shiny yourself, this tool cannot do that.
If a shiny is not found, it will automatically loop again until one is found or you press the button to stop the shiny hunting routine.

All shiny Pokemon from your stash are saved in their incomplete "wild" format in the StashedShinies folder.

## Features & Enhancements

### Shiny Detection & Management
- **Automatic Shiny Detection** - Continuously monitors and identifies shiny Pokémon in the game
- **Stashed Shiny Display System** - Disaplys all shinies currently found in your cache
  - View detailed information on hover of each shiny in your stash
  - Right-click individual entries to remove from stash: "Clear from Stash"
  - Clear entire cache with "Clear ALL from stash" option
  - Export stashed sets on demand
- **Smart Cache Management** - Multiple action modes when shinies are found:
  - **Stop on Full Cache** - Stops hunting when 10 shinies are found  regardless of filter settings, allowing you to review and decide you next steps
  - **Clear Cache and Continue** - Clears all non-filter matches from the cache as they're found, keeping only filter matches until cache is full

### Filtering System
- **Species Filtering** - Target specific Pokémon or choose `Any` to cache all found shiny Pokémon
- **Alpha Filtering** - Filter for Alpha Pokémon (all Alphas are 255, but not all 255s are Alphas)
- **IV Filtering** - Fiter based on 3 values: Any, Perfect, and Zero
- **Size Filtering** - Search for specific size ranges based on miminum and maximum scale
- **Reset Filters Button** - Quickly restore all filters to default settings

### Discord Webhook Integration
- **Multiple Webhook Support** - Configure and use multiple Discord webhooks simultaneously
- **Embed Messages** - Sends detailed shiny information including:
  - Pokémon sprite images
  - Species name and form
  - Stats and characteristics
- **Webhook Settings Panel** - Dedicated UI for managing webhook configurations
- **Test Button** - Verify webhook connectivity before hunting. Select any webhook and click the test button

### Coordinate Management
- **Position List Management** - Create and organize multiple warp locations
- **Import/Export System** - Share coordinate sets with other users
  - Option to overwrite existing coordinates
  - Option to to bottom of the current coordinate list
- **List Organization Tools**
  - Shift coordinate entries up/down in the list using the dedicated side button
- **Right-click Menu**
  - Copy and paste coordinates directly from the list.
  - Inset pasted coordinate either above or below currently select 
  - Quicker delete abilty by automatically selecting the next coordinates in your list

### Environment Control
- **Weather Control** - Set and maintain specific weather conditions
  - Checked every minute to ensure stability
- **Time of Day Control** - Lock time to prevent day/night cycling
  - Checked every 5 minutes for consistency
- *(Credit to Kunogi for discovering these pointers)*

### Configuration & User Experience
- **Persistent Settings** - All program configurations are saved to `config.json`
- **Legacy Support** - Automatically imports from old `.txt` config files on first run when there is no existing `config.json`
- **Warp Progress Window** - Always display centered with main window informing of warp progress

## Known Issues
* Sometimes your character goes flying into the air, the bot will fix that itself.
* If one of your positions are in a battle zone, the bot will eventually show you an error that it was unable to warp.

## Other Use-Cases
* Warping to places nearby.
* Falling off or warping back on top of buildings multiple times.
* Warping into the geometry and [falling through the map to your inevitable doom.](https://x.com/berichandev/status/1980471677659279623) This always happens when warping too far away due to the collision LODing.

## Video Guide
Original Rundown video: https://youtu.be/eKydGGQbS_0 

## Credits & Attributions

**Original Creator Credits:**
- Leverages PKHeX nuget and sys-botbase interfacing built upon [NHSE.Injection](https://github.com/kwsch/NHSE) by [Kurt](https://github.com/kwsch)
- Thanks to Anubis for [this research tweet](https://x.com/Sibuna_Switch/status/1980306261213393163) that gave a starting point
- Thanks to Olliz0r, Koi, and FishGuy for all the interfacing tools
- Thanks to Berichan for creating this amazing program

**Additional Contributions:**
- **Special Thanks to Kunogi** for discovering and documenting the 3 main pointers allowing so much control of the game while hunting!

---

*This bot is designed for use with Pokémon Legends: Z-A for research and quality-of-life purposes.*