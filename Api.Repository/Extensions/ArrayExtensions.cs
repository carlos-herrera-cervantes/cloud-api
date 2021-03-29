using System;

namespace Api.Repository.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Checks if array is empty
        /// </summary>
        /// <param name="array"></param>
        /// <returns>If array is empty returns true otherwise false</returns>
        public static bool IsEmpty(this Array array) => array.Length == 0;
    }
}