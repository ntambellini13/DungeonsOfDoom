using DungeonsOfDoom.Core;
using DungeonsOfDoom.Core.Characters;
using DungeonsOfDoom.Core.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DungeonsOfDoom
{
    class ConsoleGame
    {
        Player player;
        Room[,] world;

        public void Play()
        {
            CreatePlayer();
            CreateWorld();

            do
            {
                Console.Clear();
                DisplayWorld();
                DisplayStats();
                AskForMovement();
            } while (player.IsAlive && Monster.MonsterCount > 0);

            GameOver();
        }

        private void CreatePlayer()
        {
            player = new Player(100, 0, 0);
        }

        private void CreateWorld()
        {
            world = new Room[20, 5];
            for (int y = 0; y < world.GetLength(1); y++)
            {
                for (int x = 0; x < world.GetLength(0); x++)
                {
                    world[x, y] = new Room();

                    int percentage = RandomUtils.Percentage();
                    if (percentage < 5)
                        world[x, y].Monster = new Ogre();
                    else if (percentage < 10)
                        world[x, y].Monster = new Skeleton();
                    else if (percentage < 15)
                        world[x, y].Item = new Potion();
                    else if (percentage < 20)
                        world[x, y].Item = new Sword();
                }
            }
        }

        private void DisplayWorld()
        {
            for (int y = 0; y < world.GetLength(1); y++)
            {
                for (int x = 0; x < world.GetLength(0); x++)
                {
                    Room room = world[x, y];
                    if (player.X == x && player.Y == y)
                        Console.Write("P");
                    else if (room.Monster != null)
                        Console.Write("M");
                    else if (room.Item != null)
                        Console.Write("I");
                    else
                        Console.Write(".");
                }
                Console.WriteLine();
            }
        }

        private void DisplayStats()
        {
            Console.WriteLine($"Monsters: {Monster.MonsterCount}");
            Console.WriteLine($"Health: {player.Health}");
            foreach (var item in player.Backpack)
            {
                Console.WriteLine(item.Name);
            }
        }

        private void AskForMovement()
        {
            int newX = player.X;
            int newY = player.Y;
            bool isValidKey = true;

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.RightArrow: newX++; break;
                case ConsoleKey.LeftArrow: newX--; break;
                case ConsoleKey.UpArrow: newY--; break;
                case ConsoleKey.DownArrow: newY++; break;
                default: isValidKey = false; break;
            }

            if (isValidKey &&
                newX >= 0 && newX < world.GetLength(0) &&
                newY >= 0 && newY < world.GetLength(1))
            {
                player.X = newX;
                player.Y = newY;

                EnterRoom();
            }
        }

        private void EnterRoom()
        {
            Room currentRoom = world[player.X, player.Y];
            Monster monster = currentRoom.Monster;

            if (monster != null)
            {
                AttackResult result = monster.Attack(player);
                Console.WriteLine($"Monster ({result.Attacker.Health}) damaged player by {result.Damage}");
                Console.ReadKey(true);


                if (player.IsAlive)
                    player.Attack(monster);
                    Console.WriteLine($"Player damaged monster by {result.Damage}");
                    Console.ReadKey(true);

                if (!monster.IsAlive)
                    player.Backpack.Add(currentRoom.Monster);
                    currentRoom.Monster = null;
            }

            if (player.IsAlive && currentRoom.Item != null)
            {
                player.Backpack.Add(currentRoom.Item);
                currentRoom.Item.Use(player);
                currentRoom.Item = null; // remove item from the room after it is picked up
            }
        }

        private void GameOver()
        {
            Console.Clear();

            if (Monster.MonsterCount > 0)
                Console.WriteLine("Game over...");
            else
                Console.WriteLine("Congrats on defeating all the sacry monsters in the dungeon!");
            Console.ReadKey();
            Play();
        }
    }
}
