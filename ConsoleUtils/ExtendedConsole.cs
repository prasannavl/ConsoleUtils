// Author: Prasanna V. Loganathar
// Project: ConsoleUtils
// 
// Copyright 2014 Launchark. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//  
// Created: 9:21 PM 03-04-2014

namespace ConsoleUtils
{
    using System;
    using System.Text;

    public static class ExtendedConsole
    {
        public static ConsoleColor ErrorColor = ConsoleColor.Red;
        public static ConsoleColor WarningColor = ConsoleColor.DarkYellow;
        public static ConsoleColor SuccessColor = ConsoleColor.Green;
        public static ConsoleColor InfoColor = ConsoleColor.DarkGray;

        static ExtendedConsole()
        {
            ConsoleResetHandler = delegate { Console.ResetColor(); };
            Console.CancelKeyPress += ConsoleResetHandler;
        }

        public static ConsoleCancelEventHandler ConsoleResetHandler { get; set; }

        public static void DoColored(ConsoleColor textColor, Action<object> action, object actionState)
        {
            DoColored(textColor, Console.BackgroundColor, action, actionState);
        }

        public static void DoColored(
            ConsoleColor textColor,
            ConsoleColor backgroundColor,
            Action<object> action,
            object actionState)
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
                action(actionState);
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
            DoColored(textColor, Console.WriteLine, value);
        }

        public static void Write(ConsoleColor textColor, string format, params object[] args)
        {
            DoColored(textColor, Console.Write, string.Format(format, args));
        }

        public static void Write(ConsoleColor textColor, object value)
        {
            DoColored(textColor, Console.Write, value);
        }

        public static void WriteLine(ConsoleColor textColor, string format, params object[] args)
        {
            DoColored(textColor, Console.WriteLine, string.Format(format, args));
        }

        public static void WriteCentered(
            string text,
            char fillerChar = ' ',
            int leftSpacing = 1,
            int rightSpacing = 1,
            bool ignoreRight = false)
        {
            Console.Write(GetCenteredString(text, fillerChar, leftSpacing, rightSpacing, ignoreRight));
        }

        public static void WriteLineCentered(
            string text,
            char fillerChar = ' ',
            int leftSpacing = 1,
            int rightSpacing = 1,
            bool ignoreRight = false)
        {
            Console.WriteLine(GetCenteredString(text, fillerChar, leftSpacing, rightSpacing, ignoreRight));
        }

        public static StringBuilder GetCenteredString(
            string text,
            char fillerChar = ' ',
            int leftSpacing = 1,
            int rightSpacing = 1,
            bool ignoreRight = false)
        {
            var sb = new StringBuilder(Console.WindowWidth);

            var innerLength = text.Length + leftSpacing + rightSpacing;
            var fillerLength = (Console.WindowWidth - innerLength) / 2;
            sb.Append(new string(fillerChar, fillerLength));
            sb.Append(new string(' ', leftSpacing) + text + new string(' ', rightSpacing));

            // Remove an extra filler to make sure the content is centralized regardless of even or odd length.
            if (!ignoreRight)
            {
                sb.Append(new string(fillerChar, innerLength % 2 == 0 ? fillerLength - 1 : fillerLength));
            }

            return sb;
        }

        public static void FillRow(char c, string addendum = null)
        {
            Console.Write(new string(c, Console.WindowWidth - 1) + addendum);
        }

        public static void FillLine(char c)
        {
            FillRow(c, Environment.NewLine);
        }

        public static void FillRemainingLine(char c)
        {
            FillRemainingRow(c, Environment.NewLine);
        }

        public static void FillRemainingRow(char c, string addendum = null)
        {
            Console.Write(new string(c, Console.WindowWidth - Console.CursorLeft - 1) + addendum);
        }

        /// <summary>
        ///     Set console buffer if possible based on the platform, and return true if it was successfully set.
        ///     <para>
        ///         Note: Buffer size is set to the maximum value of Int16.MaxValue - 1, if its larger than that,
        ///         and will still return true.
        ///     </para>
        /// </summary>
        /// <param name="width">Buffer width. A value of -1 leaves it unchanged.</param>
        /// <param name="height">Buffer height. A value of -1 leaves it unchanged.</param>
        /// <returns>True if the buffer was successfully set.</returns>
        public static bool SetBufferSize(int width = -1, int height = -1)
        {
            try
            {
                var platform = Environment.OSVersion.Platform;
                if (platform.HasFlag(PlatformID.Win32NT) || platform.HasFlag(PlatformID.Win32Windows)
                    || platform.HasFlag(PlatformID.Win32S))
                {
                    const int Max = Int16.MaxValue - 1;

                    if (height < 0)
                    {
                        height = Console.BufferHeight;
                    }
                    else if (height > Max)
                    {
                        height = Max;
                    }
                    if (width < 0)
                    {
                        width = Console.BufferWidth;
                    }
                    else if (width > Max)
                    {
                        width = Max;
                    }

                    Console.SetBufferSize(width, height);
                    return true;
                }
            }
            catch
            {
                // Any errors can be ignored.
            }

            return false;
        }
    }
}