using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Robot.Core.Classes
{
    /// <summary>
    /// ������� ��������� ������ �� ������ ������ ���������� � ���������.
    /// ����� ��������� ��������� ����� � ��������� ��� �� ��������� 
    /// (� ��������� ������ �� �������� ������� ����������, ������� = �������� = �������� = �������)
    /// ������������ ��������� �� ������� � 49 �������, �������� ��� ���� �������������� ������������. 
    /// ��� ��� � ��������� ���������� �������� ����� ���� �� ������������ ����� � ����� ��������� �������
    /// </summary>
    public class LastCandleRemover
    {
        private static readonly Func<Candle, bool> CheckForCurentTime =
            c => c.Date.TimeOfDay.Minutes == DateTime.Now.Minute && DateTime.Now.TimeOfDay.Seconds < 60;

        public static List<Candle> Remove(List<Candle> rawCandles, bool checkCurrentTime = true)
        {
            var lastCandle = rawCandles.LastOrDefault();

            if (lastCandle != null && (!checkCurrentTime || (CheckForCurentTime(lastCandle))))
            {
                /* We cannot check  lastCandle.Date.TimeOfDay.Seconds < 49 because
                   candles doesn't contain seconds - probably point for TODO: */      
          
                return rawCandles.TakeWhile((c, i) => i < rawCandles.Count - 1).ToList();
            }

            return rawCandles;
        }
    }
}