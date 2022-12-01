using SphereOS.Shell;
using SphereOS.Users;
using System;

namespace SphereOS.Commands.GamesTopic
{
    internal class Hangman : Command
    {
        public Hangman() : base("hangman")
        {
            Description = "The classic guessing game.";

            Topic = "games";
        }

        internal override ReturnCode Execute(string[] args)
        {
            var hangman = new Apps.Hangman.Hangman();
            bool exit = false;
            while (!exit)
            {
                hangman.Play();
                Console.WriteLine("Play again? [y/N]: ");
                if (Console.ReadKey(true).Key != ConsoleKey.Y)
                {
                    exit = true;
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            return ReturnCode.Success;
        }
    }
}
