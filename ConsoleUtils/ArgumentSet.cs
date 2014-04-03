﻿// Author: Prasanna V. Loganathar
// Project: ConsoleUtils
// Copyright (c) Launchark. All rights reserved.
// See License.txt in the project root for license information.
//  
// Created: 9:26 PM 03-04-2014

namespace ConsoleUtils
{
    using System;
    using System.Reflection;
    using System.Text;

    using CommandLine;
    using CommandLine.Text;

    public class ArgumentSet
    {
        [Flags]
        public enum HelpOptionFlag : byte
        {
            All = 0xFF,
            Copyright = 1 << 0,
            Company = 1 << 1,
            Title = 1 << 2,
            Version = 1 << 3,
            Description = 1 << 4,
            OptionsTextHeading = 1 << 5,
        }

        private const string CRLF = "\r\n";

        private readonly AssemblyMetaInfo assemblyInfo;
        private readonly HelpOptionFlag helpOptionFlags;

        public string UndeterminedErrorMessage = "Sorry, something's wrong with the options.";
        public string PreOptionsText = CRLF + "Options: ";

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public ArgumentSet()
            : this(Assembly.GetExecutingAssembly())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public ArgumentSet(HelpOptionFlag helpOptionFlags)
            : this(Assembly.GetExecutingAssembly(), helpOptionFlags)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public ArgumentSet(Assembly assembly, HelpOptionFlag helpOptionFlags = HelpOptionFlag.All)
            : this(new AssemblyMetaInfo(assembly), helpOptionFlags)
        {
        }

        public ArgumentSet(AssemblyMetaInfo assemblyInfo, HelpOptionFlag helpOptionFlags = HelpOptionFlag.All)
        {
            this.assemblyInfo = assemblyInfo;
            this.helpOptionFlags = helpOptionFlags;
        }

        [Option('h', "help", HelpText = "Display this screen.")]
        public virtual bool DisplayHelp { get; set; }

        public virtual void PrintHelp()
        {
            Console.WriteLine(GetUsage());
        }

        public virtual void Process(string[] args, Action action)
        {
            try
            {
                if (Parser.Default.ParseArguments(args, this))
                {
                    if (DisplayHelp)
                    {
                        PrintHelp();
                        return;
                    }

                    action();
                }
                else
                {
                    ExtendedConsole.WriteErrorLine(CRLF + "Error: You've provided invalid options.");
                    PrintHelp();
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        public virtual void HandleError(Exception ex)
        {
            if (ex.InnerException != null)
            {
                ExtendedConsole.WriteErrorLine(CRLF + "Error: " + CRLF + CRLF + ex.InnerException.Message);
            }
            else
            {
                ExtendedConsole.WriteErrorLine(CRLF + UndeterminedErrorMessage);
                PrintHelp();
            }
        }

        public virtual string GetUsage()
        {
            var text = HelpText.AutoBuild(this, (x) => { });

            var copyright = new StringBuilder();
            if (helpOptionFlags.HasFlag(HelpOptionFlag.Copyright))
            {
                copyright.Append(assemblyInfo.CopyrightNormalizedText);
                if (helpOptionFlags.HasFlag(HelpOptionFlag.Company) && !string.IsNullOrEmpty(assemblyInfo.Company))
                {
                    copyright.Append(", " + assemblyInfo.Company);
                }
            }
            else if (helpOptionFlags.HasFlag(HelpOptionFlag.Company) && !string.IsNullOrEmpty(assemblyInfo.Company))
            {
                copyright.Append(assemblyInfo.Company);
            }

            copyright.Append(
                copyright.Length > 0 ? copyright[copyright.Length - 1] == '.' ? string.Empty : "." : string.Empty);

            text.Copyright = copyright.ToString();

            if (helpOptionFlags.HasFlag(HelpOptionFlag.OptionsTextHeading))
            {
                text.AddPreOptionsLine(PreOptionsText);
            }

            if (helpOptionFlags.HasFlag(HelpOptionFlag.Title))
            {
                if (helpOptionFlags.HasFlag(HelpOptionFlag.Version))
                {
                    text.Heading = CRLF + assemblyInfo.Title + " " + assemblyInfo.Assembly.GetName().Version;
                }
                else
                {
                    text.Heading = CRLF + assemblyInfo.Title;
                }

                if (helpOptionFlags.HasFlag(HelpOptionFlag.Description)
                    && !string.IsNullOrEmpty(assemblyInfo.Description))
                {
                    text.Heading += CRLF + assemblyInfo.Description;
                }
            }
            else
            {
                text.Heading = CRLF;
            }

            text.AddDashesToOption = true;

            return text;
        }
    }
}