// Author: Prasanna V. Loganathar
// Project: ConsoleUtils
// Copyright (c) Launchark. All rights reserved.
// See License.txt in the project root for license information.
//  
// Created: 9:21 PM 03-04-2014

namespace ConsoleUtils
{
    using System;

    public static class ExtendedConsole
    {
        public static ConsoleColor ErrorColor = ConsoleColor.Red;
        public static ConsoleColor WarningColor = ConsoleColor.DarkYellow;
        public static ConsoleColor SuccessColor = ConsoleColor.Green;
        public static ConsoleColor InfoColor = ConsoleColor.DarkGray;

        private static ConsoleCancelEventHandler ConsoleResetHandler;

        static ExtendedConsole()
        {
            ConsoleResetHandler = delegate { Console.ResetColor(); };
            Console.CancelKeyPress += ConsoleResetHandler;
        }

        public static ConsoleCancelEventHandler GetConsoleResetHandler()
        {
            return ConsoleResetHandler;
        }

        public static void ColoredAction(ConsoleColor textColor, Action action)
        {
            ColoredAction(textColor, Console.BackgroundColor, action);
        }

        public static void ColoredAction(ConsoleColor textColor, ConsoleColor backgroundColor, Action action)
        {
            lock (Console.Out)
            {
                var currentBgColor = Console.BackgroundColor;
                var currentFgColor = Console.ForegroundColor;

                var shouldReplaceBgColor = currentBgColor != backgroundColor;

                Console.ForegroundColor = textColor;
                if (shouldReplaceBgColor)
                {
                    Console.BackgroundColor = backgroundColor;
                }
                action();
                Console.ForegroundColor = currentFgColor;
                if (shouldReplaceBgColor)
                {
                    Console.BackgroundColor = currentBgColor;
                }
            }
        }

        public static void WriteErrorLine(object value)
        {
            WriteLine(ErrorColor, value);
        }

        public static void WriteErrorLine(string format, params object[] args)
        {
            WriteLine(ErrorColor, format, args);
        }

        public static void WriteError(object value)
        {
            Write(ErrorColor, value);
        }

        public static void WriteError(string format, params object[] args)
        {
            Write(ErrorColor, format, args);
        }

        public static void WriteInfoLine(object value)
        {
            WriteLine(InfoColor, value);
        }

        public static void WriteInfoLine(string format, params object[] args)
        {
            WriteLine(InfoColor, format, args);
        }

        public static void WriteInfo(object value)
        {
            Write(InfoColor, value);
        }

        public static void WriteInfo(string format, params object[] args)
        {
            Write(InfoColor, format, args);
        }

        public static void WriteWarningLine(object value)
        {
            WriteLine(WarningColor, value);
        }

        public static void WriteWarningLine(string format, params object[] args)
        {
            WriteLine(WarningColor, format, args);
        }

        public static void WriteWarning(object value)
        {
            Write(WarningColor, value);
        }

        public static void WriteWarning(string format, params object[] args)
        {
            Write(WarningColor, format, args);
        }

        public static void WriteSuccessLine(object value)
        {
            WriteLine(SuccessColor, value);
        }

        public static void WriteSuccessLine(string format, params object[] args)
        {
            WriteLine(SuccessColor, format, args);
        }

        public static void WriteSuccess(object value)
        {
            Write(SuccessColor, value);
        }

        public static void WriteSuccess(string format, params object[] args)
        {
            Write(SuccessColor, format, args);
        }

        public static void WriteLine(ConsoleColor textColor, object value)
        {
            ColoredAction(textColor, () => Console.WriteLine(value));
        }

        public static void Write(ConsoleColor textColor, string format, params object[] args)
        {
            ColoredAction(textColor, () => Console.Write(format, args));
        }

        public static void Write(ConsoleColor textColor, object value)
        {
            ColoredAction(textColor, () => Console.Write(value));
        }

        public static void WriteLine(ConsoleColor textColor, string format, params object[] args)
        {
            ColoredAction(textColor, () => Console.WriteLine(format, args));
        }

        public static void WriteCentered(
            string text,
            char fillerChar = ' ',
            int leftSpacing = 1,
            int rightSpacing = 1,
            bool ignoreRight = false,
            Action onFinish = null)
        {
            lock (Console.Out)
            {
                var innerLength = text.Length + leftSpacing + rightSpacing;
                var fillerLength = (Console.WindowWidth - innerLength) / 2;
                Console.Write(new string(fillerChar, fillerLength));
                Console.Write(new string(' ', leftSpacing) + text + new string(' ', rightSpacing));

                // Remove an extra filler to make sure the content is centralized regardless of even or odd length.
                if (!ignoreRight)
                {
                    Console.Write(new string(fillerChar, innerLength % 2 == 0 ? fillerLength - 1 : fillerLength));
                }
                if (onFinish != null)
                {
                    onFinish();
                }
            }
        }

        public static void WriteLineCentered(
            string text,
            char fillerChar = ' ',
            int leftSpacing = 1,
            int rightSpacing = 1)
        {
            WriteCentered(text, fillerChar, leftSpacing, rightSpacing, onFinish: Console.WriteLine);
        }

        public static void FillRow(char c)
        {
            Console.Write(new string(c, Console.WindowWidth - 1));
        }

        public static void FillLine(char c)
        {
            FillRow(c);
            Console.WriteLine();
        }

        public static void FillRemainingLine(char c)
        {
            FillRemainingRow(c);
            Console.WriteLine();
        }

        public static void FillRemainingRow(char c)
        {
            Console.Write(new string(c, Console.WindowWidth - Console.CursorLeft - 1));
        }

        public static void RepeatString(string text, int numberOfTimes, bool breakLines = false)
        {
            for (int i = 0; i < numberOfTimes; i++)
            {
                Console.Write(text);
                if (breakLines)
                {
                    Console.WriteLine();
                }
            }
        }
    }
}