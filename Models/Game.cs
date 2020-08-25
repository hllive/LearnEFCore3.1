using System;
using System.Collections.Generic;

namespace Models
{
    //足球比赛
    public class Game
    {
        public Game()
        {
            GamePlayers = new List<GamePlayer>();
        }
        public Guid Id { get; set; }
        public int Round { get; set; }//比赛阶段
        public DateTimeOffset? StartTime { get; set; }//开始时间
        public List<GamePlayer> GamePlayers { get; set; }
    }
}
