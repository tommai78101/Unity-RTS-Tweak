# Unity-RTS-Tweak

### Update

This project is to be deprecated due to Unity added a deprecation model on the old Networking codes. In its place, I have created a new project, [Multiplier](https://github.com/tommai78101/Multiplier), to continue with development, as well as to stay up-to-date with Unity 5.2.

**TL;DR:** Development for this project is indefinitely set to **discontinued**. Please migrate to [Multiplier](https://github.com/tommai78101/Multiplier), where the project is continuing.

------

### Abstract

Using Unity to develop a real-time strategy game involving tweaking the game balances to reduce complexity and flatten the learning curve associated with it.

Link to an online playable demo:  [http://tom-mai78101.itch.io/multiplier](http://tom-mai78101.itch.io/multiplier)

------

### Disclaimer

Note that there's a lot of kinks and quirks that I haven't been able to sort out. More importantly as I am a novice in Unity game development starting out just a month ago, I am still learning Unity, so please understand. 

Since this prototype is all about network multiplayer RTS, I intentionally designed it so that no two players are using the same computer. If you are playing alone, you can open two webpages together, and don't worry about Player 2. Please continue in the Start section below.

-------

### Introduction

This game is my first Unity network multiplayer game prototype, and my first Unity game in my lifetime. I am currently testing out ideas on what I can do to bring down the complexity that all real time strategy games suffer from. By bringing down this complexity, I can flatten the learning curve, as well as get new players to play more real time strategy games. 

This real time strategy game, Multiplier, is based on the math equation, y=2x. In other words, there's only cloning and kill. You create clones, you merge, and you kill the other players. 

I have no idea if it's possible to host server games that are hosted online. Let me know anything. 

---------

### Start

You may play against yourself, or have someone to join you on another computer. If you are playing with yourself, you must also do the Player 2 instructions. 

Player 1 is the host of the game. Player 2 is the client of the game. 

**Player 1 instructions (Host):**

1.Press "Host New Server".    
2.Once you are in the game, you must wait until Player 2 joins the game.    

**Player 2 instructions (Client):**

1.When you know the host is currently hosting a new server, press "Find Servers".    
2.Wait for 1 second.    
3.There should be at least 1 server found. If not, then it means your computer could not locate the host server.    
4.Click on the server titled, "Unity RTS Prototype".    

Once both players are in the game, both players will start off with 1 unit each. Please continue to read Controls section for how to play. 

-------

### Controls

* Left Mouse Button - Select units. (You can only select your own units.) Hold down button and drag to make selection box.
* Right Mouse Button - Move to Location.
* A - Attack where? (Right click to choose location)
* S - Split. Split into two units. Note that there's a cooldown of 10 seconds.
* D - Merge. Merge any two units together.
* ` (Tilde) - Bring up the console. (Only when you're hosting or you're in the game)

-------

### Changelog  

**v0.04:**

* Added ability for units to heal over time.

**v0.03:**
 
* Fixed splitting units creating glitchy split animation on the client side.
* Streamlined merging and splitting for each units.

**v0.02:**
 
* Fixed merging may go out of sync. May introduce new bugs. 
* Fixed scaling issues related to scaling exponentially and not incrementally. 
* Tweaked selection so that players can now select units without any hassles. 

**v0.01b:**

* Fixed being able to merge with other units of different levels.
* Fixed a few bugs.
* Added prototype number. Numbering scheme is as follows: ◦[Game Build Type] - v[Big Release].[Small Release][Minor Fixes]

**v0.01a:**

* Fixed issue with inconsistent dividing behavior.

**v0.01-minus:**

* Reverted back to previous version due to some changes causing inconsistent behaviors. 

**v0.01:**

* Fixed issue such that the player cannot see if their units are taking damage.
* Decrease max health point.
* Decrease spawning time

--------

### Credits

**Creator:** Thompson Lee 

