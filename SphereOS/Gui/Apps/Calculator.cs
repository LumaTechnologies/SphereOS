using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.UILib;
using System;
using System.Drawing;

namespace SphereOS.Gui.Apps
{
    internal class Calculator : Process
    {
        internal Calculator() : base("Calculator", ProcessType.Application) { }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private enum Operator
        {
            None,
            Add,
            Subtract,
            Multiply,
            Divide
        }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Calculator.GridButton.bmp")]
        private static byte[] gridButtonBytes;
        private static Bitmap gridButtonBitmap = new Bitmap(gridButtonBytes);

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Calculator.Display.bmp")]
        private static byte[] displayBytes;
        private static Bitmap displayBitmap = new Bitmap(displayBytes);

        private const int padding = 16;
        private const int gridButtonSize = 40;
        private const int resultHeight = 40;

        private long num1 = 0;
        private long num2 = 0;
        private Operator op = Operator.None;

        private void RenderGridButton(string text, int x, int y)
        {
            int buttonX = (x * gridButtonSize);
            int buttonY = (y * gridButtonSize) + resultHeight;
            window.DrawImage(gridButtonBitmap, buttonX, buttonY);
            window.DrawString(text, Color.Black, buttonX + (gridButtonSize / 2) - ((text.Length * 8) / 2), buttonY + (gridButtonSize / 2) - (16 / 2));
        }

        private void WindowClick(int x, int y)
        {
            int gridX = x / gridButtonSize;
            int gridY = (y - resultHeight) / gridButtonSize;
            if (gridY < 0)
            {
                return;
            }
            switch (gridY)
            {
                case 0:
                    switch (gridX)
                    {
                        case 0:
                            InputNum("7");
                            break;
                        case 1:
                            InputNum("8");
                            break;
                        case 2:
                            InputNum("9");
                            break;
                        case 3:
                            InputOp(Operator.Add);
                            break;
                    }
                    break;
                case 1:
                    switch (gridX)
                    {
                        case 0:
                            InputNum("4");
                            break;
                        case 1:
                            InputNum("5");
                            break;
                        case 2:
                            InputNum("6");
                            break;
                        case 3:
                            InputOp(Operator.Subtract);
                            break;
                    }
                    break;
                case 2:
                    switch (gridX)
                    {
                        case 0:
                            InputNum("1");
                            break;
                        case 1:
                            InputNum("2");
                            break;
                        case 2:
                            InputNum("3");
                            break;
                        case 3:
                            InputOp(Operator.Multiply);
                            break;
                    }
                    break;
                case 3:
                    switch (gridX)
                    {
                        case 0:
                            InputNum("0");
                            break;
                        case 1:
                            InputBksp();
                            break;
                        case 2:
                            InputEquals();
                            break;
                        case 3:
                            InputOp(Operator.Divide);
                            break;
                    }
                    break;
            }
        }

        private void RenderDisplay(bool updateWindow = true)
        {
            window.DrawImage(displayBitmap, 0, 0);
            string text;
            if (op != Operator.None)
            {
                char opChar;
                opChar = op switch
                {
                    Operator.Add => '+',
                    Operator.Subtract => '-',
                    Operator.Multiply => '*',
                    Operator.Divide => '/',
                    _ => throw new Exception("Unrecognised operator.")
                };
                text = num1.ToString().TrimEnd('.') + opChar + num2.ToString().TrimEnd('.');
            }
            else
            {
                text = num1.ToString().TrimEnd('.');
            }
            window.DrawString(text, Color.Black, window.Width - 12 - (text.Length * 8), 12);
            if (updateWindow)
            {
                wm.Update(window);
            }
        }

        private void InputNum(string num)
        {
            if (op != Operator.None)
            {
                num2 = long.Parse(num2.ToString() + num);
            }
            else
            {
                num1 = long.Parse(num1.ToString() + num);
            }
            RenderDisplay();
        }

        private void InputOp(Operator @operator)
        {
            if (op != Operator.None)
            {
                num1 = num2;
            }
            op = @operator;
            num2 = 0;
            RenderDisplay();
        }

        private void InputBksp()
        {
            long num = op != Operator.None ? num2 : num1;
            string numStr = num.ToString();
            if (numStr.Length > 1)
            {
                num = long.Parse(numStr.Substring(0, numStr.Length - 1));
            }
            else
            {
                num = 0;
            }
            if (op != Operator.None)
            {
                num2 = num;
            }
            else
            {
                num1 = num;
            }
            RenderDisplay();
        }

        private void InputEquals()
        {
            if (op == Operator.None) return;

            switch (op)
            {
                case Operator.Add:
                    num1 = num1 + num2;
                    break;
                case Operator.Subtract:
                    num1 = num1 - num2;
                    break;
                case Operator.Multiply:
                    num1 = num1 * num2;
                    break;
                case Operator.Divide:
                    if (num2 == 0)
                    {
                        MessageBox messageBox = new MessageBox(this, "Calculator", "Cannot divide by zero.");
                        messageBox.Show();
                        return;
                    }
                    num1 = num1 / num2;
                    break;
                default:
                    throw new Exception("Unrecognised operator.");
            }
            num2 = 0;
            op = Operator.None;
            RenderDisplay();
        }

        private void RenderGridButtons()
        {
            RenderGridButton("7", 0, 0);
            RenderGridButton("8", 1, 0);
            RenderGridButton("9", 2, 0);
            RenderGridButton("+", 3, 0);

            RenderGridButton("4", 0, 1);
            RenderGridButton("5", 1, 1);
            RenderGridButton("6", 2, 1);
            RenderGridButton("-", 3, 1);

            RenderGridButton("1", 0, 2);
            RenderGridButton("2", 1, 2);
            RenderGridButton("3", 2, 2);
            RenderGridButton("*", 3, 2);

            RenderGridButton("0", 0, 3);
            RenderGridButton("<-", 1, 3);
            RenderGridButton("=", 2, 3);
            RenderGridButton("/", 3, 3);
        }

        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, gridButtonSize * 4, (gridButtonSize * 4) + resultHeight);
            wm.AddWindow(window);

            window.Title = "Calculator";
            window.Clear(Color.Gray);
            window.Icon = AppManager.GetAppMetadata("Calculator").Icon;
            window.OnClick = WindowClick;
            window.Closing = TryStop;

            /*inputTextBlock = new TextBlock(window, padding / 2, padding / 2, window.Width - padding, resultHeight - padding);
            inputTextBlock.Background = Color.Gray;
            inputTextBlock.Foreground = Color.White;
            wm.AddWindow(inputTextBlock);*/

            RenderDisplay(updateWindow: false);

            RenderGridButtons();

            wm.Update(window);
        }

        internal override void Run()
        {

        }
    }
}
