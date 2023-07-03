# FinalProjectSocketsUnity
 
A 3D Unity game heavily based on the hit game "Fall Guys", the multiplayer is implemented using sockets and SQLite database, and the communication is server-side.

Quick and short explanation:

ServerClient - The client side of the game, implemented in Unity.
LobbyServer1 - The Server side of the game (logging in, signing up, entering/creating lobbies, hopping into games and etc.), implemented in c#.
UnityGameServer -  a single instance of a game, implemented in Unity (this is where all the calculations of a single multiplayer match happen: collisions, movement, keeping score, etc.)


