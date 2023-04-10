using System;
using COP2513_001;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace COP2513_001
{
    public partial class CandlestickChartForm : Form
    {
        private List<Candlestick> candlesticks;
        public CandlestickChartForm(List<Candlestick> candlesticks)
        {
            InitializeComponent();

            // Store as instance variable
            this.candlesticks = candlesticks;

            // Set the chart title
            this.candlestickChart.Titles.Add("Candlestick Chart");

            // Set the chart data source
            this.candlestickChart.DataSource = candlesticks;

            // Set the series data members for the candlestick chart
            this.candlestickChart.Series["Candlestick"].XValueMember = "Date";
            this.candlestickChart.Series["Candlestick"].YValueMembers = "High,Low,Open,Close";
            this.candlestickChart.Series["Candlestick"].CustomProperties = "PriceDownColor=Red,PriceUpColor=Green";

            // Set the chart area and axis properties
            this.candlestickChart.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd";
            this.candlestickChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            this.candlestickChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            this.candlestickChart.ChartAreas[0].AxisY2.Enabled = AxisEnabled.False;
            this.candlestickChart.ChartAreas[0].AxisY.Minimum = (double)Math.Round(candlesticks.Min(c => c.Low), 2);
            this.candlestickChart.ChartAreas[0].AxisY.Maximum = (double)Math.Round(candlesticks.Max(c => c.High), 2);

        }


        private void patternType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPattern = patternType.SelectedItem.ToString();
            CandlestickRecognizer recognizer = new CandlestickRecognizer();


            // Identify the pattern in the stock data
            CandlestickPattern detectedPattern = recognizer.IdentifyPattern(candlesticks, selectedPattern);

            // If a pattern is detected, highlight it in the chart
            if (detectedPattern != null)
            {
                // Draw a rectangle around the pattern in the chart
                for (int i = detectedPattern.StartIndex; i <= detectedPattern.EndIndex; i++)
                {
                    DataPoint point = candlestickChart.Series["Candlestick"].Points[i];
                    point.Color = Color.Red;
                    point.BorderWidth = 2;
                }
            }
        }

        public class CandlestickPattern
        {
            public string Name { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
        }

        public class CandlestickRecognizer
        {
            private List<Candlestick> _candlesticks;

            public CandlestickRecognizer(List<Candlestick> candlesticks)
            {
                _candlesticks = candlesticks;
            }

            public List<Candlestick> FindDoji()
            {
                List<Candlestick> dojiList = new List<Candlestick>();

                foreach (Candlestick candlestick in _candlesticks)
                {
                    if (IsDoji(candlestick))
                    {
                        candlestick.Type = "Doji";
                        dojiList.Add(candlestick);
                    }
                }

                return dojiList;
            }

            public List<Candlestick> FindMarubozu()
            {
                List<Candlestick> marubozuList = new List<Candlestick>();

                foreach (Candlestick candlestick in _candlesticks)
                {
                    if (IsMarubozu(candlestick))
                    {
                        candlestick.Type = "Marubozu";
                        marubozuList.Add(candlestick);
                    }
                }

                return marubozuList;
            }

            public List<Candlestick> FindHammer()
            {
                List<Candlestick> hammerList = new List<Candlestick>();

                foreach (Candlestick candlestick in _candlesticks)
                {
                    if (IsHammer(candlestick))
                    {
                        candlestick.Type = "Hammer";
                        hammerList.Add(candlestick);
                    }
                }

                return hammerList;
            }

            public List<Candlestick> FindEngulfing()
            {
                List<Candlestick> engulfingList = new List<Candlestick>();

                for (int i = 0; i < _candlesticks.Count - 1; i++)
                {
                    if (IsEngulfing(_candlesticks[i], _candlesticks[i + 1]))
                    {
                        _candlesticks[i].Type = "Engulfing";
                        _candlesticks[i + 1].Type = "Engulfing";
                        engulfingList.Add(_candlesticks[i]);
                        engulfingList.Add(_candlesticks[i + 1]);
                    }
                }

                return engulfingList;
            }

            public List<Candlestick> FindDragonfly()
            {
                List<Candlestick> dragonflyList = new List<Candlestick>();

                foreach (Candlestick candlestick in _candlesticks)
                {
                    if (IsDragonfly(candlestick))
                    {
                        candlestick.Type = "Dragonfly";
                        dragonflyList.Add(candlestick);
                    }
                }

                return dragonflyList;
            }

            public List<Candlestick> FindGravestone()
            {
                List<Candlestick> gravestoneList = new List<Candlestick>();

                foreach (Candlestick candlestick in _candlesticks)
                {
                    if (IsGravestone(candlestick))
                    {
                        candlestick.Type = "Gravestone";
                        gravestoneList.Add(candlestick);
                    }
                }

                return gravestoneList;
            }

            public List<Candlestick> FindLongLeggedDoji()
            {
                List<Candlestick> longLeggedDojiList = new List<Candlestick>();

                foreach (Candlestick candlestick in _candlesticks)
                {
                    if (IsLongLeggedDoji(candlestick))
                    {
                        candlestick.Type = "Long-legged Doji";
                        longLeggedDojiList.Add(candlestick);
                    }
                }

                return longLeggedDojiList;
            }

            public bool IsDragonfly(Candlestick candlestick)
            {
                double bodySize = (double)Math.Abs(candlestick.Open - candlestick.Close);
                double totalSize = (double)(candlestick.High - candlestick.Low);
                double upperWick = (double)Math.Abs(candlestick.High - Math.Max(candlestick.Open, candlestick.Close));
                double lowerWick = (double)Math.Abs(Math.Min(candlestick.Open, candlestick.Close) - candlestick.Low);

                if (bodySize < totalSize * 0.05 && upperWick > totalSize * 0.4 && lowerWick < totalSize * 0.1)
                {
                    return true;
                }

                return false;
            }

            public bool IsGravestone(Candlestick candlestick)
            {
                double bodySize = (double)Math.Abs(candlestick.Open - candlestick.Close);
                double totalSize = (double)(candlestick.High - candlestick.Low);
                double upperWick = (double)Math.Abs(candlestick.High - Math.Max(candlestick.Open, candlestick.Close));
                double lowerWick = (double)Math.Abs(Math.Min(candlestick.Open, candlestick.Close) - candlestick.Low);

                if (bodySize < totalSize * 0.05 && lowerWick > totalSize * 0.4 && upperWick < totalSize * 0.1)
                {
                    return true;
                }

                return false;
            }

            public bool IsLongLeggedDoji(Candlestick candlestick)
            {
                double bodySize = (double)Math.Abs(candlestick.Open - candlestick.Close);
                double totalSize = (double)(candlestick.High - candlestick.Low);
                double upperWick = (double)Math.Abs(candlestick.High - Math.Max(candlestick.Open, candlestick.Close));
                double lowerWick = (double)Math.Abs(Math.Min(candlestick.Open, candlestick.Close) - candlestick.Low);

                if (bodySize < totalSize * 0.05 && upperWick >= totalSize * 0.6 && lowerWick >= totalSize * 0.6)
                {
                    return true;
                }

                return false;
            }

            public bool IsDoji(Candlestick candlestick)
            {
                double bodySize = (double)Math.Abs(candlestick.Open - candlestick.Close);
                double totalSize = (double)(candlestick.High - candlestick.Low);
                double upperWick = (double)Math.Abs(candlestick.High - Math.Max(candlestick.Open, candlestick.Close));
                double lowerWick = (double)Math.Abs(Math.Min(candlestick.Open, candlestick.Close) - candlestick.Low);

                if (bodySize < totalSize * 0.05 && upperWick > totalSize * 0.4 && lowerWick > totalSize * 0.4)
                {
                    return true;
                }

                return false;
            }

            public bool IsMarubozu(Candlestick candlestick)
            {
                double bodySize = (double)Math.Abs(candlestick.Open - candlestick.Close);
                double totalSize = (double)(candlestick.High - candlestick.Low);

                if (bodySize > totalSize * 0.95)
                {
                    return true;
                }

                return false;
            }

            public bool IsHammer(Candlestick candlestick)
            {
                if (candlestick.Open < candlestick.Close)
                {
                    return false; // Hammer is a bearish reversal pattern, so candlestick should open higher than it closes
                }

                decimal bodySize = Math.Abs(candlestick.Open - candlestick.Close);
                decimal lowerShadowSize = Math.Abs(candlestick.Low - Math.Min(candlestick.Open, candlestick.Close));
                decimal totalSize = Math.Abs(candlestick.High - candlestick.Low);

                // Hammer should have a small body and a long lower shadow
                bool hasSmallBody = bodySize <= 2 * candlestick.CandleSize;
                bool hasLongLowerShadow = lowerShadowSize >= 2 * candlestick.CandleSize && lowerShadowSize >= totalSize / 2;

                return hasSmallBody && hasLongLowerShadow;
            }

            public bool IsEngulfing(Candlestick first, Candlestick second)
            {
                // Check for a bullish engulfing pattern
                if (first.Close < first.Open && second.Close > first.Open && second.Open < first.Close && second.Close > second.Open)
                {
                    return true;
                }

                // Check for a bearish engulfing pattern
                if (first.Close > first.Open && second.Close < first.Open && second.Open > first.Close && second.Close < second.Open)
                {
                    return true;
                }

                // If neither pattern is detected, return false
                return false;
            }

        }
        public CandlestickPattern IdentifyPattern(List<Candlestick> candlesticks, string patternName)
            {
                CandlestickPattern pattern = null;

                // Check each candlestick in the list for the specified pattern
                for (int i = 0; i < candlesticks.Count; i++)
                {
                    switch (patternName)
                    {
                        case "Doji":
                            if (CandlestickRecognizer.IsDoji(candlesticks[i]))
                            {
                                pattern = new CandlestickPattern { Name = patternName, StartIndex = i, EndIndex = i };
                                return pattern;
                            }
                            break;
                        case "Marubozu":
                            if (CandlestickRecognizer.IsMarubozu(candlesticks))
                            {
                                pattern = new CandlestickPattern { Name = patternName, StartIndex = i - 1, EndIndex = i };
                                return pattern;
                            }
                            break;
                        case "Hammer":
                            if (CandlestickRecognizer.IsHammer(candlesticks, i))
                            {
                                pattern = new CandlestickPattern { Name = patternName, StartIndex = i - 1, EndIndex = i };
                                return pattern;
                            }
                            break;

                        case "Engulfing":
                            if (CandlestickRecognizer.IsEngulfing(candlesticks, i))
                            {
                                pattern = new CandlestickPattern { Name = patternName, StartIndex = i - 1, EndIndex = i };
                                return pattern;
                            }
                            break;

                        case "Dragonfly":
                            if (CandlestickRecognizer.IsDragonfly(candlesticks[i]))
                            {
                                pattern = new CandlestickPattern { Name = patternName, StartIndex = i, EndIndex = i };
                                return pattern;
                            }
                            break;

                        case "Gravestone":
                            if (CandlestickRecognizer.IsGravestone(candlesticks[i]))
                            {
                                pattern = new CandlestickPattern { Name = patternName, StartIndex = i, EndIndex = i };
                                return pattern;
                            }
                            break;

                        case "Long Legged Doji":
                            if (CandlestickRecognizer.IsLongLeggedDoji(candlesticks[i]))
                            {
                                pattern = new CandlestickPattern { Name = patternName, StartIndex = i, EndIndex = i };
                                return pattern;
                            }
                            break;
                    }
                }

                return pattern;
            }



        }
    }

