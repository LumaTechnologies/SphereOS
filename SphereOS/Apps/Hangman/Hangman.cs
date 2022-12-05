using SphereOS.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SphereOS.Apps.Hangman
{
    internal class Hangman
    {
        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Apps.Hangman.WordList.txt")]
        private static readonly byte[] wordListBytes;

        private static string[] wordList;

        private static string[] man = new string[]
        {
                "    ___    \n" +
                "   |   |   \n" +
                "   |   O   \n" +
                "   |       \n" +
                "   |       \n" +
                "   |       \n" +
                "   |       \n"
            ,
                "    ___    \n" +
                "   |   |   \n" +
                "   |   O   \n" +
                "   |   |   \n" +
                "   |   |   \n" +
                "   |       \n" +
                "   |       \n"
            ,
                "    ___    \n"  +
                "   |   |   \n"  +
                "   |   O   \n"  +
                "   |  \\|  \n"  +
                "   |   |   \n"  +
                "   |       \n"  +
                "   |       \n"
            ,
                "    ___    \n"  +
                "   |   |   \n"  +
                "   |   O   \n"  +
                "   |  \\|/ \n"  +
                "   |   |   \n"  +
                "   |       \n"  +
                "   |       \n"
            ,
                "    ___    \n"  +
                "   |   |   \n"  +
                "   |   O   \n"  +
                "   |  \\|/ \n"  +
                "   |   |   \n"  +
                "   |  /    \n"  +
                "   |       \n"
            ,
                "    ___    \n"  +
                "   |   |   \n"  +
                "   |   O   \n"  +
                "   |  \\|/ \n"  +
                "   |   |   \n"  +
                "   |  / \\ \n"  +
                "   |       \n"
            ,
                "    ___    \n"  +
                "   |   |   \n"  +
                "   |   X   \n"  +
                "   |  \\|/ \n"  +
                "   |   |   \n"  +
                "   |  / \\ \n"  +
                "   |       \n"
        };

        private string word;

        private List<char> letters;

        private Random random;

        private bool playing = false;

        private bool gameOver = false;

        private bool exited = false;

        private string message = string.Empty;

        private int fails = 0;

        private void Render()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine();
            Console.WriteLine(man[fails]);
            Console.WriteLine();

            for (int i = 0; i < word.Length; i++)
            {
                if (gameOver || letters.Contains(word[i]))
                {
                    Console.Write(word[i].ToString().ToUpper() + " ");
                }
                else
                {
                    Console.Write("_ ");
                }
            }

            bool parenthesesStarted = false;
            for (int i = 0; i < letters.Count; i++)
            {
                if (!word.Contains(letters[i]))
                {
                    if (!parenthesesStarted)
                    {
                        Console.Write(" (");
                        parenthesesStarted = true;
                    }
                    Console.Write(letters[i]);
                }
            }
            if (parenthesesStarted)
            {
                Console.Write(")");
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        private bool IsWordRevealed()
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (!letters.Contains(word[i]))
                {
                    return false;
                }
            }
            return true;
        }

        internal Hangman()
        {
            wordList = Encoding.ASCII.GetString(wordListBytes).Split("\n");
            random = new Random();
        }

        internal void Play()
        {
            word = wordList[random.Next(wordList.Length - 1)];
            letters = new List<char>();
            playing = true;
            fails = 0;
            message = "HANGMAN (Type 'Exit' to exit.)";
            exited = false;
            gameOver = false;

            while (playing)
            {
                Render();

                message = string.Empty;

                string input = Console.ReadLine().ToLower().Trim();
                if (input == "exit")
                {
                    exited = true;
                    break;
                }
                else if (input.Length == 1)
                {
                    char letter = input[0];
                    if (!char.IsLetter(letter))
                    {
                        message = "Not a letter!";
                        continue;
                    }
                    if (letters.Contains(letter))
                    {
                        message = "You've already used that letter.";
                    }
                    else
                    {
                        if (!word.Contains(letter))
                        {
                            fails++;
                            if (fails >= 6)
                            {
                                gameOver = true;
                                message = $"Game over! The word was '{word}'.";
                                Render();
                                break;
                            }
                        }
                        letters.Add(letter);
                        if (IsWordRevealed())
                        {
                            message = "Congratulations, you win!";
                            Render();
                            break;
                        }
                    }
                }

                ProcessManager.Yield();
            }

            playing = false;

            if (!exited)
            {
                Console.ReadKey(true);
            }
        }
    }
}
