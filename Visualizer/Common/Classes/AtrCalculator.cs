using Common.Classes.General;
using System;
using System.Collections.Generic;

public class AtrCalculator
{
    private List<Candle> _candles;
    private int _rangeLength;

    public AtrCalculator(List<Candle> candles, int rangeLength)
    {
        _candles = candles;
        _rangeLength = rangeLength;
    }

    public double[] CalculateForAll()
    {
        List<double> result = new List<double>();

        for (int i = _rangeLength; i < _candles.Count; i++)
        {
            result.Add(Calculate(i));
        }

        return result.ToArray();
    }


    public double[] CalculateAccelerationForAll()
    {
        var atrs = CalculateForAll();

        List<double> result = new List<double>();

        for (int i = 2; i < atrs.Length; i++)
        {
            result.Add(atrs[i - 2] - 2 * atrs[i - 1] + atrs[i]);
        }

        return result.ToArray();
    }

    public double Calculate(int candleIndex)
    {
        if (candleIndex > _candles.Count)
        {
            throw new ArgumentException("Candle index is bigger than number of candles");
        }

        var tempCandles = new Candle[_rangeLength]; //нужен непрерывный кусок свечей

        for (int i = candleIndex, j = 0; i >= 0 && j < _rangeLength; i--, j++)
        {
            tempCandles[j] = _candles[i];
        }

        return new SimpleAtr(tempCandles).Calculate();
    }
}