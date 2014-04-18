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
// Created: 9:24 PM 03-04-2014

namespace ConsoleUtils
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public class AssemblyMetaInfo
    {
        private const int DescriptionBit = 1 << 0;
        private const int CompanyBit = 1 << 1;
        private const int TitleBit = 1 << 2;
        private const int CopyrightBit = 1 << 3;

        private string company;
        private string copyright;
        private string description;
        private string title;

        private BitVector32 initStatus = new BitVector32(0);

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public AssemblyMetaInfo()
        {
            Assembly = Assembly.GetExecutingAssembly();
        }

        public AssemblyMetaInfo(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            Assembly = assembly;
        }

        public string Description
        {
            get
            {
                if (!initStatus[DescriptionBit])
                {
                    var descAttr = Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
                    Description = descAttr == null ? string.Empty : descAttr.Description;
                }
                return description;
            }
            set
            {
                description = value;
                initStatus[DescriptionBit] = true;
            }
        }

        public string Title
        {
            get
            {
                if (!initStatus[TitleBit])
                {
                    var titleAttr = Assembly.GetCustomAttribute<AssemblyTitleAttribute>();
                    Title = titleAttr == null ? string.Empty : titleAttr.Title;
                }
                return title;
            }
            set
            {
                title = value;
                initStatus[TitleBit] = true;
            }
        }

        public string Copyright
        {
            get
            {
                if (!initStatus[CopyrightBit])
                {
                    var copyrightAttr = Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
                    Copyright = copyrightAttr == null ? string.Empty : copyrightAttr.Copyright;
                }
                return copyright;
            }
            set
            {
                copyright = value;
                initStatus[CopyrightBit] = true;
            }
        }

        public virtual string CopyrightNormalizedText
        {
            get
            {
                return Regex.Replace(Copyright, "© +", string.Empty);
            }
        }

        public string Company
        {
            get
            {
                if (!initStatus[CompanyBit])
                {
                    var companyAttr = Assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
                    Company = companyAttr == null ? string.Empty : companyAttr.Company;
                }
                return company;
            }
            set
            {
                company = value;
                initStatus[CompanyBit] = true;
            }
        }

        public Assembly Assembly { get; private set; }
    }
}