using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Common;
using Microsoft.Win32;
using Vector = Common.Vector;

namespace TeamName
{
    public class TeamName : ITeam
    {
        private bool _myTeamLast = false;
        private float _goalPostTop = Field.MyGoal.Top.Y;
        private float _goalPostBottom = Field.MyGoal.Bottom.Y;
        private float _goalKeeperMaxX = Field.MyGoal.Center.X + 25;
        private float _defenderMaxX = Field.MyGoal.Center.X + 100;
        private List<Player> _freeForwards = new List<Player>();
        private List<Player> _freeDefenders = new List<Player>();
        private static Random _random = new Random();
        private List<IPosition> _openPositions = new List<IPosition>();
        private List<IPosition> _openForwardPositions = new List<IPosition>();
        private int _nrOfWidthBoxes = 20;
        private int _nrOfHeightBoxes = 20;

        public void Action(Team myTeam, Team enemyTeam, Ball ball, MatchInfo matchInfo)
        {
            if (ball.Owner != null)
            {
                if (ball.Owner.Team == enemyTeam)
                {
                    _myTeamLast = false;
                }
                else if (ball.Owner.Team == myTeam)
                {
                    _myTeamLast = true;
                }
            }

            if (ball.Position.X > 800 && _myTeamLast)
            {
                _defenderMaxX = Field.MyGoal.Center.X + 400;
            }
            else
            {
                _defenderMaxX = Field.MyGoal.Center.X + 100;
            }

            foreach (Player player in myTeam.Players)
            {
                Player closestEnemy = player.GetClosest(enemyTeam);

                if (ball.Owner == player)
                {
                    switch (player.PlayerType)
                    {
                        case PlayerType.Keeper:
                            UpdateFreePlayers(myTeam, enemyTeam, player, 75);

                            if (_freeDefenders.Count > 0)
                            {
                                player.ActionShoot(_freeDefenders[_random.Next(_freeDefenders.Count)]);
                            }
                            else if (_freeForwards.Count > 0)
                            {
                                player.ActionShoot(_freeForwards[_random.Next(_freeForwards.Count)]);
                            }
                            else
                            {
                                player.ActionShoot(Field.Borders.Bottom);
                            }
                            break;
                        case PlayerType.LeftDefender:
                            UpdateFreePlayers(myTeam, enemyTeam, player, 100);

                            if (_freeForwards.Count > 0)
                            {
                                player.ActionShoot(_freeForwards[_random.Next(_freeForwards.Count)]);
                            }
                            else if (_freeDefenders.Count > 0)
                            {
                                player.ActionShoot(_freeDefenders[_random.Next(_freeDefenders.Count)], 6);
                            }
                            else
                            {
                                player.ActionShoot(Field.Borders.Top);
                            }
                            break;
                        case PlayerType.RightDefender:
                            UpdateFreePlayers(myTeam, enemyTeam, player, 100);

                            if (_freeForwards.Count > 0)
                            {
                                player.ActionShoot(_freeForwards[_random.Next(_freeForwards.Count)]);
                            }
                            else if (_freeDefenders.Count > 0)
                            {
                                player.ActionShoot(_freeDefenders[_random.Next(_freeDefenders.Count)], 6);
                            }
                            else
                            {
                                player.ActionShoot(Field.Borders.Bottom);
                            }
                            break;
                        case PlayerType.LeftForward:
                            if (player.GetDistanceTo(Field.EnemyGoal) < 300)
                            {
                                // Finds enemy GK
                                var enemyTeamGk = enemyTeam.Players.Find(player1 => player1.PlayerType == PlayerType.Keeper);

                                // Checks GK distance to top & bottom of goal
                                var disBot = enemyTeamGk.GetDistanceTo(Field.EnemyGoal.Top);
                                var disTop = enemyTeamGk.GetDistanceTo(Field.EnemyGoal.Bottom);

                                // Shoot at the side of the goal the enemy GK is furthest away from
                                if (disTop >= disBot)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Top.Y - 40));
                                }
                                else if (disBot >= disTop)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Bottom.Y + 40));
                                }
                                else
                                {
                                    player.ActionShootGoal();
                                }
                            }
                            else if (player.GetDistanceTo(closestEnemy) < 100)
                            {
                                UpdateFreePlayers(myTeam, enemyTeam, player, 75);
                                if (_freeForwards.Count > 0)
                                {
                                    player.ActionShoot(_freeForwards[_random.Next(_freeForwards.Count)], 8);
                                }
                                else if (_freeDefenders.Count > 0)
                                {
                                    player.ActionShoot(_freeDefenders[_random.Next(_freeDefenders.Count)], 8);
                                }
                                else
                                {
                                    player.ActionShootGoal();
                                }
                            }
                            else
                            {
                                player.ActionGo(Field.EnemyGoal);
                            }
                            
                            break;
                        case PlayerType.CenterForward:
                            if (player.GetDistanceTo(Field.EnemyGoal) < 300)
                            {
                                // Finds enemy GK
                                var enemyTeamGk = enemyTeam.Players.Find(player1 => player1.PlayerType == PlayerType.Keeper);

                                // Checks GK distance to top & bottom of goal
                                var disBot = enemyTeamGk.GetDistanceTo(Field.EnemyGoal.Top);
                                var disTop = enemyTeamGk.GetDistanceTo(Field.EnemyGoal.Bottom);

                                // Shoot at the side of the goal the enemy GK is furthest away from
                                if (disTop >= disBot)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Top.Y - 40));
                                }
                                else if (disBot >= disTop)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Bottom.Y + 40));
                                }
                                else
                                {
                                    player.ActionShootGoal();
                                }
                            }
                            else if (player.GetDistanceTo(closestEnemy) < 100)
                            {
                                UpdateFreePlayers(myTeam, enemyTeam, player, 75);
                                if (_freeForwards.Count > 0)
                                {
                                    player.ActionShoot(_freeForwards[_random.Next(_freeForwards.Count)], 8);
                                }
                                else if (_freeDefenders.Count > 0)
                                {
                                    player.ActionShoot(_freeDefenders[_random.Next(_freeDefenders.Count)], 8);
                                }
                                else
                                {
                                    player.ActionShootGoal();
                                }
                            }
                            else
                            {
                                player.ActionGo(Field.EnemyGoal);
                            }
                            break;
                        case PlayerType.RightForward:
                            if (player.GetDistanceTo(Field.EnemyGoal) < 300)
                            {
                                // Finds enemy GK
                                var enemyTeamGk = enemyTeam.Players.Find(player1 => player1.PlayerType == PlayerType.Keeper);

                                // Checks GK distance to top & bottom of goal
                                var disBot = enemyTeamGk.GetDistanceTo(Field.EnemyGoal.Top);
                                var disTop = enemyTeamGk.GetDistanceTo(Field.EnemyGoal.Bottom);

                                // Shoot at the side of the goal the enemy GK is furthest away from
                                if (disTop >= disBot)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Top.Y - 40));
                                }
                                else if (disBot >= disTop)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Bottom.Y + 40));
                                }
                                else
                                {
                                    player.ActionShootGoal();
                                }
                            }
                            else if (player.GetDistanceTo(closestEnemy) < 100)
                            {
                                UpdateFreePlayers(myTeam, enemyTeam, player, 75);
                                if (_freeForwards.Count > 0)
                                {
                                    player.ActionShoot(_freeForwards[_random.Next(_freeForwards.Count)], 8);
                                }
                                else if (_freeDefenders.Count > 0)
                                {
                                    player.ActionShoot(_freeDefenders[_random.Next(_freeDefenders.Count)], 8);
                                }
                                else
                                {
                                    player.ActionShootGoal();
                                }
                            }
                            else
                            {
                                player.ActionGo(Field.EnemyGoal);
                            }
                            break;
                        default:
                            player.ActionShootGoal();
                            break;
                    }
                }

                else if (player.CanPickUpBall(ball)) // Picks up the ball if posible.
                {
                    player.ActionPickUpBall();
                }

                // Tackles any enemy that is close.
                else if (player.CanTackle(closestEnemy))
                {
                    player.ActionTackle(closestEnemy);
                }

                else // If the player cannot do anything urgently usefull, move to a good position.
                {
                    switch (player.PlayerType)
                    {
                        case PlayerType.Keeper:
                            if (Field.MyGoal.Bottom.Y > BallTrajectoryYPos(ball, Field.MyGoal.Left.X) && BallTrajectoryYPos(ball, Field.MyGoal.Left.X) > Field.MyGoal.Top.Y
                                && ball.Owner == null && ball.Position.X < 400)
                            {
                                player.ActionGo(GoaliePosition(ball));
                            }
                            else
                            {
                                // Do stuff
                                var gKy = (ball.Position.Y/Field.Borders.Bottom.Y)*
                                          (Field.MyGoal.Bottom.Y - Field.MyGoal.Top.Y) + Field.MyGoal.Top.Y;
                                
                                player.ActionGo(new Vector(_goalKeeperMaxX, gKy));
                            }
                            break;
                        case PlayerType.LeftDefender:
                            if (!_myTeamLast)
                            {
                                if (Field.MyGoal.Bottom.Y > BallTrajectoryYPos(ball, Field.MyGoal.Left.X) && BallTrajectoryYPos(ball, Field.MyGoal.Left.X) > Field.MyGoal.Top.Y
                                    && ball.Owner == null)
                                {
                                    // && player.GetDistanceTo(ball) < (myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.RightDefender).GetDistanceTo(ball)) 
                                    player.ActionGo(DefenderPosition(ball));
                                }
                                else if (player.GetDistanceTo(ball) < myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.RightDefender).GetDistanceTo(ball)
                                    && player.GetDistanceTo(ball) < ball.GetDistanceTo(ball.GetClosest(enemyTeam)))
                                {
                                    if (ball.Owner == null)
                                    {
                                        player.ActionGo(DefenderPosition(ball));
                                    }
                                    else
                                    {
                                        player.ActionGo(closestEnemy);
                                    }
                                }
                                else
                                {
                                    // Do stuff
                                    var dY = (ball.Position.Y / Field.Borders.Bottom.Y) *
                                              (Field.MyGoal.Bottom.Y - Field.MyGoal.Top.Y) + Field.MyGoal.Top.Y;

                                    player.ActionGo(new Vector(_defenderMaxX, dY - 100));
                                }
                            }
                            else
                            {
                                // Do stuff
                                var dY = (ball.Position.Y / Field.Borders.Bottom.Y) *
                                          (Field.MyGoal.Bottom.Y - Field.MyGoal.Top.Y) + Field.MyGoal.Top.Y;

                                player.ActionGo(new Vector(_defenderMaxX, dY - 100));
                            }
                            break;
                        case PlayerType.RightDefender:
                            if (!_myTeamLast)
                            {
                                if (Field.MyGoal.Bottom.Y > BallTrajectoryYPos(ball, Field.MyGoal.Left.X) && BallTrajectoryYPos(ball, Field.MyGoal.Left.X) > Field.MyGoal.Top.Y
                                    && ball.Owner == null)
                                {
                                    // && player.GetDistanceTo(ball) < (myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.LeftDefender).GetDistanceTo(ball)) 
                                    player.ActionGo(DefenderPosition(ball));
                                }
                                else if (player.GetDistanceTo(ball) <
                                    myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.LeftDefender).GetDistanceTo(ball)
                                    && player.GetDistanceTo(ball) < ball.GetDistanceTo(ball.GetClosest(enemyTeam)))
                                {
                                    if (ball.Owner == null)
                                    {
                                        player.ActionGo(DefenderPosition(ball));
                                    }
                                    else
                                    {
                                        player.ActionGo(closestEnemy);
                                    }
                                }
                                else
                                {
                                    // Do stuff
                                    var dY = (ball.Position.Y / Field.Borders.Bottom.Y) *
                                              (Field.MyGoal.Bottom.Y - Field.MyGoal.Top.Y) + Field.MyGoal.Top.Y;

                                    player.ActionGo(new Vector(_defenderMaxX, dY + 100));
                                }
                            }
                            else
                            {
                                // Do stuff
                                var dY = (ball.Position.Y / Field.Borders.Bottom.Y) *
                                          (Field.MyGoal.Bottom.Y - Field.MyGoal.Top.Y) + Field.MyGoal.Top.Y;

                                player.ActionGo(new Vector(_defenderMaxX, dY + 100));
                            }
                            break;
                        case PlayerType.LeftForward:
                            if (!_myTeamLast && player == ball.GetClosest(myTeam))
                            {
                                var bY = BallTrajectoryYPos(ball, ball.Position.X + ball.Velocity.X);
                                player.ActionGo(new Vector(ball.Position.X + ball.Velocity.X, bY));
                            }
                            else
                            {
                                // Get into position (kinda)
                                var pos = new Vector(myTeam.Players[4].Position.X + 200, myTeam.Players[4].Position.Y - 200);
                                player.ActionGo(pos);
                            }
                            break;
                        case PlayerType.CenterForward:
                            if (!_myTeamLast && player == ball.GetClosest(myTeam))
                            {
                                var bY = BallTrajectoryYPos(ball, ball.Position.X + ball.Velocity.X);
                                player.ActionGo(new Vector(ball.Position.X + ball.Velocity.X, bY));
                            }
                            else
                            {
                                // Get into position (kinda)
                                var bY = BallTrajectoryYPos(ball, ball.Position.X + ball.Velocity.X);
                                player.ActionGo(new Vector(ball.Position.X + ball.Velocity.X, bY));
                            }
                            break;
                        case PlayerType.RightForward:
                            if (!_myTeamLast && player == ball.GetClosest(myTeam))
                            {
                                var bY = BallTrajectoryYPos(ball, ball.Position.X + ball.Velocity.X);
                                player.ActionGo(new Vector(ball.Position.X + ball.Velocity.X, bY));
                            }
                            else
                            {
                                // Get into position (kinda)
                                var pos = new Vector(myTeam.Players[4].Position.X + 200, myTeam.Players[4].Position.Y + 200);
                                player.ActionGo(pos);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //private IPosition randomForwardPos(Team enemyTeam)
        //{
        //    UpdateOpenPositions(enemyTeam);
        //    UpdateOpenForwardPositions();
        //    return _openForwardPositions[_random.Next(_openForwardPositions.Count)];
        //}

        //private void UpdateOpenPositions(Team enemyTeam)
        //{
        //    _openPositions.Clear();

        //    var width = Field.Borders.Width / _nrOfWidthBoxes;
        //    var height = Field.Borders.Height / _nrOfHeightBoxes;

        //    float currentWidth = 0;
        //    float currentHeight = 0;

        //    for (int x = 0; x < _nrOfWidthBoxes; x++)
        //    {
        //        for (int y = 0; y < _nrOfHeightBoxes; y++)
        //        {
        //            if (!AreaContainsPlayer(enemyTeam, currentWidth, currentWidth + width, currentHeight, currentHeight + height))
        //            {
        //                _openPositions.Add(new Vector(currentWidth + width / 2, currentHeight + height / 2));
        //            }
        //            currentHeight += height;
        //        }
        //        currentWidth += width;
        //    }
        //}

        //private bool AreaContainsPlayer(Team enemyTeam, float xMin, float xMax, float yMin, float yMax)
        //{
        //    foreach (var player in enemyTeam.Players)
        //    {
        //        if ((player.Position.X >= xMin && player.Position.X <= xMax) && (player.Position.Y >= yMin && player.Position.Y <= yMax))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private void UpdateOpenForwardPositions()
        //{
        //    _openForwardPositions.Clear();

        //    foreach (var pos in _openPositions)
        //    {
        //        if (pos.Position.X > 500)
        //        {
        //            _openForwardPositions.Add(pos);
        //        }
        //    }
        //}

        private void UpdateFreePlayers(Team myTeam, Team enemyTeam, Player player, float radius)
        {
            _freeForwards.Clear();
            _freeDefenders.Clear();
            
            foreach (var playerTarget in myTeam.Players)
            {
                if (CheckIfPlayerIsFree(player, playerTarget, enemyTeam, radius))
                {
                    switch (playerTarget.PlayerType)
                    {
                        case PlayerType.LeftForward:
                        case PlayerType.CenterForward:
                        case PlayerType.RightForward:
                            _freeForwards.Add(playerTarget);
                            break;
                        case PlayerType.LeftDefender:
                        case PlayerType.RightDefender:
                            _freeDefenders.Add(playerTarget);
                            break;
                        case PlayerType.Keeper:
                            break;
                    }
                }
            }
        }

        private float BallTrajectoryYPos(Ball ball, float x)
        {
            if (ball.Velocity.X == 0)
            {
                return ball.Position.Y;
            }
            return (ball.Velocity.Y / ball.Velocity.X)*(x-ball.Position.X) + ball.Position.Y;
        }

        private float EnemyTrajectoryYPos(Player enemy, float x)
        {
            if (enemy.Velocity.X == 0)
            {
                return enemy.Position.Y;
            }
            return (enemy.Velocity.Y/enemy.Velocity.X)*(x - enemy.Position.X) + enemy.Position.Y;
        }

        private IPosition GoaliePosition(Ball ball)
        {
            return new Vector(_goalKeeperMaxX, BallTrajectoryYPos(ball, _goalKeeperMaxX));
        }

        private IPosition DefenderPosition(Ball ball)
        {
            return new Vector(_defenderMaxX, BallTrajectoryYPos(ball, _defenderMaxX));
        }

        private bool CheckIfPlayerIsFree(Player myPlayer, Player targetPlayer, Team enemyTeam, float radius)
        {
            if (myPlayer == targetPlayer)
            {
                return false;
            }

            Point myPoint = new Point(myPlayer.Position.X, myPlayer.Position.Y);
            Point targetPoint = new Point(targetPlayer.Position.X, targetPlayer.Position.Y);
            
            //var side = radius/Math.Pow(2, 0.5);

            var isFree = true;

            foreach (var player in enemyTeam.Players)
            {
                //var enemyX = player.Position.X;
                //var enemyY = player.Position.Y;

                ////var octagon = new Polygon();
                ////octagon.Points.Add(new Point(enemyX, enemyY - radius));
                ////octagon.Points.Add(new Point(enemyX - side, enemyY - side));
                ////octagon.Points.Add(new Point(enemyX - radius, enemyY));
                ////octagon.Points.Add(new Point(enemyX - side, enemyY + side));
                ////octagon.Points.Add(new Point(enemyX, enemyY + radius));
                ////octagon.Points.Add(new Point(enemyX + side, enemyY + side));
                ////octagon.Points.Add(new Point(enemyX + radius, enemyY));
                ////octagon.Points.Add(new Point(enemyX + side, enemyY - side));

                //List<Point> octagon = new List<Point>
                //{
                //    new Point(enemyX, enemyY - radius),
                //    new Point(enemyX - side, enemyY - side),
                //    new Point(enemyX - radius, enemyY),
                //    new Point(enemyX - side, enemyY + side),
                //    new Point(enemyX, enemyY + radius),
                //    new Point(enemyX + side, enemyY + side),
                //    new Point(enemyX + radius, enemyY),
                //    new Point(enemyX + side, enemyY - side)
                //};

                if (CheckIntersection(new Point(player.Position.X, player.Position.Y), radius, myPoint, targetPoint))
                {
                    isFree = false;
                }
            }
            return isFree;
        }

        private bool CheckIntersection(Point circlePos, float circleRadius, Point point1, Point point2)
        {
            float dx, dy, a, b, c, det;

            float cx = (float) circlePos.X;
            float cy = (float) circlePos.Y;
            
            dx = (float) (point2.X - point1.X);
            dy = (float) (point2.Y - point1.Y);

            a = dx * dx + dy * dy;
            b = (float) (2 * (dx * (point1.X - cx) + dy * (point1.Y - cy)));
            c = (float) ((point1.X - cx) * (point1.X - cx) + (point1.Y - cy) * (point1.Y - cy) - circleRadius * circleRadius);

            det = b * b - 4 * a * c;
            if ((a <= 0.0000001) || (det < 0))
            {
                // No intersections.
                return false;
            }
            else
            {
                // Intersections.
                return true;
            }
        }

        //private bool CheckIntersection(Point point1, Point point2, IEnumerable<Point> octagon)
        //{
        //    if (octagon == null || !octagon.Any()) return false;
        //    var side = GetSide(point1, point2, octagon.First());
        //    return
        //        side == 0
        //            ? true
        //            : octagon.All(x => GetSide(point1, point2, x) == side);
        //}

        //public int GetSide(Point lineP1, Point lineP2, Point queryP)
        //{
        //    return Math.Sign((lineP2.X - lineP1.X) * (queryP.Y - lineP1.Y) - (lineP2.Y - lineP1.Y) * (queryP.X - lineP1.X));
        //}

        //public static bool IsOutside(Point lineP1, Point lineP2, IEnumerable<Point> region)
        //{
        //    if (region == null || !region.Any()) return true;
        //    var side = GetSide(lineP1, lineP2, region.First());
        //    return
        //      side == 0
        //      ? false
        //      : region.All(x => GetSide(lineP1, lineP2, x) == side);
        //}
    }
}
    
