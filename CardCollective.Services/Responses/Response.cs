using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.Common.Responses
{
    public class Response : IResponse
    {
        public bool IsSuccess { get; set; }
        public string ResponseMessage { get; set; }

        public Response(string responseMessage, bool isSuccess = true)
        {
            ResponseMessage = responseMessage;
            IsSuccess = isSuccess;
        }
    }
}