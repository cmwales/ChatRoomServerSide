using System.Collections.Generic;
using System;
using System.Linq;

namespace SocialMix.Api.Helpers
{
    public static class EnumHelper
    {
        public static TEnum Parse<TEnum>(string value) where TEnum : struct, Enum
        {
            if (!Enum.TryParse<TEnum>(value, out TEnum result))
            {
                throw new ArgumentException($"Invalid value '{value}' for enum type '{typeof(TEnum).Name}'.");
            }

            return result;
        }

        public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct, Enum
        {
            return Enum.TryParse<TEnum>(value, out result);
        }

        public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        public static IEnumerable<string> GetNames<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetNames(typeof(TEnum));
        }
    }
}