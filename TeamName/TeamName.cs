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
        public void Action(Team myTeam, Team enemyTeam, Ball ball, MatchInfo matchInfo)
        {
            if (ball.Owner != null)
            {
                _lastTeam = ball.Owner.Team == myTeam ? myTeam : enemyTeam;
            }

            foreach (Player player in myTeam.Players)
            {
                Player closestEnemy = player.GetClosest(enemyTeam);

                if (ball.Owner == player)
                {
                    player.ActionShootGoal();

                    var bal = ball;

                    switch (player.PlayerType)
                    {
                        case PlayerType.Keeper:
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

                else if (player.CanPickUpBall(ball)) // Picks up the ball if posible.
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
                    if (player == ball.GetClosest(myTeam))
                    {
                        player.ActionGo(ball);
                    }
                    
                    else switch (player.PlayerType)
                    {
                        case PlayerType.Keeper:
                            player.ActionGo(new Vector(50, Math.Max(Math.Min(ball.Position.Y, Field.MyGoal.Bottom.Y), Field.MyGoal.Top.Y)));
                            break;
                        case PlayerType.LeftDefender:
                            break;
                        case PlayerType.CenterForward:
                            break;
                        case PlayerType.RightDefender:
                            player.ActionGo(Field.EnemyGoal);
                            break;
                        case PlayerType.LeftForward:
                            player.ActionGo(ball);
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
    }
}
    
