using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace PairMatching.Models
{
    public class BaseModel : IEquatable<BaseModel>
    {
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// Is this record as deleted from the database
        /// </summary>
        public bool IsDeleted { get; set; }

        public IEnumerable<Note> Notes { get; set; }


        public bool Equals(BaseModel other)
        {
            if (other is null)
            {
                return false;
            }
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BaseModel);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, IsDeleted);
        }
    }
}
