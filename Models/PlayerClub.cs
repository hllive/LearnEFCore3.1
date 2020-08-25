using System;

namespace Models
{
    //根据视图ViewPlayerClub创建一个类
    //这个类没有主键
    public class PlayerClub
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string ClubName { get; set; }
        public string ClubCity { get; set; }
    }
}