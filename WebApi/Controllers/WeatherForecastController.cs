using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        #region 构造函数
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly AppDbContext _dbContext;
        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            AppDbContext dbContext
            )
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        #endregion

        [HttpPost("add")]
        public IActionResult SaveLeague()
        {
            var league1 = new League()
            {
                Country = "贵州",
                Name = "贵州贵阳足球联赛"
            };
            var league2 = new League()
            {
                Country = "俄罗斯",
                Name = "第二冬季足球联赛"
            };
            var club = new Club
            {
                Name = "茅台足球队",
                City = "贵州仁怀",
                DateOfEstablishment = new System.DateTime(1999, 7, 1),
                League = league1
            };

            //批量插入相同类
            //第一种批量插入League，用params参数形式
            //_dbContext.Leagues.AddRange(league1, league2);
            //第二种批量插入League，使用集合
            //_dbContext.Leagues.AddRange(new List<League> { league1, league2 });

            ///批量插入不同类的方法
            _dbContext.AddRange(league1, league2, club);

            var count = _dbContext.SaveChanges();
            return Ok(count);
        }
        [HttpPost("Club")]
        public IActionResult SaveClub()
        {
            //1、通过邀请码获取一个联赛，叫《第一季度足球联赛》
            League league = _dbContext.Leagues.SingleOrDefault(l => l.Id == new Guid("3D0F192D-732B-4EDE-1EEA-08D848383261"));
            //2、创建一个俱乐部
            var club = new Club
            {
                //3、将获取的联赛添加到俱乐部中
                League = league,//这样League和Club就关联上了
                //4、设置其他属性
                Name = "新智联足球队",
                City = "贵州省贵阳市",
                DateOfEstablishment = new DateTime(2020, 8, 23),
                History = "参加很多比赛",
                //5、添加队员
                Players = new List<Player>() {
                    new Player{ Name="王建国",Birth=new DateTime(1994,8,2) },
                    new Player{ Name="李刚",Birth=new DateTime(1994,9,25) }
                }
            };
            //添加与保存
            _dbContext.Clubs.Add(club);
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }
        [HttpPost("Palyer")]
        public IActionResult AddPalyer()
        {
            //1、查询出俱乐部
            var club = _dbContext.Clubs.SingleOrDefault(l => l.Name == "新智联足球队");
            //2、在俱乐部中添加队员
            club.Players.Add(new Player() { Name = "张全蛋", Birth = new DateTime(1999, 12, 1) });
            //3、保存数据
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }

        [HttpPost("UseAttach")]
        public IActionResult UseAttach()
        {
            //1、查询出俱乐部（不追踪）
            var club = _dbContext.Clubs.AsNoTracking().SingleOrDefault(x => x.Name == "新智联足球队");
            //2、在查询出来的俱乐部中添加新队员
            club.Players.Add(new Player { Name = "陈浩杰", Birth = new DateTime(2000, 5, 6) });
            //3、使用Update更新数据
            //_dbContext.Clubs.Update(club);//离线数据需要使用Update方法
            _dbContext.Clubs.Attach(club);//附加对象
                                          //4、保存数据
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }

        [HttpPost("GamePlayer")]
        public IActionResult GamePlayer()
        {
            //1、查询符合添加的队员
            var players = _dbContext.Players.Where(p => p.Birth > new DateTime(1995, 1, 1)).ToList();
            //2、创建比赛对象
            var game = new Game
            {
                Round = 2,
                StartTime = new DateTime(2020, 5, 1),
                //3、将查询出来的队员和比赛建立关系
                GamePlayers = players.Select(p => new GamePlayer { Player = p }).ToList()
            };
            //添加和保存
            _dbContext.Games.Add(game);
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }


        [HttpGet]
        public IActionResult GetLeague()
        {
            //第一种 模仿查询
            var leagues = _dbContext.Leagues
                        .Where(l => l.Country.Contains("中"))//查询条件
                        .ToList();
            //第二种 模仿查询
            var league_ef = _dbContext.Leagues
                        .Where(l => EF.Functions.Like(l.Country, "中%"))
                        .ToList();
            //第二种
            var conutry = "俄罗斯";
            var league2 = (from lg in _dbContext.Leagues
                           where lg.Country == conutry//查询条件
                           select lg).ToList();
            return Ok(leagues);
        }
        [HttpGet("Single")]
        public IActionResult GetSingleLeague()
        {
            var _id = new Guid("4227506D-05E4-47A2-B94F-08D8451D5DC0");
            //第一种
            var leagues = _dbContext.Leagues.OrderByDescending(x => x.Id).SingleOrDefault(l => l.Id == _id);
            //第二种
            var league2 = _dbContext.Leagues.Find(_id);

            return Ok(new { leagues, league2 });
        }

        [HttpGet("Eager")]
        public IActionResult GetEager()
        {
            var clubs = _dbContext.Clubs
                .Where(c => c.Name.Contains("足球队"))   //过滤条件
                .Include(c => c.League)                 //关联数据-联赛
                .Include(c => c.Players)                //关联数据-队员
                    .ThenInclude(p => p.Resume)         //关联子属性的简历
                .Include(c => c.Players)                //继续关联数据-队员
                    .ThenInclude(p => p.GamePlayers)    //关联子属性
                        .ThenInclude(g => g.Game)       //GamePlayers关联Game子属性
                .ToList();
            return Ok(clubs);
        }
        [HttpGet("Eager2")]
        public IActionResult GetEager2()
        {
            var clubs = _dbContext.Clubs
                .Where(c => c.Name.Contains("足球队"))   //过滤条件
                .Select(x => new
                {
                    x.Id,//自己的属性
                    x.Name,
                    LeagueName = x.League.Name,//关联属性的属性
                    Players = x.Players.Where(p => p.Birth > new DateTime(2000, 1, 1))//查询过滤条件的队员
                })
                .ToList();
            return Ok(clubs);
        }

        [HttpGet("Explicit")]
        public IActionResult GetExplicit()
        {
            //1、查询一条俱乐部数据
            var club = _dbContext.Clubs.FirstOrDefault();
            //2、通过查询出来的对象逐一查询关联数据-队员
            _dbContext.Entry(club)
                .Collection(x => x.Players)//关联队员集合数据
                .Query().Where(x => x.Birth > new DateTime(2000, 1, 1))//添加过滤条件
                .Load();
            //3、通过查询出来的对象逐一查询关联数据-联赛
            _dbContext.Entry(club)
                .Reference(x => x.League)//关联单个联赛数据
                .Load();
            var data = _dbContext.Clubs.Where(x => x.League.Name.Contains("足球联赛"));

            return Ok(club);

        }


        [HttpDelete]
        public IActionResult DeleteLeague()
        {
            //先查询出来，因为只能删除被追踪的数据
            var league = _dbContext.Leagues.SingleOrDefault(l => l.Country == "贵州");
            if (league == null) return NotFound();
            //1、单独删除方法
            _dbContext.Leagues.Remove(league);//删除单个Leagues
            _dbContext.Remove(league);//直接在context上Remove()方法传入model，它会判断类型

            //2、批量删除方法
            var league2 = _dbContext.Leagues.SingleOrDefault(l => l.Country == "中国");
            _dbContext.Leagues.RemoveRange(league, league2);
            _dbContext.RemoveRange(league, league2);

            //执行数据库操作
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }

        //删除多对多关系数据
        [HttpDelete("GamePlayer")]
        public IActionResult DeleteGamePlayer()
        {
            //1、新建一个比赛和队员直接的关系对象，比如是前段传入的比赛ID和队员ID
            var gamePlayer = new GamePlayer
            {
                GameId = new Guid("D0E17A1A-6AC5-472E-C1D7-08D848950DDD"),//假如是前端传入的比赛ID
                PlayerId = new Guid("FA896D64-E87C-4087-4E18-08D847725F2B")//假如是前端传入的队员ID
            };
            //2、在Context中把GamePlayer删除
            _dbContext.Remove(gamePlayer);

            //执行数据库操作
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }


        [HttpPut]
        public IActionResult UpdateLeague()
        {
            //先查询出来，因为操作被追踪的数据
            var league = _dbContext.Leagues.FirstOrDefault();
            //修改Name属性，被追踪的league状态属性就会变为Modify
            league.Name += "-";
            //执行数据库操作
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }

        [HttpPut("UpdateLeagueForClub")]
        public IActionResult UpdateLeagueForClub()
        {
            var club = _dbContext.Clubs.Include(c => c.League).FirstOrDefault();
            club.League.Name = "新名称";
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }

        [HttpPut("UpdatePlayerForGame")]
        public IActionResult UpdatePlayerForGame()
        {
            //1、通过Game查询出多对多的GamePlayer的Player数据
            var game = _dbContext.Games
                .Include(g => g.GamePlayers)
                    .ThenInclude(p => p.Player)
                .AsNoTracking()//不追踪（离线数据）
                .FirstOrDefault();
            //2、修改Game中第一个队员的Name属性
            var firstPlayer = game.GamePlayers.First().Player;
            firstPlayer.Name = "李晓明";
            //3、修改队员（离线数据需要使用Update()方法）
            //_dbContext.Players.Update(firstPlayer);
            //4、使用EntityState.Modified手动设置修改状态
            _dbContext.Entry(firstPlayer).State = EntityState.Modified;
            //执行数据库操作
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }
        //直接传值、将比赛和队员关联起来
        [HttpPut("GameAndPlayer")]
        public IActionResult UpdateGameAndPlayer()
        {
            //1、添加队员和比赛之间的关系
            var gamePlayer = new GamePlayer
            {
                GameId = new Guid("D0E17A1A-6AC5-472E-C1D7-08D848950DDD"),//假如是前端传入的比赛ID
                PlayerId = new Guid("FA896D64-E87C-4087-4E18-08D847725F2B")//假如是前端传入的队员ID
            };
            //2、将GamePlayer添加至Context中
            _dbContext.Add(gamePlayer);
            //执行数据库操作
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }
        //查询出比赛，再添加队员
        [HttpPut("GameAddPlayer")]
        public IActionResult UpdateGameAddPlayer()
        {
            //1、把第一个比赛先查出来
            var game = _dbContext.Games.FirstOrDefault();
            //2、给查出来的比赛中添加一个队员
            game.GamePlayers.Add(new GamePlayer { PlayerId = new Guid("916EA175-5AA9-4249-4E19-08D847725F2B") });//将队员添加至比赛中
                                                                                                                 //执行数据库操作
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }
        [HttpPut("PlayerAddResume")]
        public IActionResult UpdatePlayerAddResume()
        {
            //1、查询出王建国队员
            var player = _dbContext.Players.FirstOrDefault(p => p.Name == "王建国");
            //2、给队员添加简历对象
            player.Resume = new Resume { Description = "我是一个DotNet工程师" };
            //执行数据库操作
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }
        [HttpPut("PlayerAddResume2")]
        public IActionResult UpdatePlayerAddResume2()
        {
            //1、查询出王建国队员（不追踪）
            var player = _dbContext.Players.AsNoTracking().FirstOrDefault(p => p.Name == "李刚");
            //2、给队员添加简历对象
            player.Resume = new Resume { Description = "我是全栈工程师" };

            //执行数据库操作
            _dbContext.Attach(player);//使用Attach方法会判断player主键是否有值，如果有值就是未修改状态
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }
        [HttpPut("PlayerAddResume3")]
        public IActionResult UpdatePlayerAddResume3()
        {
            //1、查询出王建国队员，一起把Resume也查询出来
            var player = _dbContext.Players
                        .Include(p => p.Resume).FirstOrDefault(p => p.Name == "李刚");
            //2、给队员添加简历对象，如果player中Resume已经存在，会先删除之前的Resume在添加新对象
            player.Resume = new Resume { Description = "我是全栈工程师" };

            //执行数据库操作
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }

        [HttpPut("test")]
        public IActionResult UpdateLeague2()
        {
            //var league = _dbContext.Leagues.AsNoTracking().FirstOrDefault();
            //这里模拟前端传过来的JSON数据序列化为对象
            var league = new League
            {
                Id = new Guid("EDAAEE79-78C9-43B5-A924-08D845203D11"),
                Name = "贵州贵阳足球联赛"
            };
            //修改对象的属性
            league.Name = league.Name.Replace("贵州贵阳", "遵义仁怀");
            //让context进行追踪，并知道它已经被修改
            _dbContext.Leagues.Update(league);
            //执行数据库操作
            int count = _dbContext.SaveChanges();
            return Ok(count);
        }


        #region 10、执行原生SQL
        [HttpGet("PlayerClub")]
        public IActionResult GetViewPlayerClubAsync()
        {
            //查询视图ViewPlayerClub
            var playerClub = _dbContext.PlayerClubs.Where(px => px.ClubCity.Contains("贵州")).ToList();
            return Ok(playerClub);
        }
        [HttpGet("SqlTest")]
        public IActionResult GetSqlTest()
        {
            //直接写SQL语句，没有参数
            var leagues = _dbContext.Leagues.FromSqlRaw("SELECT * FROM Leagues").ToList();
            return Ok(leagues);
        }
        [HttpGet("SqlTest1")]
        public IActionResult GetSqlTest1([FromQuery] string name)
        {
            //使用带参数的FromSqlInterpolated
            var leagues = _dbContext.Leagues
                .FromSqlInterpolated($"SELECT * FROM Leagues WHERE Name LIKE N'%{name}%'")
                .ToList();
            return Ok(leagues);
        }
        [HttpGet("SqlTest2")]
        public IActionResult GetSqlTest2()
        {
            //使用ExecuteSqlRaw
            var count = _dbContext.Database.ExecuteSqlRaw("EXEC [dbo].[RemoveGamePlayer] {0}", new Guid(""));
            //使用ExecuteSqlInterpolated
            var counts = _dbContext.Database.ExecuteSqlInterpolated($"EXEC [dbo].[RemoveGamePlayer] {new Guid("")}");
            return Ok(new { count, counts });
        }
        #endregion
    }
}