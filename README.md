# !romhack - Streamer.Bot based Overlay Management for Super Mario World Romhack Players

This is an FOSS (Free and Open-Source Software) setup for [Streamer.Bot](https://streamer.bot/) that allows you to automate your OBS overlay, provide the ability to search and link romhacks, receive suggestions, and keep track of your progress.

## Features
- Automate your OBS overlay
- Search and link SMW Central from your chat
- Receive suggestions
- Keep track of your progress
- Permissions system to ensure only you and your mods can make updates
- Only manages two files: a history of the hacks you've beaten (including when you did so) and a markdown table of which games people requested you to play


## Prerequisits
Streamer.Bot with OBS Websocket enabled

## Installation

1. Download [Streamer.Bot](https://streamer.bot/downloads)
2. Follow the [Streamer.Bot Quick-Start](https://wiki.streamer.bot/en/Quick-Start) guide if you haven't used bot before.
3. Copy the Import Code and import the command into the bot.
4. Add a valid `path` to the the Action `Chat - !romhack`
4. Enable the `!romhack` command, add `Hot Keys` to `Exit++` and `Exit--`
5. Run `!romhack setup` while being offline.

### Import Code 
[See Releases](https://github.com/synthie-cat/-romhack/releases/tag/V1)

## Usage

| Command 	                       	| Description 								| Streamer| Mods | Viewers |
|--------------------------------------	|-------------								|---|---|---|
| `setup` 	                       	| Creates the "[O]Romhack Info" overlay scene that will be updated       | ✓ |   |   |
| `update [ROM Hack Name]` 	       	| Updates the overlay to the specified ROM Hack.            		| ✓ | ✓ |   |
| `restore`	                       	| Reverts the overlay to the previous hack.            			| ✓ | ✓ |   |
| `manual [Name, Creator, Type, Exits]`	| Takes a list of comma seperated values to manually generate an overlay| ✓ | ✓ |   |
| `search [ROM Hack Name]`		| Searches SMW Central for a ROM Hack and posts it to chat            	| ✓ | ✓ | ✓ |
| `suggest [ROM Hack Name]` 		| Add a ROM Hack to the suggestions list				| ✓ | ✓ | ✓ |      


## Contributions

I gladly welcome contributions and feature requests for this setup. Feel free to open an Issue, create a pull request or message me on [Discord](https://discord.gg/bra4apXCh7). 

**A HUGE SHOUTOUT** to [EvilAdmiralKivi](https://twitch.tv/eviladmiralkivi) who wrote the parser and without whom this would not exist.

## Upcoming features
- Auto Download of game covers
- Parse by ID
- Search for Tools, ASM, Music
- RHR parsing
- ~~Manual updating (for Romhack Testing)~~

## License

This project is licensed under the [MIT License](LICENSE).
