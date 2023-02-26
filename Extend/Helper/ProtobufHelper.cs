using System;
using System.ComponentModel;
using System.IO;
using YuoTools.Main.Ecs;
using ProtoBuf;
using ProtoBuf.Meta;

namespace YuoTools.Extend.Helper
{
    public class ProtobufHelper
    {
        public static byte[] ToBytes(object message)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, message);
                return stream.ToArray();
            }
        }

        public static void ToStream(object message, MemoryStream stream)
        {
            Serializer.Serialize(stream, message);
        }

        public static string ToString(object message)
        {
            return ToBytes(message).ToStr();
        }

        public static object FromStream(Type type, MemoryStream stream)
        {
            object o = RuntimeTypeModel.Default.Deserialize(stream, null, type);
            if (o is ISupportInitialize supportInitialize)
            {
                supportInitialize.EndInit();
            }

            return o;
        }

        public static object FromStream<T>(MemoryStream stream)
        {
            return FromStream(typeof(T), stream);
        }

        public static T FromBytes<T>(byte[] bytes, int index, int count)
        {
            using (MemoryStream stream = new MemoryStream(bytes, index, count))
            {
                var result = Serializer.Deserialize<T>(stream);
                if (result is ISupportInitialize supportInitialize)
                {
                    supportInitialize.EndInit();
                }

                return result;
            }
        }

        public static T FromBytes<T>(byte[] bytes)
        {
            return FromBytes<T>(bytes, 0, bytes.Length);
        }
        
        public static T FromString<T>(string str)
        {
            return FromBytes<T>(StringHelper.ToByteArray(str));
        }
    }
}