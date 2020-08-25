using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    //运动员
    public class Player
    {
        public Player()
        {
            GamePlayers = new List<GamePlayer>();
        }
        public Guid Id { get; set; }

        [MaxLength(100), Required]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime Birth { get; set; }
        public List<GamePlayer> GamePlayers { get; set; }
        public Guid ResumeId { get; set; }//Resume外键
        public Resume Resume { get; set; }//Resume导航属性
    }
}