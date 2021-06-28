using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CardCollectiveBot.Data.Entities
{
    public class Currency : ITrackedEntity
    {
        public ulong UserId { get; set; }

        [Range(0, int.MaxValue)]
        public int Mangoes { get; set; }

        public DateTime DateModified { get; set; }
    }
}
