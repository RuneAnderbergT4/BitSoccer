using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace TeamName
{
    public class TeamName : ITeam
    {
        private static Team _lastTeam;

        private static float _goalPostTop = Field.MyGoal.Top.Y;
        private static float _goalPostBottom = Field.MyGoal.Bottom.Y;
        private static float _goalKeeperMaxX = Field.MyGoal.Center.X + 50;
        private static float _defenderMaxX = Field.MyGoal.Center.X + 150;

        public void Action(Team myTeam, Team enemyTeam, Ball ball, MatchInfo matchInfo)
        {
            var vel = Common.Constants.PlayerMaxVelocity;
            var bal = ball.Velocity;

            var temp1 = Field.Borders.Height/2;

            var temp = Field.Borders.Width / 2;

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
                            player.ActionShoot(Field.Borders.Top);
                            break;
                        case PlayerType.LeftDefender:
                            break;
                        case PlayerType.CenterForward:
                            break;
                        case PlayerType.RightDefender:
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
                        case PlayerType.RightForward:
                            break;
                        default:
                            player.ActionShootGoal();
                            break;
                    }
                }

                else if (player.CanPickUpBall(ball) && player.PlayerType == PlayerType.Keeper) // Picks up the ball if posible.
                {
                    player.ActionPickUpBall();
                }

                // Tackles any enemy that is close.
                else if ((player.CanTackle(closestEnemy) && player.PlayerType == PlayerType.LeftDefender) ||
                         (player.CanTackle(closestEnemy) && player.PlayerType == PlayerType.CenterForward) ||
                         (player.CanTackle(closestEnemy) && player.PlayerType == PlayerType.RightDefender))
                {
                    player.ActionTackle(closestEnemy);
                }

                else // If the player cannot do anything urgently usefull, move to a good position.
                {
                    //if (player == ball.GetClosest(myTeam))
                    //{
                    //    player.ActionGo(ball);
                    //}
                    
                    switch (player.PlayerType)
                    {
                        case PlayerType.Keeper:
                            if (Field.MyGoal.Bottom.Y > BallTrajectoryYPos(ball, Field.MyGoal.Left.X) && BallTrajectoryYPos(ball, Field.MyGoal.Left.X) > Field.MyGoal.Top.Y)
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
                            if (Field.MyGoal.Bottom.Y > BallTrajectoryYPos(ball, Field.MyGoal.Left.X) && BallTrajectoryYPos(ball, Field.MyGoal.Left.X) > Field.MyGoal.Top.Y
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
                            break;
                        case PlayerType.RightDefender:
                            if (Field.MyGoal.Bottom.Y> BallTrajectoryYPos(ball, Field.MyGoal.Left.X) && BallTrajectoryYPos(ball, Field.MyGoal.Left.X) > Field.MyGoal.Top.Y
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
                            break;
                        case PlayerType.CenterForward:
                            break;
                        case PlayerType.LeftForward:
                            break;
                        case PlayerType.RightForward:
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

        private IPosition GoaliePosition(Ball ball)
        {
            return new Vector(_goalKeeperMaxX, BallTrajectoryYPos(ball, _goalKeeperMaxX));
        }

        private IPosition DefenderPosition(Ball ball)
        {
            return new Vector(_defenderMaxX, BallTrajectoryYPos(ball, _defenderMaxX));
        }
    }
}
    
