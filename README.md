
# Development Process

## Approach

1. **Overview**:  
Due to the nature of the role to be filled, I approached this assignment from a global architecture point of view more than programming raw mechanics (like the combat look and feel), which in my opinion is the easiest part to polish. I focused on data structures,  scaling, modularity and re-usability. I implemented patterns and systems that can be used by several entities (Addressable-ready poolings, Combat & Movement handlers, State machines, etc.). The idea is to have solid encapsulated systems independent enough to be heavily shared between entities but flexible to be adapted for every occasion.

2. **Planning & Development**: 
Unlike other games where I have to test whether the core mechanics are fun or not, I have prototyped this concept ‘from the outside in’, i.e. taking into account what services and systems an RPG requires, and leaving the more ‘front end’ mechanics for the end. For example, variety of entities (Heroes, Enemies, Npcs, Items, Weapons, skills...) and the need for these to be organised in an understandable data structure.



## Time Allocation

| Phase              | Time Spent (Hours) 
|---------------------|--------------------|
| Planning            | 1 hours        
| Core Development    | 9 hours       
| Bug Fixing  | 1 hour        

- I may exceed a little bit the 10h limit because of having to hand-write some systems that normally I'd just import from my library (Poolings, common handlers...). It'd take me around 1h to polish the front combat feeling and 30min to polish the selection panel, which right now it's very raw. Feel free to ask me for an improvement and I'll make the polished version of both.

## Challenges Faced
- **Scalability vs Simplicity**:  
   - Sometimes it is difficult to get the balance right, especially when you don't know the real scoop of the project or the specific requirements.


## Areas for Improvement
- **GUI / Hero And Weapon selection panel**:  
   - The selection panel was the last thing I made, and for time reasons I rushed the bare minimum to demonstrate that we can list and filter the data in order to retrieve what we need (Heroes, Weapons, etc.) and be able to test different setups. Since it wasn't stated how the game loop would be, I just made an easy picker, almost a debug tool.
- **Combat Look & Feel (VFX, animations, audio, etc)** 
   - As I said, I didn't focus on this part (even if it's one of the most visible) because it's pretty easy to polish with little time.

## Further steps
- **Data persistence**
    - Since it wasn't stated that levels or any kind of persistence should be included I skipped that part, but the idea would be to mirror de ModelData (which is static, handled by ScriptableObjects) as RuntimeData according to the persistency needs, for example,  "WeaponRuntimeData" would be a  Serialized class that handle if the weapon have been upgraded (Runes, Levels, Enchantments), linked through the ID with its Model.
    - We could handle the runtime data through Binaries or JSON (I prefer binaries), mixing local data with server validations, or, if security is a must, handling the whole runtime data server-side.
- **Gameplay stuff**
    - Buffs & Debuffs, Habilities, Weapon effects, class perks,  leveling, Game Objectives, Cool gameloops, etc.
- **Scriptables / Data Structure tools**:  
    - Something I like about Scriptables is the synergy with editor tools to visualize, handle and organize them.



## Comments and FAQ
- **Enemy and Hero Detection**: 
    - I could have handled this interaction in several ways, but I used this unusal approach for performance reasons thinking in an open scenario with lots of enemies that stay in their place (instead of chasing from the beggining).
    - The idea is to have a single, large collider that notifies both hero and enemies of each other's existence when they are nearby. This avoids loops of checks and the need to have many colliders and physics checks.
    - This approach is incompatible with the use of friendly npcs and hinders multiplayer, but it's suitable for specific cases.
- **Why are there columns on the map?**:
    - I though that it would be unrealistic to have a game scenario without obstacles so I put some pillars to force myself into using NavMesh and obstacle avoiding.
- **Why is there a second hero with different weapons?**:
    - I wanted to show up the diversity of entities and data handling. Also magic is fun! 
- **About naming**
    - I used a simple naming convention (PascalCase and camelCase) just for clarity.
    - Some classes (CombatHandler, etc...) and others named x**Handler** are basically non-monobehaviour **controllers**. It's just an habit but I have no problem in switching to the conventional naming!
