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
// Created: 4:24 AM 16-04-2014

namespace ConsoleUtils
{
    using System;
    using System.Diagnostics;
    using System.Text;

    using TextUtils;

    public class ProgressBar
    {
        public delegate StringBuilder ProgressBarHook(ProgressBar self, StringBuilder builder, double value);

        private string leftDiscriminator = "[";
        private string rightDiscriminator = "]";
        private string progressZoneFiller = "=";
        private string remainderZoneFiller = " ";

        public ProgressBarHook ProgressBarPreHooks;
        public ProgressBarHook ProgressBarPostHooks;
        private bool shouldFillRemainderZone = true;
        private string percentDisplayFormat = "{0:##0.#} %";
        private int percentDisplayPaddedLength = 7;
        private bool isValidated;

        public string LeftDiscriminator
        {
            get
            {
                return leftDiscriminator;
            }
            set
            {
                leftDiscriminator = value;
            }
        }

        public string RightDiscriminator
        {
            get
            {
                return rightDiscriminator;
            }
            set
            {
                rightDiscriminator = value;
            }
        }

        public string ProgressZoneFiller
        {
            get
            {
                return progressZoneFiller;
            }
            set
            {
                progressZoneFiller = value;
            }
        }

        public string RemainderZoneFiller
        {
            get
            {
                return remainderZoneFiller;
            }
            set
            {
                remainderZoneFiller = value;
            }
        }

        public bool ShouldFillRemainderZone
        {
            get
            {
                return shouldFillRemainderZone;
            }
            set
            {
                shouldFillRemainderZone = value;
            }
        }

        public string PercentDisplayFormat
        {
            get
            {
                return percentDisplayFormat;
            }
            set
            {
                percentDisplayFormat = value;
            }
        }

        public int PercentDisplayPaddedLength
        {
            get
            {
                return percentDisplayPaddedLength;
            }
            set
            {
                percentDisplayPaddedLength = value;
            }
        }

        protected virtual void Validate()
        {
            if (ProgressZoneFiller.Length != RemainderZoneFiller.Length)
            {
                throw new Exception("ProgressZoneFiller and RemainderZoneFiller must be of equal length.");
            }
            isValidated = true;
        }

        public virtual StringBuilder Build(double ratio, int progressWidth, StringBuilder builder = null)
        {
            Debug.WriteLine("Progress ratio: " + ratio);
            Debug.WriteLine("Progress width: " + progressWidth);

            if (!isValidated)
            {
                Validate();
            }

            if (builder == null)
            {
                builder = new StringBuilder(Console.WindowWidth);
            }
            if (ProgressBarPreHooks != null)
            {
                builder = ProgressBarPreHooks(this, builder, ratio);
            }
            if (progressWidth > 0)
            {
                builder = BuildProgressBar(builder, ratio, progressWidth);
            }
            if (ProgressBarPostHooks != null)
            {
                builder = ProgressBarPostHooks(this, builder, ratio);
            }
            return builder;
        }

        public virtual StringBuilder BuildProgressBar(StringBuilder builder, double ratio, int progressBarWidth)
        {
            progressBarWidth = progressBarWidth - (leftDiscriminator.Length + rightDiscriminator.Length);
            var fillerWidth = progressBarWidth / ProgressZoneFiller.Length;
            var currentIndicatorLength = (int)(ratio * fillerWidth);
            var remainderWidth = (progressBarWidth / RemainderZoneFiller.Length) - currentIndicatorLength;

            builder.Append(leftDiscriminator);
            builder.Append(ProgressZoneFiller.Repeat(currentIndicatorLength));
            if (ShouldFillRemainderZone)
            {
                builder.Append(RemainderZoneFiller.Repeat(remainderWidth));
            }
            builder.Append(rightDiscriminator);
            return builder;
        }

        public virtual void WriteProgress(double ratio, int progressWidth)
        {
            Console.Write(Build(ratio, progressWidth).ToString());
        }

        public virtual void UpdateProgress(double ratio, int progressWidth)
        {
            Console.Write(Build(ratio, progressWidth) + "\r");
        }

        public virtual void WriteProgressLine(double ratio, int progressWidth)
        {
            Console.WriteLine(Build(ratio, progressWidth).ToString());
        }

        public StringBuilder BuildPercentDisplay(StringBuilder builder, double ratio)
        {
            var progress = ratio * 100;
            builder.Append(string.Format(PercentDisplayFormat, progress).PadLeft(PercentDisplayPaddedLength));
            return builder;
        }

        public int GetHookGeneratedLength(ProgressBarHook hook)
        {
            if (hook == null)
            {
                return 0;
            }

            var tempBuilder = new StringBuilder();
            tempBuilder = hook(this, tempBuilder, 0);
            Debug.WriteLine("Hook length: " + tempBuilder.Length);
            return tempBuilder.Length;
        }
    }

    public class FixedWidthProgressBar : ProgressBar
    {
        public FixedWidthProgressBar(int width)
        {
            Width = width;
        }

        public int Width { get; set; }

        public void WriteProgress(double ratio)
        {
            WriteProgress(ratio, Width);
        }

        public void WriteProgressLine(double ratio)
        {
            WriteProgressLine(ratio, Width);
        }

        public void UpdateProgress(double ratio)
        {
            UpdateProgress(ratio, Width);
        }
    }

    public class FixedWidthInfomativeProgressBar : FixedWidthProgressBar
    {
        private double totalCount;
        private int totalCountLength;
        private bool isInitialized;
        private int progressBarLength;

        public FixedWidthInfomativeProgressBar(double totalCount, int width = -1)
            : base(width < 0 ? Console.WindowWidth - 1 : width)
        {
            TotalCount = totalCount;

            ProgressBarPreHooks += (self, b, v) =>
                {
                    var bx = b.Append(' ');
                    bx = ((FixedWidthInfomativeProgressBar)self).BuildCountInfo(bx, v);
                    return bx.Append(' ');
                };

            ProgressBarPostHooks += (self, b, v) =>
                {
                    var bx = self.BuildPercentDisplay(b, v);
                    return bx.Append(' ');
                };
        }

        public double TotalCount
        {
            get
            {
                return totalCount;
            }
            set
            {
                totalCount = value;
                totalCountLength = totalCount.ToString().Length;
            }
        }

        public new void WriteProgress(double count)
        {
            Console.Write(Build(count));
        }

        public new void UpdateProgress(double count)
        {
            Console.Write(Build(count) + "\r");
        }

        public new void WriteProgressLine(double count)
        {
            Console.WriteLine(Build(count));
        }

        public StringBuilder BuildCountInfo(StringBuilder builder, double ratio)
        {
            var currrentRequests = (ratio * TotalCount).ToString().PadLeft(totalCountLength, ' ');
            builder.Append(currrentRequests + " / " + TotalCount);
            return builder;
        }

        public void Initialize()
        {
            progressBarLength = Width
                                - (GetHookGeneratedLength(ProgressBarPreHooks)
                                   + GetHookGeneratedLength(ProgressBarPostHooks));
            isInitialized = true;
        }

        public StringBuilder Build(double count, StringBuilder builder = null)
        {
            Debug.WriteLine("Progress count: " + count);
            if (!isInitialized)
            {
                Initialize();
            }
            if (builder == null)
            {
                builder = new StringBuilder(Console.WindowWidth);
            }
            var progressRatio = count / TotalCount;
            return Build(progressRatio, progressBarLength, builder);
        }
    }
}