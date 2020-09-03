using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Pictua
{
    internal static class Xml
    {
        private static readonly Dictionary<Type, XmlSerializer> _serializers = new Dictionary<Type, XmlSerializer>();

        public static void Serialize(Type type, Stream stream, object? value)
        {
            if (!_serializers.TryGetValue(type, out var serializer))
            {
                _serializers[type] = serializer = new XmlSerializer(type);
            }
            serializer.Serialize(stream, value);
        }

        public static object Deserialize(Type type, Stream stream)
        {
            if (!_serializers.TryGetValue(type, out var serializer))
            {
                _serializers[type] = serializer = new XmlSerializer(type);
            }
            return serializer.Deserialize(stream);
        }

        public static void Serialize<T>(Stream stream, T value)
        {
            Serialize(typeof(T), stream, value);
        }

        public static T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(typeof(T), stream);
        }
    }
}