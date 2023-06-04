using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DataAccess.Infrastructure
{
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

    [AttributeUsage(AttributeTargets.Property)]
    public class BsonIgnoreInParentAttribute : Attribute
    {
        public Type IgnoredClass { get; }

        public BsonIgnoreInParentAttribute(Type ignoredClass)
        {
            IgnoredClass = ignoredClass;
        }
    }

    public class BsonIgnoreInParentSerializer<T> : SerializerBase<T>
    {
        private readonly IBsonSerializer<T> defaultSerializer;

        public BsonIgnoreInParentSerializer(IBsonSerializer<T> defaultSerializer)
        {
            this.defaultSerializer = defaultSerializer;
        }

        public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return defaultSerializer.Deserialize(context, args);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
        {
            if (ShouldIgnore(context, typeof(T)))
                return;

            defaultSerializer.Serialize(context, args, value);
        }

        private bool ShouldIgnore(BsonSerializationContext context, Type type)
        {  
            var currentType = type;
            while (currentType != null)
            {
                if (currentType.GetCustomAttributes(typeof(BsonIgnoreInParentAttribute), true).Any())
                    return true;

                currentType = currentType.BaseType;
            }
            return false;
            
        }
    }

}
