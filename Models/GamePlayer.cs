using System;

namespace Models
{
    //比赛和队员之间多对多关系，在DbContext中不设置DbSet属性
    //这只是一个间接对象，生成数据库表就会以类名作为表名-也就是GamePlayer
    public class GamePlayer
    {
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }

        //这里也可以体现一对多的关系
        public Game Game { get; set; }//Game导航属性
        public Player Player { get; set; }//Player导航属性
    }
}