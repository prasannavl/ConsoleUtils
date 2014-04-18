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
// Created: 9:26 PM 03-04-2014

namespace ConsoleUtils
{
    using System;
    using System.Reflection;
    using System.Text;

    using CommandLine;
    using CommandLine.Text;

    public class CommandLineOptions
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

        public const string UndeterminedErrorMessage = "Sorry, something's wrong with the given options.";
        public const string PreOptionsText = CRLF + "Options: ";
        public const string RequiredText = "[Required]";
        public const string ErrorDetailText = "Error details: ";
        public const string HelpMessage = "Display this screen.";
        public const string IncorrectOptionsText = CRLF + "The provided options are incorrect. Details are given below.";
        public const string ErrorText = "Error: ";
        private readonly AssemblyMetaInfo assemblyInfo;
        private readonly HelpOptionFlag helpOptionFlags;
        private BaseSentenceBuilder sentenceBuilder;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public CommandLineOptions()
            : this(Assembly.GetEntryAssembly())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public CommandLineOptions(HelpOptionFlag helpOptionFlags)
            : this(Assembly.GetEntryAssembly(), helpOptionFlags)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public CommandLineOptions(Assembly assembly, HelpOptionFlag helpOptionFlags = HelpOptionFlag.All)
            : this(new AssemblyMetaInfo(assembly), helpOptionFlags)
        {
        }

        public CommandLineOptions(AssemblyMetaInfo assemblyInfo, HelpOptionFlag helpOptionFlags = HelpOptionFlag.All)
        {
            this.assemblyInfo = assemblyInfo;
            this.helpOptionFlags = helpOptionFlags;
        }

        public BaseSentenceBuilder SentenceBuilder
        {
            get
            {
                return sentenceBuilder ?? (sentenceBuilder = new DefaultSentenceBuilder());
            }
            set
            {
                sentenceBuilder = value;
            }
        }

        [Option('h', "help", HelpText = HelpMessage)]
        public virtual bool DisplayHelp { get; set; }

        [ParserState]
        public IParserState State { get; set; }

        public virtual void PrintHelp()
        {
            Console.WriteLine(GetUsage());
        }

        public virtual void Process(string[] args, Action action, Action<ParserSettings> configuration = null)
        {
            try
            {
                var parser = configuration == null ? new Parser(s => s.CaseSensitive = true) : new Parser(configuration);
                if (parser.ParseArguments(args, this))
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
                    ExtendedConsole.WriteErrorLine(IncorrectOptionsText);
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
                ExtendedConsole.WriteErrorLine(CRLF + ErrorText + ex.InnerException.Message);
            }
            else
            {
                ExtendedConsole.WriteErrorLine(CRLF + UndeterminedErrorMessage);
                PrintHelp();
            }
        }

        public virtual HelpText GetHelpTextBuilder()
        {
            var builder = new HelpText(SentenceBuilder)
                              {
                                  AddDashesToOption = true,
                                  AdditionalNewLineAfterOption = true
                              };

            builder.AddOptions(this, RequiredText);
            ParseErrors(builder);

            return builder;
        }

        public virtual void ParseErrors(HelpText builder)
        {
            if (State != null && State.Errors != null)
            {
                if (State.Errors.Count > 0)
                {
                    var errors = builder.RenderParsingErrorsText(this, 2);
                    if (!string.IsNullOrEmpty(errors))
                    {
                        builder.AddPreOptionsLine(
                            string.Concat(Environment.NewLine, ErrorDetailText, Environment.NewLine));
                        builder.AddPreOptionsLine(errors);
                    }
                }
            }
        }

        public virtual HelpText CustomErrorWithUsage(string errorMessage)
        {
            var builder = GetHelpTextBuilder();

            if (State == null || State.Errors == null || State.Errors.Count == 0)
            {
                builder.AddPreOptionsLine(string.Concat(Environment.NewLine, ErrorDetailText, Environment.NewLine));
            }

            builder.AddPreOptionsLine(errorMessage);
            return GetUsage(builder);
        }

        public virtual HelpText GetHeader(HelpText helpTextBuilder)
        {
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

            helpTextBuilder.Copyright = copyright.ToString();

            if (helpOptionFlags.HasFlag(HelpOptionFlag.Title))
            {
                if (helpOptionFlags.HasFlag(HelpOptionFlag.Version))
                {
                    helpTextBuilder.Heading = CRLF + assemblyInfo.Title + " " + assemblyInfo.Assembly.GetName().Version;
                }
                else
                {
                    helpTextBuilder.Heading = CRLF + assemblyInfo.Title;
                }

                if (helpOptionFlags.HasFlag(HelpOptionFlag.Description)
                    && !string.IsNullOrEmpty(assemblyInfo.Description))
                {
                    helpTextBuilder.Heading += CRLF + assemblyInfo.Description;
                }
            }
            else
            {
                helpTextBuilder.Heading = CRLF;
            }

            return helpTextBuilder;
        }

        public virtual HelpText GetUsage(HelpText helpTextBuilder = null)
        {
            if (helpTextBuilder == null)
            {
                helpTextBuilder = GetHelpTextBuilder();
            }

            helpTextBuilder = GetHeader(helpTextBuilder);

            if (helpOptionFlags.HasFlag(HelpOptionFlag.OptionsTextHeading))
            {
                helpTextBuilder.AddPreOptionsLine(PreOptionsText);
            }

            helpTextBuilder.AddDashesToOption = true;

            return helpTextBuilder;
        }

        public class DefaultSentenceBuilder : EnglishSentenceBuilder
        {
            /// <summary>
            ///     Gets a string containing the sentence for missing required option in English.
            /// </summary>
            public override string RequiredOptionMissingText
            {
                get
                {
                    return " - Required option missing";
                }
            }

            /// <summary>
            ///     Gets a string containing the sentence for format violation in English.
            /// </summary>
            public override string ViolatesFormatText
            {
                get
                {
                    return " - Invalid format";
                }
            }

            /// <summary>
            ///     Gets a string containing the sentence for mutual exclusiveness violation in English.
            /// </summary>
            public override string ViolatesMutualExclusivenessText
            {
                get
                {
                    return " - Exclusive option. Only one exclusive option from a group may be used";
                }
            }
        }
    }
}