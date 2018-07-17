using System;
using System.Collections.Generic;

namespace Common
{
    /* 
     Можно считать что значения доходности распределено нормально. 
     Поэтому для поиска выбросов можно воспользоватся правилом 3 сигм. 
     Т.е аномалией считаем все, что лежит за пределами 3-х стандартных отклонений. 
     Мерой центральной тенденции наверно лучше считать медиану, а не среднее.
     Отклоненение тоже лучше посчитать как медиану отклонений.
     В итоге выбросом будем считать все, что лежит за пределами K медианных отклонений от центра
    */

    public class ThreeSigmaOutliersFinder
    {
        private const int Koefficient = 5;
        private readonly StatList _values = new StatList();
        private readonly List<double> _outliers = new List<double>();
        
        public ThreeSigmaOutliersFinder(IEnumerable<double> values)
        {
            _values.AddRange(values);         
        }

        public IEnumerable<double> Find()
        {            
            foreach (var value in _values)
            {
                var difference = Math.Abs(_values.Median - value);
                var theshold = _values.MedianSigma*Koefficient;

                if (difference > theshold)
                {
                    _outliers.Add(value);
                }
            }

            return _outliers;
        }        
    }
}
