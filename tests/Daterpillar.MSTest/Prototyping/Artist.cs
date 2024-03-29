﻿using Acklann.Daterpillar.Annotations;
using System;

namespace Acklann.Daterpillar.Prototyping
{
    [Table("artist")]
    public class Artist
    {
        [Column("id", AutoIncrement = true)]
        public int Id { get; set; }

        [Index, Column]
        public string Name { get; set; }

        [Column]
        public string Bio { get; set; }

        [Column]
        public DateTime DOB { get; set; }
    }
}