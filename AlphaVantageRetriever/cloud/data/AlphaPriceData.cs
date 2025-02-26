﻿using System;

#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageRetriever;

public class AlphaPriceData
{
    public string Symbol { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
}
