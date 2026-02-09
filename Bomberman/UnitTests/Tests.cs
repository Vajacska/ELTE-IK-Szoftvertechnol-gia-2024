using Model;
using Moq;
using Persistence;
using Persistence.Enums;
using Persistence.GameObjects;
using Persistence.Monsters;
using Persistence.Structures;
using System.Drawing;

namespace Tests
{
    [TestClass]
    public class Tests
    {
        private Map? map1;
        private Map? map2;
        private Map? map3;
        private Map? map4;
        private List<Player>? players;
        private int numberOfWins;
        private MapType mapType1;
        private MapType mapType2;
        private MapType mapType3;
        private MapType tutorial;
        private GameModel? gameModel;
        private int gameTime;
        private List<Monster>? monsters;
        private Monster? monster;
        private DangerZone? _dangerZone;


        [TestInitialize]
        public void Initialize()
        {
            players = new List<Player>() { new Player(1), new Player(2) };
            numberOfWins = 1;
            mapType1 = MapType.TYPE1;
            mapType2 = MapType.TYPE2;
            mapType3 = MapType.TYPE3;
            tutorial = MapType.TUTORIAL;
            var mock1 = new Mock<Map>(players.Count(), mapType1);
            var mock2 = new Mock<Map>(players.Count(), mapType2);
            var mock3 = new Mock<Map>(players.Count(), mapType3);
            var mock4 = new Mock<Map>(players.Count(), tutorial);
            map1 = mock1.Object;
            map2 = mock2.Object;
            map3 = mock3.Object;
            map4 = mock4.Object;
            gameTime = 180;
            monsters = new List<Monster>();
        }


        [TestMethod]
        public void ConstructorTest()
        {
            // Arrage

            // Act
            gameModel = new GameModel(map1, players, numberOfWins);
            var gameTime2 = gameModel.GameTime;
            gameModel.IsGameOver = false;
            var isGameOver = gameModel.IsGameOver;
            gameModel.NumberOfWins = numberOfWins;
            var numberOfWins2 = gameModel.NumberOfWins;


            // Assert
            Assert.AreEqual(gameModel.Map, map1);
            Assert.AreEqual(gameModel.NumOfPlayers, players?.Count);
            Assert.AreEqual(gameModel.Players, players);
            Assert.AreEqual(gameModel.Monsters.Count, monsters?.Count);

            Assert.AreEqual(gameTime2, gameTime);
            Assert.IsFalse(isGameOver);
            Assert.AreEqual(numberOfWins2, numberOfWins);
        }

        [TestMethod]
        public void NewGameMap1Test()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(1, 1), typeof(Player)));
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(1, 11), typeof(Player)));
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if ((i == 0 || i == 12 || j == 0 || j == 12) || i % 2 == 0 && j % 2 == 0)
                    {
                        Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(i, j), typeof(Wall)));
                    }
                    else if (i % 3 == 0 && j % 3 == 0)
                    {
                        Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(i, j), typeof(Box)));
                    }
                }
            }
        }

        [TestMethod]
        public void NewGameMap2Test()
        {
            gameModel = new GameModel(map2, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(1, 1), typeof(Player)));
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(1, 11), typeof(Player)));

            for (int i = 2; i < 6; i++)
            {
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(2, i), typeof(Wall)));
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(10, i), typeof(Wall)));
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(2, 12 - i), typeof(Wall)));
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(10, 12 - i), typeof(Wall)));
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(i, 2), typeof(Wall)));
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(i, 10), typeof(Wall)));
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(12 - i), typeof(Wall)));
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(12 - i, 10), typeof(Wall)));
            }

            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(5, 5), typeof(Wall)));
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(5, 9), typeof(Wall)));
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(9, 5), typeof(Wall)));
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(9, 9), typeof(Wall)));
        }
        [TestMethod]
        public void NewGameMap3Test()
        {
            gameModel = new GameModel(map3, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(1, 1), typeof(Player)));
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(1, 11), typeof(Player)));
            for (int i = 2; i < 11; i++)
            {
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(2, i), typeof(Wall)));
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(10, i), typeof(Wall)));
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(i, 10), typeof(Wall)));
            }

            for (int i = 5; i < 8; i++)
            {
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(5, i), typeof(Wall)));
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(7, i), typeof(Wall)));
                Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(i, 7), typeof(Wall)));
            }
        }

        [TestMethod]
        public void NewGameTestMap()
        {
            gameModel = new GameModel(map4, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(1, 1), typeof(Player)));
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(1, 11), typeof(Player)));

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if ((i == 0 || i == 12 || j == 0 || j == 12) || (i == 5 && j < 10 && j > 3) || (j == 5 && i > 5 && i < 12))
                    {
                        Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(i, j), typeof(Wall)));
                    }
                }
            }
        }

        [TestMethod]
        public void StepPlayerDownTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            gameModel.StartTimer();
            gameModel.StepPlayer(players?[0], Direction.DOWN);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(2, 1), typeof(Player)));
            gameModel.StepPlayer(players?[1], Direction.DOWN);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(2, 11), typeof(Player)));
        }

        [TestMethod]
        public void StepPlayerRightAndLeftTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            gameModel.StartTimer();
            gameModel.StepPlayer(players?[0], Direction.RIGHT);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(1, 2), typeof(Player)));
            gameModel.StepPlayer(players?[1], Direction.LEFT);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(1, 10), typeof(Player)));
        }

        [TestMethod]
        public void StepPlayerUpTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            gameModel.Map?.SetGameObject(new System.Drawing.Point(2, 1), new Player(1));
            gameModel.Map?.SetGameObject(new System.Drawing.Point(2, 11), new Player(2));
            gameModel.StartTimer();
            gameModel.StepPlayer(players?[0], Direction.UP);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(1, 1), typeof(Player)));
            gameModel.StepPlayer(players?[1], Direction.UP);
            Assert.IsTrue(gameModel?.Map.CheckGameObjectType(new System.Drawing.Point(1, 11), typeof(Player)));
        }

        [TestMethod]
        public void PlayerTriesToStepOnAnotherPlayer()
        {
            gameModel = new GameModel(map4, players, numberOfWins);
            players?.Add(new Player(8));
            gameModel.NewGame(players.Count);
            gameModel.Map?.SetGameObject(new Point(1,10), new Player(8));
            gameModel.StepPlayer(players[players.Count-1], Direction.RIGHT);

            Assert.IsTrue(gameModel.Map?.FindPlayer(players[players.Count - 1]) == new Point(1, 10));
        }

        [TestMethod]
        public void PlayerStepsOnAMonster()
        {
            
        }

        [TestMethod]
        public void SetMonstersTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            gameModel.SetMonster(new Point(11, 5));
            Assert.IsTrue(gameModel.Map.CheckGameObjectType(new System.Drawing.Point(11, 5), typeof(Default)) ||
            gameModel.Map.CheckGameObjectType(new System.Drawing.Point(11, 5), typeof(Ghost)) ||
            gameModel.Map.CheckGameObjectType(new System.Drawing.Point(11, 5), typeof(FrankEinstein)) ||
            gameModel.Map.CheckGameObjectType(new System.Drawing.Point(11, 5), typeof(Noob)));
        }

        [TestMethod]
        public void SetDefaultMonsterTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            monster = new Default(1, new Point(11, 5));
            gameModel.SetMonster(monster);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(11, 5), typeof(Default)));
        }

        [TestMethod]
        public void SetGhostMonsterTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            monster = new Ghost(1, new Point(11, 5));
            gameModel.SetMonster(monster);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(11, 5), typeof(Ghost)));
        }

        [TestMethod]
        public void SetFrankEinsteinMonsterTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            monster = new FrankEinstein(1, new Point(11, 5));
            gameModel.SetMonster(monster);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(11, 5), typeof(FrankEinstein)));
        }

        [TestMethod]
        public void SetNoobMonsterTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players?.Count);
            monster = new Noob(1, new Point(11, 5));
            gameModel.SetMonster(monster);
            Assert.IsTrue(gameModel.Map?.CheckGameObjectType(new System.Drawing.Point(11, 5), typeof(Noob)));
        }

        [TestMethod]
        public void TimerTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players.Count);
            gameModel.StartTimer();
            Assert.IsTrue(gameModel.IsTimerEnabled());
            gameModel.StopTimer();
            Assert.IsFalse(gameModel.IsTimerEnabled());
        }

        [TestMethod]
        public void SetBombTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players.Count);
            gameModel.StartTimer();
            gameModel.SetBomb(players[0]);
            Assert.IsTrue(gameModel.Map.CheckGameObjectType(new System.Drawing.Point(1, 1), typeof(Bomb)));
        }

        [TestMethod]
        public void ExpandDangerZoneTest()
        {
            var player = new Player(1);
            var players2 = new List<Player>() { player };
            gameModel = new GameModel(map1, new List<Player>(players2), numberOfWins);
            map1.SetGameObject(new Point(5, 5), player);
            gameModel.StartTimer();
            gameModel.SetBomb(players2[0]);

            Point startPosition = new Point(5, 5);
            _dangerZone = new DangerZone(startPosition, 1, true);

            Thread.Sleep(6000);

            Assert.IsTrue(map1.CheckGameObjectType(new Point(5, 5), typeof(DangerZone)));
            Assert.IsTrue(map1.CheckGameObjectType(new Point(5, 6), typeof(DangerZone)));
            Assert.IsTrue(map1.CheckGameObjectType(new Point(5, 7), typeof(DangerZone)));
            Assert.IsTrue(map1.CheckGameObjectType(new Point(5, 4), typeof(DangerZone)));
            Assert.IsTrue(map1.CheckGameObjectType(new Point(5, 3), typeof(DangerZone)));
            Assert.IsTrue(map1.CheckGameObjectType(new Point(6, 5), typeof(DangerZone)));
            Assert.IsTrue(map1.CheckGameObjectType(new Point(7, 5), typeof(DangerZone)));
            Assert.IsTrue(map1.CheckGameObjectType(new Point(4, 5), typeof(DangerZone)));
            Assert.IsTrue(map1.CheckGameObjectType(new Point(3, 5), typeof(DangerZone)));
        }

        [TestMethod]
        public void ExpandDangerZoneOnPlayer1()
        {
            var player1 = new Player(1);
            var player2 = new Player(2);
            var player3 = new Player(3);
            var bomb = new Bomb(2, new Point(5, 6), player1);
            var players2 = new List<Player>() { player1, player2, player3 };
            gameModel = new GameModel(map1, new List<Player>(players2), numberOfWins);
            map1.SetGameObject(new Point(5, 5), player1);
            map1.SetGameObject(new Point(5, 6), player2);
            map1.SetGameObject(new Point(5, 4), player3);
            gameModel.StartTimer();
            gameModel.SetBomb(players2[0]);

            Point startPosition = new Point(5, 5);
            _dangerZone = new DangerZone(startPosition, 1, true);

            Thread.Sleep(6000);

            Assert.IsFalse(player2.IsAlive);
            Assert.IsFalse(player3.IsAlive);
        }

        [TestMethod]
        public void ExpandDangerZoneOnPlayer2()
        {
            var player1 = new Player(1);
            var player2 = new Player(2);
            var player3 = new Player(3);
            var bomb = new Bomb(2, new Point(5, 6), player1);
            var players2 = new List<Player>() { player1, player2, player3 };
            gameModel = new GameModel(map1, new List<Player>(players2), numberOfWins);
            map1.SetGameObject(new Point(5, 5), player1);
            map1.SetGameObject(new Point(6, 5), player2);
            map1.SetGameObject(new Point(4, 5), player3);
            gameModel.StartTimer();
            gameModel.SetBomb(players2[0]);

            Point startPosition = new Point(5, 5);
            _dangerZone = new DangerZone(startPosition, 1, true);

            Thread.Sleep(6000);

            Assert.IsFalse(player2.IsAlive);
            Assert.IsFalse(player3.IsAlive);
        }

        [TestMethod]
        public void StepMonsterDefault1()
        {
            var defMonster = new Default(1, new Point(5, 5));
            var player = new Player(1);
            var players2 = new List<Player>() { player };
            var eventRaised = false;
            gameModel = new GameModel(map1, new List<Player>(players2), numberOfWins);
            map1.SetGameObject(new Point(5, 5), defMonster);
            defMonster.MonsterStep += (sender, e) =>
            {
                eventRaised = true;
            };
            gameModel.StartTimer();
            defMonster.StartMoving();

            Thread.Sleep(2000);

            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void StepMonsterDefault2()
        {
            var defMonster = new Default(1, new Point(11, 11));
            var player = new Player(1);
            var players2 = new List<Player>() { player };
            var eventRaised = false;
            gameModel = new GameModel(map1, new List<Player>(players2), numberOfWins);
            map1.SetGameObject(new Point(11, 11), defMonster);
            defMonster.MonsterStep += (sender, e) =>
            {
                eventRaised = true;
            };
            gameModel.StartTimer();
            defMonster.StartMoving();

            Thread.Sleep(2000);

            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void StepMonsterGhostSimple()
        {
            var ghost = new Ghost(1, new Point(4, 1));
            gameModel = new GameModel(map4, players, 1);
            gameModel.SetMonster(ghost);
            ghost.MonsterStep += (sender, e) =>
            {
                ghost.StopMoving();
                Assert.AreEqual(ghost.Coord, new Point(5, 1));
            };
            gameModel.StartTimer();
            ghost.StartMoving();
        }

        [TestMethod]
        public void GhostGoesThroughTheWall()
        {
            var ghost = new Ghost(1, new Point(1, 3));
            gameModel = new GameModel(map4, players, 1);
            gameModel.SetMonster(ghost);
            for (int i = 2; i < 7; i++)
            {
                gameModel.Map.SetGameObject(new Point(i, 3), new Wall());
            }
            ghost.MonsterStep += (sender, e) =>
            {
                ghost.StopMoving();
                Assert.AreEqual(ghost.Coord, new Point(7, 3));
            };
            gameModel.StartTimer();
            ghost.StartMoving();
        }

        [TestMethod]
        public void GameTimeTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            Assert.IsTrue(gameModel.GameTime == 180);
        }

        [TestMethod]
        public void StepMonsterFrankEinstein1()
        {
            var frank = new FrankEinstein(1, new Point(1, 5));
            var player = new Player(1);
            var players2 = new List<Player>() { player };
            var eventRaised = false;
            gameModel = new GameModel(map1, new List<Player>(players2), numberOfWins);
            map1.SetGameObject(new Point(1, 5), frank);
            map1.SetGameObject(new Point(1, 1), player);
            frank.MonsterStep += (sender, e) =>
            {
                eventRaised = true;
            };
            gameModel.StartTimer();
            frank.StartMoving();

            Thread.Sleep(2000);

            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void StepMonsterFrankEinstein2()
        {
            var frank = new FrankEinstein(1, new Point(11, 11));
            var player = new Player(1);
            var players2 = new List<Player>() { player };
            var eventRaised = false;
            gameModel = new GameModel(map1, new List<Player>(players2), numberOfWins);
            map1.SetGameObject(new Point(11, 11), frank);
            map1.SetGameObject(new Point(1, 1), player);
            frank.MonsterStep += (sender, e) =>
            {
                eventRaised = true;
            };
            gameModel.StartTimer();
            frank.StartMoving();

            Thread.Sleep(2000);

            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void StepMonsterNoob1()
        {
            var noob = new Noob(1, new Point(5, 5));
            var player = new Player(1);
            var players2 = new List<Player>() { player };
            var eventRaised = false;
            gameModel = new GameModel(map1, new List<Player>(players2), numberOfWins);
            map1.SetGameObject(new Point(5, 5), noob);
            map1.SetGameObject(new Point(1, 1), player);
            noob.MonsterStep += (sender, e) =>
            {
                eventRaised = true;
            };
            gameModel.StartTimer();
            noob.StartMoving();

            Thread.Sleep(2000);

            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void StepMonsterNoob2()
        {
            var noob = new Noob(1, new Point(11, 11));
            var player = new Player(1);
            var players2 = new List<Player>() { player };
            var eventRaised = false;
            gameModel = new GameModel(map1, new List<Player>(players2), numberOfWins);
            map1.SetGameObject(new Point(11, 11), noob);
            map1.SetGameObject(new Point(1, 1), player);
            noob.MonsterStep += (sender, e) =>
            {
                eventRaised = true;
            };
            gameModel.StartTimer();
            noob.StartMoving();

            Thread.Sleep(2000);

            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void OnGameAdvancedTest()
        {
            var gameAdvancedRaised = false;
            gameModel = new GameModel(map3, players, numberOfWins);
            gameModel.NewGame(players.Count);
            gameModel.GameAdvanced += (sender, e) =>
            {
                gameAdvancedRaised = true;
            };
            gameModel.StartTimer();

            Thread.Sleep(1000);

            Assert.IsTrue(gameAdvancedRaised);
        }

        [TestMethod]
        public void OnRoundOverTestWithOneDeathAndTwoPlayers()
        {
            var roundOverRaised = false;
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players.Count);
            gameModel.StartTimer();
            gameModel.RoundOver += (sender, e) =>
            {
                roundOverRaised = true;
            };

            players[0].IsAlive = false;
            gameModel.StartTimer2();

            Thread.Sleep(4500);

            Assert.IsTrue(roundOverRaised);
        }

        [TestMethod]
        public void OnRoundOverTestWithTwoDeathsAndTwoPlayers()
        {
            var roundOverRaised = false;
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players.Count);
            gameModel.StartTimer();
            gameModel.RoundOver += (sender, e) =>
            {
                roundOverRaised = true;
            };

            players[0].IsAlive = false;
            players[1].IsAlive = false;
            gameModel.StartTimer2();

            Thread.Sleep(4500);

            Assert.IsTrue(roundOverRaised);
        }

        [TestMethod]
        public void OnRoundOverTestWithThreeDeathsAndThreePlayers()
        {
            var roundOverRaised = false;
            players.Add(new Player(3));
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players.Count);
            gameModel.StartTimer();
            gameModel.RoundOver += (sender, e) =>
            {
                roundOverRaised = true;
            };

            players[0].IsAlive = false;
            players[1].IsAlive = false;
            players[2].IsAlive = false;
            gameModel.StartTimer2();

            Thread.Sleep(4500);

            Assert.IsTrue(roundOverRaised);
        }

        
        /*
        [TestMethod]
        public void PlaceBoxTest()
        {
            gameModel = new GameModel(map4, players, numberOfWins);
            gameModel.NewGame(players.Count);
            gameModel.PlaceBox(players[0]);
            Point coord = gameModel.Map.FindPlayer(players[0]);
            Assert.IsFalse(gameModel.Map.CheckGameObjectType(coord, typeof(Box)));
        }
        */


        [TestMethod]
        public void BattleRoyalTest()
        {
            gameModel = new GameModel(map1, players, numberOfWins);
            gameModel.NewGame(players.Count);
            gameModel.GameTime = 0;
            gameModel.StartTimer();

            Thread.Sleep(1000);

            Assert.AreEqual(120, gameModel.GameTime);
        }
    }
}