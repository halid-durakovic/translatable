﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Translatable
{
    internal class EnumTranslationFactory
    {
        internal static EnumTranslation<T>[] CreateEnumTranslation<T>(ITranslationSource translationSource) where T : struct, IConvertible
        {
            var type = typeof(T);

            if (!type.IsEnum)
                throw new InvalidOperationException($"The type {type.Name} has to be an enum.");

            if (!type.IsDefined(typeof(TranslatableAttribute), false))
                throw new InvalidOperationException($"The type {type.Name} is no translatable enum! Add the Attribute Translatable to the enum declaration.");

            var values = new List<EnumTranslation<T>>();

            foreach (var value in Enum.GetValues(type).Cast<T>())
            {
                try
                {
                    var msgid = TranslationAttribute.GetValue(value);
                    var translation = GetTranslation(msgid, translationSource);
                    values.Add(new EnumTranslation<T>(translation, value));
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException($"The value {value} in enum {type.Name} does not have the [Translation] attribute. This is required to make it translatable.");
                }
            }

            return values.ToArray();
        }

        private static string GetTranslation(string msgId, ITranslationSource translationSource = null)
        {
            if (translationSource == null)
                return msgId;
            return translationSource.GetTranslation(msgId);
        }
    }
}