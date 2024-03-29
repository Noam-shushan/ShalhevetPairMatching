﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models
{
    public class OldPairDto
    {
        [BsonId]
        public int Id { get; set; }

        /// <summary>
        /// flag that determine if the pair is deleted from the database 
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// the first student id 
        /// </summary>
        public int StudentFromIsraelId { get; set; }

        /// <summary>
        /// The student id for the first student
        /// </summary>
        public int StudentFromWorldId { get; set; }

        public DateTime DateOfCreate { get; set; }

        public DateTime DateOfUpdate { get; set; }

        public DateTime DateOfDelete { get; set; }

        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Preferred tracks of learning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public PrefferdTracks PrefferdTracks { get; set; }

        public List<NoteOld> Notes { get; set; } = new List<NoteOld>();
        
        [BsonIgnore]
        public Student StudentFromIsrael { get; set; }
        
        [BsonIgnore]
        public Student StudentFromWorld { get; set; }

        public Pair ToNewPair()
        {
            return new Pair
            {
                OldId = Id,
                DateOfCreate = DateOfCreate,
                DateOfDelete = DateOfDelete,
                IsActive = IsActive,
                IsDeleted = IsDeleted,
                Notes = Notes.Select(n => new Note
                {
                    Author = n.Author,
                    Content = n.Text,
                    Date = n.Date
                }).ToList(),
                Track = PrefferdTracks,
                //ParticipantFromIsrael = StudentFromIsrael.ToParticipant(),
                //ParticipantFromWorld = StudentFromWorld.ToParticipant(),              
            };
        }
    }
}
