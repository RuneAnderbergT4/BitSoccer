using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Common;

namespace TeamName
{
    public class TeamName : ITeam
    {
        private static Team _lastTeam;

        private static float _goalPostTop = Field.MyGoal.Top.Y;
        private static float _goalPostBottom = Field.MyGoal.Bottom.Y;
        private static float _goalKeeperMaxX = Field.MyGoal.Center.X + 20;
        private static float _defenderMaxX = Field.MyGoal.Center.X + 120;

        public void Action(Team myTeam, Team enemyTeam, Ball ball, MatchInfo matchInfo)
        {
            if (ball.Position.X > 800 && _lastTeam == myTeam)
            {
                _defenderMaxX = 240;
            }
            else
            {
                _defenderMaxX = 120;
            }

            if (ball.Owner != null)
            {
                _lastTeam = ball.Owner.Team == myTeam ? myTeam : enemyTeam;
            }
            
            foreach (Player player in myTeam.Players)
            {
                Player closestEnemy = player.GetClosest(enemyTeam);

                if (ball.Owner == player)
                {

                    switch (player.PlayerType)
                    {
                        case PlayerType.Keeper:
                            player.ActionShootGoal();
                            break;
                        case PlayerType.LeftDefender:
                            player.ActionShoot(Field.Borders.Top);
                            break;
                        case PlayerType.RightDefender:
                            player.ActionShoot(Field.Borders.Bottom);
                            break;
                        case PlayerType.LeftForward:
                            if (player.GetDistanceTo(Field.EnemyGoal) < 300)
                            {
                                // Finds enemy GK
                                var enemyTeamGK = enemyTeam.Players.Find(player1 => player1.PlayerType == PlayerType.Keeper);

                                // Checks GK distance to top & bottom of goal
                                var disBot = enemyTeamGK.GetDistanceTo(Field.EnemyGoal.Top);
                                var disTop = enemyTeamGK.GetDistanceTo(Field.EnemyGoal.Bottom);

                                // Shoot at the side of the goal the enemy GK is furthest away from
                                if (disTop >= disBot)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Top.Y - 20));
                                }
                                else if (disBot >= disTop)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Bottom.Y + 20));
                                }
                                else
                                {
                                    player.ActionShootGoal();
                                }
                            }
                            else if (player.GetDistanceTo(closestEnemy) < 100)
                            {
                                player.ActionShoot(player.GetClosest(myTeam), 6);
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
                                var enemyTeamGK = enemyTeam.Players.Find(player1 => player1.PlayerType == PlayerType.Keeper);

                                // Checks GK distance to top & bottom of goal
                                var disBot = enemyTeamGK.GetDistanceTo(Field.EnemyGoal.Top);
                                var disTop = enemyTeamGK.GetDistanceTo(Field.EnemyGoal.Bottom);

                                // Shoot at the side of the goal the enemy GK is furthest away from
                                if (disTop >= disBot)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Top.Y - 20));
                                }
                                else if (disBot >= disTop)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Bottom.Y + 20));
                                }
                                else
                                {
                                    player.ActionShootGoal();
                                }
                            }
                            else if (player.GetDistanceTo(closestEnemy) < 100)
                            {
                                player.ActionShoot(player.GetClosest(myTeam), 6);
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
                                var enemyTeamGK = enemyTeam.Players.Find(player1 => player1.PlayerType == PlayerType.Keeper);

                                // Checks GK distance to top & bottom of goal
                                var disBot = enemyTeamGK.GetDistanceTo(Field.EnemyGoal.Top);
                                var disTop = enemyTeamGK.GetDistanceTo(Field.EnemyGoal.Bottom);

                                // Shoot at the side of the goal the enemy GK is furthest away from
                                if (disTop >= disBot)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Top.Y - 20));
                                }
                                else if (disBot >= disTop)
                                {
                                    player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Bottom.Y + 20));
                                }
                                else
                                {
                                    player.ActionShootGoal();
                                }
                            }
                            else if (player.GetDistanceTo(closestEnemy) < 100)
                            {
                                player.ActionShoot(player.GetClosest(myTeam), 6);
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
                    if (player.PlayerType == PlayerType.Keeper || player.PlayerType == PlayerType.LeftDefender || player.PlayerType == PlayerType.RightDefender)
                    {
                        player.ActionPickUpBall();
                    }
                    
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
                                && _lastTeam == enemyTeam && ball.Position.X < Field.Borders.Width / 2)
                            {
                                player.ActionGo(GoaliePosition(ball));
                            }
                            else
                            {
                                // Do stuff
                                var gKY = (ball.Position.Y/Field.Borders.Bottom.Y)*
                                          (Field.MyGoal.Bottom.Y - Field.MyGoal.Top.Y) + Field.MyGoal.Top.Y;
                                
                                player.ActionGo(new Vector(_goalKeeperMaxX, gKY));
                            }
                            break;
                        case PlayerType.LeftDefender:
                            if (_lastTeam == enemyTeam)
                            {
                                if (player.GetDistanceTo(ball) < ball.GetDistanceTo(closestEnemy)
                                && player.GetDistanceTo(ball) < (myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.RightDefender).GetDistanceTo(ball)))
                                {
                                    player.ActionGo(ball);
                                }
                                else if (Field.MyGoal.Bottom.Y > BallTrajectoryYPos(ball, Field.MyGoal.Left.X) && BallTrajectoryYPos(ball, Field.MyGoal.Left.X) > Field.MyGoal.Top.Y
                                    && player.GetDistanceTo(ball) < (myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.RightDefender).GetDistanceTo(ball)))
                                {
                                    player.ActionGo(DefenderPosition(ball));
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
                            if (_lastTeam == enemyTeam)
                            {
                                if (player.GetDistanceTo(ball) < ball.GetDistanceTo(closestEnemy)
                                && player.GetDistanceTo(ball) < (myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.LeftDefender).GetDistanceTo(ball)))
                                {
                                    player.ActionGo(ball);
                                }
                                else if (Field.MyGoal.Bottom.Y > BallTrajectoryYPos(ball, Field.MyGoal.Left.X) && BallTrajectoryYPos(ball, Field.MyGoal.Left.X) > Field.MyGoal.Top.Y
                                    && player.GetDistanceTo(ball) < (myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.LeftDefender).GetDistanceTo(ball)))
                                {
                                    player.ActionGo(DefenderPosition(ball));
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
                            if (player == ball.GetClosest(myTeam))
                            {
                                player.ActionGo(ball);
                            }
                            else
                            {
                                // Get into position without nearby enemies
                                player.ActionGo(Field.EnemyGoal);
                            }
                            break;
                        case PlayerType.CenterForward:
                            if (player == ball.GetClosest(myTeam))
                            {
                                player.ActionGo(ball);
                            }
                            else
                            {
                                // Get into position without nearby enemies
                                player.ActionGo(Field.EnemyGoal);
                            }
                            break;
                        case PlayerType.RightForward:
                            if (player == ball.GetClosest(myTeam))
                            {
                                player.ActionGo(ball);
                            }
                            else
                            {
                                // Get into position without nearby enemies
                                player.ActionGo(Field.EnemyGoal);
                            }
                            break;
                        default:
                            player.ActionShootGoal();
                            break;
                    }
                }
            }
        }

        private float BallTrajectoryYPos(Ball ball, float x)
        {
            return (ball.Velocity.Y / ball.Velocity.X)*(x-ball.Position.X) + ball.Position.Y;
        }

        private float EnemyTrajectoryYPos(Player enemy, float x)
        {
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

        private bool CheckIntersection(Player myPlayer, Player targetPlayer, Team enemyTeam)
        {
            var line = new Line();
            line.X1 = myPlayer.Position.X;
            line.Y1 = myPlayer.Position.Y;
            line.X2 = targetPlayer.Position.X;
            line.Y2 = targetPlayer.Position.Y;

            var octagon = new Polygon();
            octagon.

            foreach (var player in enemyTeam.Players)
            {
                
            }

            return true;
        }
    }
}
    
