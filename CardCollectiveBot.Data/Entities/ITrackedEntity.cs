using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.Data.Entities
{
    public interface ITrackedEntity
    {
        DateTime DateModified { get; set; }
    }
}
