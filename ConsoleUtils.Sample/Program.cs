﻿// Author: Prasanna V. Loganathar
// Project: ConsoleUtils.Sample
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
// Created: 12:43 AM 18-04-2014

namespace ConsoleUtils.Sample
{
    using System;
    using System.Threading.Tasks;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var progressBar = new FixedWidthInfomativeProgressBar(120);
            var t = Task.Run(
                () =>
                    {
                        for (int i = 0; i < 120; i++)
                        {
                            progressBar.UpdateProgress(i + 1);
                            Task.Delay(50).Wait();
                        }
                    });

            t.Wait();

            var pb = new FixedWidthProgressBar(90);
            t = Task.Run(
                () =>
                    {
                        for (int i = 0; i <= 120; i++)
                        {
                            pb.UpdateProgress((double)i / 120);
                            Task.Delay(50).Wait();
                        }
                    });

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}