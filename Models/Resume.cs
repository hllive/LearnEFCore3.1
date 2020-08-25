using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    //简历
    public class Resume
    {
        public Guid Id { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }

        public Guid PlayerId { get; set; }//Player外键
        public Player Player { get; set; }//Player导航属性
    }
}