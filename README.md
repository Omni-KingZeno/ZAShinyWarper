# ZA Shiny Warper
For warping or shiny hunting with your character in Pokemon Legends: Z-A.

Requires a hacked Switch console running either [sys-botbase](https://github.com/olliz0r/sys-botbase/releases) or [usb-botbase](https://github.com/Koi-3088/USB-Botbase/releases).

![Main window](ZAShinyWarper_Window.png?raw=true "The program itself")
![Webhook window](ZAShinyWarper_Webhook.png?raw=true "Webhook Settings")
![Success window](ZAShinyWarper_Success.png?raw=true "Finding a shiny")

### Shiny hunting setup
1) Ensure you have sys-botbase or an equivalent installed on your Switch running atmosphere, then create a save backup using something like JKSV.
2) Open the program and connect to your console. You'll see all your "stashed" shinies.
3) Find one or more nearby Pokemon spawn locations, for each one press "Set Pos", move away from it a little, then press "Restore Pos" to verify that it is a stable position.
4) Set a final position nearby that is considered "safe" with little to no spawns, where you can easily save, and do the same.
5) Optionally set any filters for the shiny Pokemon you want. I recommend keeping everything unchanged for now while the software is new.
6) Press "Begin Warping"

The program will now rotate the camera and warp between all your locations 10 times, then save to populate the "shiny stash". It will then read from the shiny stash and pause the game and show a message if a shiny is found. You must go and catch the shiny yourself, this tool cannot do that.
If a shiny is not found, it will automatically loop again until one is found or you press the button to stop the shiny hunting routine.

All shiny Pokemon from your stash are saved in their incomplete "wild" format in the StashedShinies folder.

### Known issues
* The memory pointers I found for this project are reliable, but there are more ideal ones present, especially for shiny stashing which does not require saving the game, more research is required for finding this.
* Sometimes your character goes flying into the air, the bot will fix that itself.
* If one of your positions are in a battle zone, the bot will eventually show you an error that it was unable to warp.
* The code is ugly.
* The tool saves non-complete Pokemon data as PK9 files instead of the Z-A filetype until PKHeX is released to support the game.

### Other use-cases
* Warping to places nearby.
* Falling off or warping back on top of buildings multiple times.
* Warping into the geometry and [falling through the map to your inevitable doom.](https://x.com/berichandev/status/1980471677659279623) This always happens when warping too far away due to the collision LODing.

Rundown video: https://youtu.be/eKydGGQbS_0 

# Attributions

Leverages PKHeX nuget and sys-botbase interfacing built upon [NHSE.Injection](https://github.com/kwsch/NHSE) by [Kurt](https://github.com/kwsch).
Thanks to Anubis for [this research tweet](https://x.com/Sibuna_Switch/status/1980306261213393163) that gave me a starting point.
Thanks to Olliz0r, Koi, and FishGuy for all the interfacing tools.