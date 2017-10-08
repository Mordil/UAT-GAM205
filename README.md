# UAT-GAM205
This was a 5-week long project for the [University of Advancing Technology's](http://www.uat.edu/) "Gameplay Programming Concepts" (GAM205) course.

This game is a split-screen multiplayer 3D game that randomly spawns players in a dynamically ("procedually") generated level with a set number of lives with the goal of getting the highest score. Various enemy behaviours are programmed for the player(s) to deal with, and once all enemies are killed - the game is won with a high score displayed.

Each week added new features and marked the project's overall weekly milestone.

> :exclamation: As part of my [UAT GPE-338 "Advanced Gameplay Programming"](https://github.com/mordil/uat-gpe338) course, I refactored several sections of this game to use concepts taught in the course. These changes can be seen on the [`#refactor-tanks`](https://github.com/mordil/uat-gam205/tree/refactor-tanks) branch.

## Course Description

This course was modeled after Unity's [Tank! Tutorial](https://unity3d.com/learn/tutorials/projects/tanks-tutorial), with additional features such as AI + Pathfinding, split-screen multiplayer, and more.

> Gameplay Programming Concepts teaches students the most important theories and concepts in game programming. Students will be exposed to major game mechanic fundamentals that are expressed in multiple games across several genres. There is a strong focus on analysis and discovery learning. Those taking this class will be expected to observe existing mechanics and attempt to reproduce them both in documentation and in code. The course makes use of an existing game engine in order to focus on gameplay mechanics with the end goal of producing a playable game.

*Note: While Unity does support many systems to handle these things and make development more streamlined and/or "simplier", the point of the course was to teach the underlying concepts and to give us experience in writing them from scratch.*

## Course Outline

Throughout the entire course, emphasis was placed on thinking and developing the "[Unity way](https://en.wikipedia.org/wiki/Entity%E2%80%93component%E2%80%93system)" as well as working as if we were on a team by keeping in mind how others may use our components. Grading included how well the inspector was organized, as well as file structure and coding style.

* Week 1
  * Player movement with Unity's [Character Controller](https://docs.unity3d.com/ScriptReference/CharacterController.html) component
  * Basic level environment for a player to navigate through and collide with
  * Shooting mechanic
  * Bullet physics
  * Basic AI that shoots at a regular interval
  * Basic score keeping
  * [Game Manager](https://unity3d.com/learn/tutorials/projects/tanks-tutorial/game-managers) shell
* Week 2
  * Advanced AI with a custom script-driven [State Machine](https://en.wikipedia.org/wiki/Finite-state_machine)
    * The basis of each tank is one that will patrol between different points assigned to it and will "listen" for the player to be within a certain range.
      * Tanks can patrol in one of three ways (defined at design time):
        * `NoRepeat` - Go through the array of points once and stops
          * e.g. 1 -> 2 -> 3 -> |
        * `Sequence` - Goes through the list before reversing the list.
          * e.g. 1 -> 2 -> 3 -> 2 -> 1...
        * `Loop` - Goes through the list before restarting.
          * e.g. 1 -> 2 -> 3 -> 1 -> 2...
      * If the player comes within range, it will pause patrolling and turn to where the player was when they "heard" them.
      * If the player comes within line of sight, the tank will start to chase the player.
      * Once the player gets too far out of range for a set time, the tank will revert back to patrolling.
      * If the tank takes any damage, it will flee from the player in the opposite direction for a set amount of time.
    * 5 different personalities:
      * `Standard` - Behaves as defined above
      * `Aggressive` - Larger model scale, that chases the player until killed
      * `Stationary` - Does not patrol, does not flee, has a different color from other tanks
      * `FrenchTank`: Smaller model scale, moves faster, flees from players twice as far and twice as long, heals while fleeing
        * I appologize for the insensitive naming to French people, as I was looking for a shorthand colloquial term for naming my object and concept - and I have a rude stereotypical American sense of humor regarding the French military.
      * `Invisible` - Invisible tank aside from VFX from movement
    * Simple pathfinding if path is blocked
* Week 3
  * Map generation using preconfigured tiles that are randomly selected and arranged at runtime
  * Support for a "level of the day" which uses the day + month + year as the randomizer's seed
  * Each section has enemy & player spawners
  * Powerups & pickups
    * Must have respawn timers
    * Enemies should be able to pick them up as well
    * Must have at least 1 expire over time
    * Must have at least 1 that is a permanent change to tank settings
  * Game Manager spawns the player at a random player spawn on a tile each time the player dies.
* Week 4
  * Level transitions between a main menu & gameplay
  * Start screen, win/lose screen, and Settings
    * Start Screen
      * New Game, Settings, Quit
    * Win/lose
      * Play Again, Main Menu
      * Displays all player's scores
      * Displays the highest all-time score that is persisted with a save file
    * Settings
      * Audio channel volume
      * Number of players
      * Number of lives per player
  * In-game UI for each player's health & lives remaining
  * Multiplayer support for 2-4 players
  * Audio for all SFX & background music
    * Main menu, gameplay music
    * SFX
      * Death
      * Shooting
      * Hitting
      * Unique pickup SFX
      * Menu button clicks
* Week 5
  * Bug fixes
  * Final polish with new assets

## Features
* [Unity Tanks! Tutorial](https://www.assetstore.unity3d.com/en/?_ga=1.132218824.773429367.1482361617#!/content/46209/) animations & models
* [World of Tanks](http://worldoftanks.com/en/media/) wallpapers and music
* Unity Sytems:
  * [Particle Systems](https://docs.unity3d.com/ScriptReference/ParticleSystem.html)
  * [UI](https://docs.unity3d.com/Manual/UISystem.html)
  * [Character Controller](https://docs.unity3d.com/ScriptReference/CharacterController.html) component

## License

All sourced assets are under their respective licenses, and the following only applies to my code.

----

MIT License

Copyright (c) 2016 Nathan "Mordil" Harris

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
