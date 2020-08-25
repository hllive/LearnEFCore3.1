using System;

namespace Models
{
    // 联赛
    public class League
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }//国家
    }
}
