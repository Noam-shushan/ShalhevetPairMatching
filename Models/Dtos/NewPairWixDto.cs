﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Models.Dtos
{
    public class NewPairWixDto
    {
        /// <summary>
        /// _id
        /// </summary>
        public string chevrutaIdFirst { get; set; }
        public string chevrutaIdSecond { get; set; }
        public string trackId { get; set; }
        public DateTime date { get; set; }
    }
}
