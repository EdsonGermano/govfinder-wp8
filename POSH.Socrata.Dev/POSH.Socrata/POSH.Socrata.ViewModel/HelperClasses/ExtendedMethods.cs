using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace POSH.Socrata.ViewModel.HelperClasses
{
    public static class ExtendedMethods
    {
        /// <summary>
        /// converts IEnumerable to observablecollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            var col = new ObservableCollection<T>();
            foreach (var cur in enumerable)
            {
                col.Add(cur);
            }
            return col;
        }
    }
}