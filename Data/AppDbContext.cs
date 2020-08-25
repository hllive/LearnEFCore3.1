using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<League> Leagues { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerClub> PlayerClubs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //设置多对多关系的联合主键
            modelBuilder.Entity<GamePlayer>().HasKey(x => new { x.PlayerId, x.GameId });

            //设置一对一关系
            modelBuilder.Entity<Resume>()
                .HasOne(x => x.Player)//一个Resume拥有一个Player
                .WithOne(x => x.Resume)//每个Player又带了一个Resume
                .HasForeignKey<Resume>(x => x.PlayerId);//Resume上又带了一个外键PlayerId

            //针对这种没有主键的model查询出来都是无法追踪的
            modelBuilder.Entity<PlayerClub>()
                .HasNoKey()//设置没有主键
                .ToView("ViewPlayerClub");//如果不写这句，当迁移的时候还会创建一个Table，把这个映射到一个视图上
        }
    }
}