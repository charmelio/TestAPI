using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationCoreTest1.Extensions
{
    public static class TExtensions
    {
        public static string GetEnumDescription<TEnum>(this TEnum item) =>
            item.GetType()
                .GetField(item.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .Cast<DescriptionAttribute>()
                .FirstOrDefault()?.Description ?? string.Empty;

        public static void SeedEnumValues<T, TEnum>(this DbSet<T> dbSet, Func<TEnum, T> converter) where T : class =>
            Enum.GetValues(typeof(TEnum))
                .Cast<object>()
                .Select(value => converter((TEnum)value))
                .ToList()
                .ForEach(instance => dbSet.Add(instance));
    }
}