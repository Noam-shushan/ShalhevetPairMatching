using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace PairMatching.Models
{
    public record BaseModel /*: IEquatable<BaseModel>*/
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Is this record as deleted from the database
        /// </summary>
        public bool IsDeleted { get; set; }

        public List<Note> Notes { get; set; } = new();


        //public bool Equals(BaseModel other)
        //{
        //    if (other is null)
        //    {
        //        return false;
        //    }
        //    return Id == other.Id;
        //}

        //public override bool Equals(object obj)
        //{
        //    return Equals(obj as BaseModel);
        //}

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(Id, IsDeleted);
        //}
    }
}
