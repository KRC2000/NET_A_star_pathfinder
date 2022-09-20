# A* algorithm pathfinder NET implementation
## What is this?
This is a simple and fast little framework for finding shortest path from point A to point B on the 2d grid.
If you are building strategy game, you might want to give orders to your units where to go. Or maybe you want zoombies chase the player. 
Well, this framework got you covered.
## Is it bloated?
No, there are 3 versions, all of them are using `dotnet framework3.1` and only one uses additional dependency - `MonoGame.Framework.DesktopGL`.
## How to use it?
Currently there are 3 folders in the repository:
* MadeClean (Just dotnet framework3.1, nothing else. Returns a path as a list of uint, where X and Y coordinates of solving nodes are packed in the row)
* MadeForMonogame (dotnet framework3.1 and MonoGame, particulary Point class. Returns a path as a list of Point)
* MadeCustom (dotnet framework3.1 and custom class Vector2i. Returns a path as a list of Vector2i)

1. If you want to use pathfinder in your Monogame project - get `MadeForMonogame`. Else - get `MadeClean`. There is no reason getting `MadeCustom`.
2. Copy all `.cs` files in your project's folder.
3. Add `using Framework.Pathfinder;`
4. Create an instance of Pathfinder: `Pathfinder finder = new Pathfinder();`
5. Initialise pathfinder by feeding him with the map grid: 
```
// Map grid where 0 - walkable, any other number - obstacle.
uint[,] map = new uint[,]{
  {0, 1, 0, 0, 0},
  {0, 1, 0, 1, 0},
  {0, 1, 0, 1, 0},
  {0, 1, 0, 1, 0},
  {0, 0, 0, 1, 0}
};

// Pathfinder initialisation
finder.Init(map); 
```
6. And finally get the path from A to B by calling `GetPath`:
```
// If you using MadeClean:
List<uint> path = new List<uint>(); // Create list to store final path coordinates
finder.GetPath(out path, 0, 0, 4, 4); // Fill just created list with path coordinates. First two numbers - X and Y position of the start cell(0, 0), next two - X and Y of the final target cell(4, 4)

//if you using MadeForMonogame
List<Point> path = new List<Point>(); // Create list to store final path coordinates
finder.GetPath(out path, new Point(0, 0), new Point(4, 4)); // Fill just created list with path coordinates. First Point - position of the start cell(0, 0), next one - X and Y of the final target cell(4, 4)
```
Now `path` will be filled with either `{ 0, 0, 0, 1, 0, 2, 0, 3, 1, 4, 2,  3, 2,  2, 2,  1, 3, 0, 4, 1, 4, 2, 4, 3, 4, 4 }` if you are using MadeClean, or it's gonna be Point objects that can be represented as so:
```
{0, 0}
{0, 1}
{0, 2}
{0, 3}
{1, 4}
{2, 3}
{2, 2}
{2, 1}
{3, 0}
{4, 1}
{4, 2}
{4, 3}
{4, 4}
```
As you can see, it do be solving path:
![pathfinfing_demo](https://user-images.githubusercontent.com/47914319/161387616-47344ad2-b089-451d-a51c-88c9a08c77f5.png)
You can also choose modes like so: `finder.Mode = PathMode.Aligned;`
By default the Mode is set to `PathMode.Free`, which means that path can be calculated using diagonal movement. If it is set to `PathMode.Aligned`, path would look like so:
![pathfinfing_demo_short](https://user-images.githubusercontent.com/47914319/161387899-4a1deae5-682e-4fa2-9ef1-bd7d089fc12f.png)

