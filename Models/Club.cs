using System;
using System.Collections.Generic;

namespace Models
{
    //足球队
    public class Club
    {
        public Club()
        {
            Players = new List<Player>();//为了以后使用不会遇到空引用
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public DateTime DateOfEstablishment { get; set; }//成立日期
        public string History { get; set; }//历史成绩
        public League League { get; set; }//联赛League导航属性
        public List<Player> Players { get; set; }//运动员列表
    }
}
