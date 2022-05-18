using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Linq;
using System.Text;

namespace PairMatching.Models
{
    public class Participant
    {
        [BsonId]
        public string Id { get; set; }

        public ParticipantPreferences Preferences { get; set; }

        /// <summary>
        /// the name of the student
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the country of the student
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// the email of the student
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// the phone number of the student
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// the gender of the student
        /// </summary>
        public Genders Gender { get; set; }

        public DateTime DateOfRegistered { get; set; }

    }
}
