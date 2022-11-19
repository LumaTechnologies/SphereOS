using SphereOS.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace SphereOS
{
    public class TextEditor
    {
        // The path of the active file.
        private string path;

        // If the user chose to quit.
        private bool quit = false;

        // If the file has been modified since it was opened.
        private bool modified = false;

        // The contents of the clipboard.
        private string clipboard = string.Empty;

        // The lines in the file.
        private List<string> lines = new List<string>();

        // The current line.
        private int currentLine = 0;

        // The position of the cursor on the current line.
        private int linePos = 0;

        // Scrolling positions, relative to the top left.
        private int scrollX = 0;
        private int scrollY = 0;

        // The range of lines to redraw next render.
        private int? updatedLinesStart;
        private int? updatedLinesEnd;

        // The height of the titlebar.
        private const int TITLEBAR_HEIGHT = 1;

        // The height of the shortcut bar.
        private const int SHORTCUT_BAR_HEIGHT = 1;

        // The shortcuts to show in the shortcut bar.
        private readonly (string, string)[] SHORTCUTS = new (string, string)[]
        {
            ("Ctrl+X", "Quit"),
            ("Ctrl+S", "Save"),
            ("Ctrl+I", "Info"),
            ("Ctrl+K", "Cut Line"),
            ("Ctrl+V", "Paste"),
            ("Ctrl+R", "Run")
        };

        public TextEditor(string path)
        {
            this.path = path;

            if (path != null)
            {
                if (File.Exists(path))
                {
                    // Load the file.
                    string text = File.ReadAllText(path);

                    // Convert Windows line endings.
                    text = text.Replace("\r\n", "\n");

                    // Split lines.
                    lines.AddRange(text.Split('\n'));
                }
                else
                {
                    lines.Add(string.Empty);
                }

                updatedLinesStart = 0;
                updatedLinesEnd = lines.Count - 1;
            }
        }

        // Draw the text of the file and update the cursor.
        private void Render()
        {
            // Only draw if the lines have changed.
            if (updatedLinesStart != null)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                for (int i = (int)updatedLinesStart; i <= updatedLinesEnd; i++)
                {
                    int y = i - scrollY + TITLEBAR_HEIGHT;
                    if (y < TITLEBAR_HEIGHT || y >= Console.WindowHeight - SHORTCUT_BAR_HEIGHT) continue;

                    Console.SetCursorPosition(0, y);

                    // If we are outside the boundaries of the document, or the entire line is hidden by scrolling, then clear the line.
                    if (i >= lines.Count || scrollX >= lines[i].Length)
                    {
                        Console.Write(new string(' ', Console.WindowWidth));
                    }
                    else
                    {
                        string line = lines[i].Substring(scrollX, Math.Min(Console.WindowWidth, lines[i].Length - scrollX));
                        // Print the line, and pad it with spaces to clear the rest of the line.
                        // if you get an indexoutofrangeexception it's likely this (REMOVE THIS COMMENT)
                        Console.Write(line + new string(' ', Console.WindowWidth - line.Length));
                    }
                }

                // Reset the updated lines.
                updatedLinesStart = null;
                updatedLinesEnd = null;
            }

            //Console.CursorVisible = true;
            Console.SetCursorPosition(linePos - scrollX, currentLine + TITLEBAR_HEIGHT - scrollY);
        }

        // Insert a new line at the cursor.
        private void InsertLine()
        {
            string line = lines[currentLine];
            if (linePos == line.Length)
            {
                lines.Insert(currentLine + 1, string.Empty);
            }
            else
            {
                lines.Insert(currentLine + 1, line.Substring(linePos, line.Length - linePos));
                lines[currentLine] = line.Remove(linePos, line.Length - linePos);
            }
            updatedLinesStart = currentLine;
            updatedLinesEnd = lines.Count - 1;

            currentLine += 1;
            linePos = 0;

            modified = true;
        }

        // Insert text at the cursor.
        private void Insert(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                lines[currentLine] = lines[currentLine].Insert(linePos, text[i].ToString());
                linePos++;

                updatedLinesStart = currentLine;
                updatedLinesEnd = currentLine;
            }
            modified = true;
        }

        // Backspace at the cursor.
        private void Backspace()
        {
            if (linePos == 0)
            {
                if (currentLine == 0) return;
                linePos = lines[currentLine - 1].Length;
                lines[currentLine - 1] += lines[currentLine];

                updatedLinesStart = currentLine - 1;
                updatedLinesEnd = lines.Count - 1;

                lines.RemoveAt(currentLine);
                currentLine -= 1;
            }
            else
            {
                lines[currentLine] = lines[currentLine].Remove(linePos - 1, 1);
                linePos--;

                updatedLinesStart = currentLine;
                updatedLinesEnd = currentLine;
            }
            modified = true;
        }

        // Move the cursor left.
        private void MoveLeft()
        {
            if (linePos == 0)
            {
                if (currentLine == 0) return;
                currentLine--;
                linePos = lines[currentLine].Length;
            }
            else
            {
                linePos--;
            }
        }

        // Move the cursor right.
        private void MoveRight()
        {
            if (linePos == lines[currentLine].Length)
            {
                if (currentLine + 1 == lines.Count) return;
                currentLine++;
                linePos = 0;
            }
            else
            {
                linePos++;
            }
        }

        // Move the cursor up.
        private void MoveUp()
        {
            if (currentLine == 0) return;
            currentLine--;
            linePos = Math.Min(linePos, lines[currentLine].Length);
        }

        // Move the cursor down.
        private void MoveDown()
        {
            if (currentLine + 1 == lines.Count) return;
            currentLine++;
            linePos = Math.Min(linePos, lines[currentLine].Length);
        }

        // Jump back to the previous word.
        private void JumpToPreviousWord()
        {
            if (linePos == 0)
            {
                if (currentLine == 0) return;
                currentLine -= 1;
                linePos = lines[currentLine].Length;
                JumpToPreviousWord();
            }
            else
            {
                int res = lines[currentLine].Substring(0, linePos - 1).LastIndexOf(' ');
                linePos = res == -1 ? 0 : res + 1;
            }
        }

        // Jump forward to the next word.
        private void JumpToNextWord()
        {
            int res = lines[currentLine].IndexOf(' ', linePos + 1);
            if (res == -1)
            {
                if (currentLine == lines.Count - 1)
                {
                    linePos = lines[currentLine].Length;
                    return;
                }

                linePos = 0;
                currentLine++;
                while (lines[currentLine] == string.Empty)
                {
                    if (currentLine == lines.Count - 1) break;
                    currentLine++;
                }
            }
            else
            {
                linePos = res + 1;
            }
        }

        // Show a notification above the shortcut bar.
        private void ShowNotification(string text)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.WindowHeight - SHORTCUT_BAR_HEIGHT - 1);
            Console.Write($" {text} ");
        }

        // Render a prompt.
        private void RenderPrompt(string question, (string, string)[] shortcuts)
        {
            RenderShortcuts(shortcuts);

            int y = Console.WindowHeight - SHORTCUT_BAR_HEIGHT - 1;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            // Clear the line.
            Console.SetCursorPosition(0, y);
            Console.Write(new string(' ', Console.WindowWidth));

            Console.SetCursorPosition(1, y);
            Console.Write(question);

            Console.SetCursorPosition(question.Length + 1, y);
        }

        // Get the entire document as a string.
        private string GetAllText()
        {
            string text = string.Empty;
            foreach (string line in lines)
            {
                text += line + "\n";
            }
            // Strip the trailing newline.
            text = text.Remove(text.Length - 1);

            return text;
        }

        // Save the file.
        private void Save(bool showFeedback)
        {
            if (modified)
            {
                modified = false;
                string text = GetAllText();
                // Write the file.
                try
                {
                    File.WriteAllText(path, text);
                }
                catch
                {
                    ShowNotification("Failed to save");
                    return;
                }
            }

            if (showFeedback)
            {
                RenderUI();
                ShowNotification("Saved");
            }
        }

        // Quit, and if the file is modified, prompt to save it.
        private void Quit()
        {
            quit = true;
            if (modified)
            {
                RenderPrompt("Save your changes?", new (string, string)[] { ("Y", "Yes"), ("N", "No"), ("Esc", "Cancel") });
                bool choiceMade = false;
                while (!choiceMade)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Y:
                            Save(false);
                            choiceMade = true;
                            break;
                        case ConsoleKey.N:
                            choiceMade = true;
                            break;
                        case ConsoleKey.Escape:
                            choiceMade = true;
                            quit = false;

                            // Hide the prompt.
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Clear();
                            RenderUI();
                            updatedLinesStart = 0;
                            updatedLinesEnd = lines.Count - 1;
                            Render();
                            break;
                    }
                }
            }
        }

        // Show information about the document.
        private void ShowInfo()
        {
            ShowNotification($"Ln {currentLine + 1}, Col {linePos + 1}");
        }

        // Cut the current line.
        private void CutLine()
        {
            if (lines[currentLine] != string.Empty)
            {
                clipboard = lines[currentLine];
                if (lines.Count == 1)
                {
                    lines[currentLine] = string.Empty;
                    linePos = 0;

                    updatedLinesStart = 0;
                    updatedLinesEnd = 0;
                }
                else
                {
                    lines.RemoveAt(currentLine);

                    if (currentLine >= lines.Count)
                    {
                        currentLine--;
                    }

                    updatedLinesStart = currentLine;
                    updatedLinesEnd = lines.Count;
                }
                modified = true;
            }
            else
            {
                ShowNotification("Nothing was cut");
            }
        }

        // Paste from the clipboard.
        private void Paste()
        {
            if (clipboard != string.Empty)
            {
                Insert(clipboard);
            }
            else
            {
                ShowNotification("Nothing to paste");
            }
        }

        // Run RiverScript.
        private void RunRs()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            try
            {
                var interpreter = new RiverScript.VM.Interpreter();
                RiverScript.StandardLibrary.StandardLibrary.LoadStandardLibrary(interpreter);

                string source = GetAllText();
                var script = new RiverScript.Script(source);
                script.Lex();

                interpreter.InterpretScript(script);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error occurred while running script: {e.Message}");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            RenderUI();
            updatedLinesStart = 0;
            updatedLinesEnd = lines.Count - 1;
            Render();
        }

        // Handle keyboard input.
        private bool HandleInput()
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Modifiers)
            {
                case ConsoleModifiers.Control:
                    switch (key.Key)
                    {
                        case ConsoleKey.X:
                            Quit();
                            break;
                        case ConsoleKey.S:
                            Save(true);
                            break;
                        case ConsoleKey.I:
                            ShowInfo();
                            break;
                        case ConsoleKey.K:
                            CutLine();
                            break;
                        case ConsoleKey.V:
                            Paste();
                            break;
                        case ConsoleKey.R:
                            RunRs();
                            break;
                        case ConsoleKey.LeftArrow:
                            JumpToPreviousWord();
                            break;
                        case ConsoleKey.RightArrow:
                            JumpToNextWord();
                            break;
                        case ConsoleKey.UpArrow:
                            currentLine = 0;
                            linePos = 0;
                            break;
                        case ConsoleKey.DownArrow:
                            currentLine = lines.Count - 1;
                            linePos = lines[currentLine].Length;
                            break;
                    }
                    return false;
            }
            switch (key.Key)
            {
                case ConsoleKey.Backspace:
                    Backspace();
                    break;
                case ConsoleKey.Enter:
                    InsertLine();
                    break;
                case ConsoleKey.LeftArrow:
                    MoveLeft();
                    break;
                case ConsoleKey.RightArrow:
                    MoveRight();
                    break;
                case ConsoleKey.UpArrow:
                    MoveUp();
                    break;
                case ConsoleKey.DownArrow:
                    MoveDown();
                    break;
                default:
                    // Check if the character is within ASCII.
                    if (key.KeyChar >= 32 && key.KeyChar <= 126)
                    {
                        Insert(key.KeyChar.ToString());
                    }
                    break;
            }
            return false;
        }

        // Update scrolling.
        private void UpdateScrolling()
        {
            bool scrollChanged = false;

            if (currentLine < scrollY)
            {
                scrollY = currentLine;
                scrollChanged = true;
            }
            else if (currentLine >= scrollY + Console.WindowHeight - TITLEBAR_HEIGHT - SHORTCUT_BAR_HEIGHT)
            {
                scrollY = currentLine - Console.WindowHeight + TITLEBAR_HEIGHT + SHORTCUT_BAR_HEIGHT + 1;
                scrollChanged = true;
            }

            if (linePos < scrollX)
            {
                scrollX = linePos;
                scrollChanged = true;
            }
            else if (linePos > scrollX + Console.WindowWidth - 1)
            {
                scrollX = linePos - Console.WindowWidth + 1;
                scrollChanged = true;
            }

            if (scrollChanged)
            {
                updatedLinesStart = 0;
                updatedLinesEnd = lines.Count - 1;
            }
        }

        // Render a list of shortcuts.
        private void RenderShortcuts((string, string)[] shortcuts)
        {
            int y = Console.WindowHeight - 1;

            // Clear the line.
            Console.SetCursorPosition(0, y);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(new string(' ', Console.WindowWidth - 1));

            Console.SetCursorPosition(0, y);
            foreach (var shortcut in shortcuts)
            {
                // Accelerator.
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"{shortcut.Item1}");

                // Description.
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" {shortcut.Item2} ");
            }
        }

        // Render the status bar and shortcut bar.
        private void RenderUI()
        {
            // Status bar.
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 0);
            string text = "  Text Editor 1.1";
            Console.WriteLine(text + new string(' ', Console.WindowWidth - text.Length));

            string displayName = path == null ? "New File" : Path.GetFileName(path);
            Console.SetCursorPosition((Console.WindowWidth - displayName.Length) / 2, 0);
            Console.Write(displayName);

            // Shortcut bar.
            RenderShortcuts(SHORTCUTS);
        }

        // Start the text editor.
        public void Start()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            RenderUI();
            while (!quit)
            {
                Render();
                HandleInput();
                UpdateScrolling();

                ProcessManager.Yield();
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }
    }
}
