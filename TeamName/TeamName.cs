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
                            if (player.GetDistanceTo(Field.EnemyGoal.Center) < 800)
                            {
                                player.
                            }
                            player.ActionGo(Field.EnemyGoal.Center);
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
    
