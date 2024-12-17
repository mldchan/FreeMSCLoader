[![](https://img.shields.io/github/release/piotrulos/MSCModLoader.svg?style=flat-square)](#) 
[![](https://img.shields.io/github/downloads/piotrulos/MSCModLoader/total.svg?style=flat-square)](#) 
[![](https://img.shields.io/github/license/piotrulos/MSCModLoader.svg?style=flat-square)](#) 

# FreeMSCLoader
A mod loader for My Summer Car but it actually follows its own license. 

[You can view the one that breaks its own license here.](https://github.com/piotrulos/MSCModLoader)

## How does it break the license?

- **Any project licensed under GPL should be compilable with hopefully all libraries findable on the internet**. MSCLoader does require the game to be installed on the user's system and the libraries linked from that, but what really breaks the license is that there are references to `MSCLoaderCore` which there's no information about. Google only returned the usages of this.
- **The project communicates with a web server, which you don't have any access to, is closed-source and you don't know what it's doing**. On top of that, all of the communication between the server and your MSC client is <u>**unencrypted**</u>, going over <u>**HTTP**</u>. <u>**NOT HTTPS!**</u> **Your literal MSCLoader token is transferred unencrypted**, making it **susceptible for a man-in-the-middle attack**. If you interact with publishing mods, your token has the possibility of getting into somebody else's hands, and **this could be really bad**. <u>**The hacker could go ahead and replace your mods that you have published on MSCLoader with viruses and this will quickly spread**</u>. HTTPS certificates are literally free nowadays.

This project fixes these problems, by nuking any interaction with the server whatsoever.

## FAQ

- **Q: How do I get mod updates?**: NexusMods helps you tell apart mods that have gotten updates by adding a visible orange badge saying "Update Available". By downloading the new version and replacing it in the mods folder, you update the mod. Mod updates aren't that frequent for you to have to update every 10 minutes.

## Reporting issues
Please report any issues and ideas [here](https://github.com/piotrulos/MSCModLoader/issues). This project is afterall a soft fork, meaning that I will pull changes in here.

**Example Mods** - this folder contains example mods source files (see documentation how to use them)  
**Visual Studio templates** - this folder contains templates for Visual Studio for easy mod creation. (see documentation how to use them)  
## Documentation
A documentation for MSCLoader is avaliable [here](https://github.com/piotrulos/MSCModLoader/wiki) (for modders and users)  

## Installation
1. Clone this project
2. Build it from source (Unlike regular MSCLoader, can be built without needing MSCLoaderCore!)
3. Install regular MSCLoader
4. Copy MSCLoader.dll from Release folder to 
## License
MSCLoader is licensed under **GNU General Public License v3.0 (GPL v3)**   
If you want to port this to other games make sure to link original reposity in your credits and keep it under same license (open-source).

## Used Libriaries
* [NAudio](https://github.com/naudio/NAudio)/[NVorbis](https://github.com/ioctlLR/NVorbis) - **MIT License (MIT)**    
* [Ionic.Zip](https://archive.codeplex.com/?p=dotnetzip) (DotNetZip) - **Microsoft Public License (Ms-PL)**   
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) - **MIT License (MIT)**    
* [UnityDoorstop](https://github.com/NeighTools/UnityDoorstop) - **CC0 (Public Domain)**    
* [INIFileParser](https://github.com/rickyah/ini-parser) - **MIT License (MIT)**    

## Like My Work?
If you want you can buy the original developer a beer :)
Paypal: [Paypal.me](https://www.paypal.me/piotrulos/0eur)  
BTC: 1NLRvUXHLhuLz5erVWyXdY7i8KmfCSjJgP
Thanks for all donations, even small ammount helps.

If you want to buy **ME** a coffee, go ahead and do that [here](https://mldchan.dev/donate)

#### Credit
* **djoe45** for MSCLoader v0.1 that was based on **Longwelwind** and **TheNoob454** work ([PhiPatcher](https://github.com/Longwelwind/PhiScript) and [PBLoader](https://github.com/TheNoob454/PBLoader))    
* All contributors to this project.