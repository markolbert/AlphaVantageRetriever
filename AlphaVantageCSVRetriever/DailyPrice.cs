﻿using System;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class DailyPrice
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public decimal NameMatchScore { get; set; }
        public DateTime Date { get; set; }
        public decimal Volume { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }
}