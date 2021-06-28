using System;
using System.Collections.Generic;
using System.Text;

namespace CardCollectiveBot.Common.Responses
{
    public class Reward
    {
        public ulong Id { get; set; }

        public string Nickname { get; set; }

        public int Value { get; set; }

        public Reward(ulong id, string nickname, int value)
        {
            Id = id;
            Nickname = nickname;
            Value = value;
        }
    }
}
