using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.Common.Responses
{
    public interface IResponse
    {
        public bool IsSuccess { get; set; }

        public string ResponseMessage { get; set; }
    }
}
