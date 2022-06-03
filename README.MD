# READ-ME

## Overview
This project originated from a pathfinding assignment set by Forsbergs Skola. The goal was to come up with a unique problem and solve it using an appropriate algorithm of my choosing. The original rep for it can be found on the [school's github here.](https://github.com/forsbergsskola-se/gp21-22-0209-algodata-HarryTheHeath)

Due to my love of games that feature communities full of characters who seemingly follow organic routines as days pass I decided upon creating a simple village of characters to simulate a simplified version of this back-and-forth, point-to-point pathfinding style. In my game, six slimes alternate between their home and six Japanese-inspired shrines, with these combinations varying uniquely every day. 

Why slimes? Because they're simple to animate and cute.

## The Game
A demo build is [currently available on Itch.io.](https://play.unity.com/mg/other/webgl-builds-170014)

Left-click when slimes aren't moving to assign new paths. Right-click anywhere, except shrines and houses, to place boulders.

![Winter](https://user-images.githubusercontent.com/90158105/160833871-c6335f09-6941-497b-8c47-213ac437502e.png "Winter Gameplay Still")

## My pathfinding problem
The problem then was to have each of the six slimes reliably calculate the shortest path it must take to reach the shines from its house and vise-versa. In addition to this, I made it so that the player can place boulders to obstruct their paths, even when they're in motion. If the player does so the slimes recalculate their pathfinding on the spot and immediately adapt, again finding the new shortest paths that they must take. These live calculations are visualised by debug draw-lines that match the colour of the slime that they're representing. 

Further path-checks are also calculated behind-the-scenes to check if a boulder placement will make any of the key paths impossible to make (e.g. by surrounding a house). If this is the case, the boulder placement is blocked and a visual indicator instead warns the player that it was an invalid action. 

![SlimeVillage](https://user-images.githubusercontent.com/90158105/160854968-7669a987-59d1-489a-b8c9-2889b6615143.gif "Placing obstacles")

## Chosen algorithm
I chose to use the A* search algorithm as my pathfinding solution due to its efficiency. By using it I was able to calculate very optimised paths, that made use of diagonal route-taking possibilities in a 2D world-space and allow the player to place lots of boulders to impede the slimes without their solutions appearing clumsy, as they might with Dijsktra and other algorithms that A* built upon. The only main drawback to A* is its relative complexity, but it wasn't a major concern for my small-scale problem.

## Borrowed code
When creating the data structure on which the rest of the game was built upon I used Code Monkey's YouTube tutorials on making a [Grid System](https://youtu.be/waEsGu--9P8) and implementing [A* Pathfinding](https://youtu.be/alU04hvz6L4). I wrote down documentation as I went along to show my understanding of the borrowed code and immediately began refactoring and reappropriating his scripts, as well as creating my own. 

I spent at least a month working on the project since finishing the tutorials to get to the point I am today (March 27th) and have made so many alterations since that the code has significantly evolved from these early days, rather than staying untouched. Code Monkey also made use of unexplained packages in his videos for parts of his code, which I refrained from using, and instead devised new solutions for. This includes the key gameplay areas of character pathfinding and placing objects on the grid, such as boulders. 

Getting the pathfinding to work properly with multiple characters proved to be a headache for a while, in particular, mainly due to the many live calculations that needed to be made when boulders are placed.

## Future of the project
I'm currently working on adding day-and-night driven systems, scheduled pathfinding using in-game time, a playable character, and scaling up the grid from its humble 10x20 size. I also want to make a mobile build of the game later. More generally, I see this as a system that I can reutilise for projects in the future that require pathfinding.


