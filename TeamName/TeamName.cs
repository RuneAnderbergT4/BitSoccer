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
                            // Find players of LD, CF or RD type
                            var myTeamLD = myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.LeftDefender);
                            var myTeamCF = myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.CenterForward);
                            var myTeamRD = myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.RightDefender);

                            // Gets distance to closest enemy player for the LD, CF and RD
                            var myTeamLDDistanceClosestEnemyPlayer =
                                myTeamLD.GetDistanceTo(myTeamLD.GetClosest(enemyTeam));
                            var myTeamCFDistanceClosestEnemyPlayer =
                                myTeamCF.GetDistanceTo(myTeamCF.GetClosest(enemyTeam));
                            var myTeamRDDistanceClosestEnemyPlayer =
                                myTeamRD.GetDistanceTo(myTeamRD.GetClosest(enemyTeam));

                            // GK passes LD, CF or RD with the biggest distance to an enemy player
                            if (myTeamLDDistanceClosestEnemyPlayer > myTeamCFDistanceClosestEnemyPlayer &&
                                myTeamLDDistanceClosestEnemyPlayer > myTeamRDDistanceClosestEnemyPlayer)
                            {
                                player.ActionShoot(myTeamLD);
                            }
                            else if (myTeamCFDistanceClosestEnemyPlayer > myTeamLDDistanceClosestEnemyPlayer &&
                                     myTeamCFDistanceClosestEnemyPlayer > myTeamRDDistanceClosestEnemyPlayer)
                            {
                                player.ActionShoot(myTeamCF);
                            }
                            else if (myTeamRDDistanceClosestEnemyPlayer > myTeamLDDistanceClosestEnemyPlayer &&
                                     myTeamRDDistanceClosestEnemyPlayer > myTeamCFDistanceClosestEnemyPlayer)
                            {
                                player.ActionShoot(myTeamRD);
                            }
                            else
                            {
                                player.ActionShootGoal();
                            }
                            break;
                        case PlayerType.LeftDefender:
                        case PlayerType.CenterForward:
                        case PlayerType.RightDefender:
                            // Find player of LF and RF type
                            var myTeamLF = myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.LeftForward);
                            var myTeamRF = myTeam.Players.Find(player1 => player1.PlayerType == PlayerType.RightForward);

                            // Gets distance to closest enemy player for the LF and RF
                            var myTeamLFDistanceClosestEnemyPlayer =
                                myTeamLF.GetDistanceTo(myTeamLF.GetClosest(enemyTeam));
                            var myTeamRFDistanceClosestEnemyPlayer =
                                myTeamRF.GetDistanceTo(myTeamRF.GetClosest(enemyTeam));

                            // Defender passes LD, CF or RD with the biggest distance to an enemy player
                            if (myTeamLFDistanceClosestEnemyPlayer > myTeamRFDistanceClosestEnemyPlayer)
                            {
                                player.ActionShoot(myTeamLF);
                            }
                            else if (myTeamRFDistanceClosestEnemyPlayer > myTeamLFDistanceClosestEnemyPlayer)
                            {
                                player.ActionShoot(myTeamRF);
                            }
                            break;
                        case PlayerType.LeftForward:
                        case PlayerType.RightForward:
                            // Finds enemy GK
                            var enemyTeamGK = enemyTeam.Players.Find(player1 => player1.PlayerType == PlayerType.Keeper);

                            // Checks GK distance to top & bottom of goal
                            var disTop = enemyTeamGK.GetDistanceTo(Field.EnemyGoal.Top);
                            var disBot = enemyTeamGK.GetDistanceTo(Field.EnemyGoal.Bottom);

                            // Shoot at the side of the goal the enemy GK is furthest away from
                            if (disTop > disBot)
                            {
                                player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Top.Y + 50));
                            }
                            else if (disBot > disTop)
                            {
                                player.ActionShoot(new Vector(Field.EnemyGoal.Center.X, Field.EnemyGoal.Bottom.Y - 50));
                            }
                            else if (player.GetDistanceTo(Field.EnemyGoal.Center) > 1000)
                            {
                                player.ActionShoot(player.GetClosest(myTeam));
                            }
                            else
                            {
                                player.ActionShoot(Field.EnemyGoal);
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

                else if ((player.CanTackle(closestEnemy) && player.PlayerType == PlayerType.LeftDefender) ||
                         (player.CanTackle(closestEnemy) && player.PlayerType == PlayerType.CenterForward) ||
                         (player.CanTackle(closestEnemy) && player.PlayerType == PlayerType.RightDefender))
                    // Tackles any enemy that is close.
                {
                    player.ActionTackle(closestEnemy);
                }

                else // If the player cannot do anything urgently usefull, move to a good position.
                {
                    // Goal Keeper
                    if (player.PlayerType == PlayerType.Keeper)
                    {
                        player.ActionGo(new Vector(50,
                            Math.Max(Math.Min(ball.Position.Y, Field.MyGoal.Bottom.Y - 50), Field.MyGoal.Top.Y + 50)));
                    }

                    // Left Defender
                    else if (player.PlayerType == PlayerType.LeftDefender)
                    {
                        player.ActionGo(new Vector(Field.Borders.Width*0.2, ball.Position.Y - 120));
                    }

                    // Center Defender
                    else if (player.PlayerType == PlayerType.CenterForward)
                    {
                        player.ActionGo(new Vector(Field.Borders.Width*0.2, ball.Position.Y));
                    }

                    // Right Defender
                    else if (player.PlayerType == PlayerType.RightDefender)
                    {
                        player.ActionGo(new Vector(Field.Borders.Width*0.2, ball.Position.Y + 120));
                    }

                    // Left Forward
                    else if (player.PlayerType == PlayerType.LeftForward)
                    {
                        // Gets closest enemy player
                        var clostestEnemyPlayer = player.GetClosest(enemyTeam);

                        // Get distance to it
                        var closestPlayerDistance = player.GetDistanceTo(clostestEnemyPlayer);

                        // If an enemy is nearby and player is close to enemy goal
                        if (closestPlayerDistance < 400 && player.GetDistanceTo(Field.EnemyGoal.Position) < 800)
                        {
                            var vector = (player.Position - clostestEnemyPlayer.Position);
                            vector.Normalize();
                            player.ActionGo(vector);
                        }

                        else
                        {
                            player.ActionGo(new Vector(Field.EnemyGoal.Center.X - 500, Field.EnemyGoal.Center.Y + 100));
                        }
                    }

                    // Right Foward
                    else if (player.PlayerType == PlayerType.RightForward)
                    {
                        // Gets closest enemy player
                        var clostestEnemyPlayer = player.GetClosest(enemyTeam);

                        // Get distance to it
                        var closestPlayerDistance = player.GetDistanceTo(clostestEnemyPlayer);

                        // If an enemy is nearby and player is close to enemy goal
                        if (closestPlayerDistance < 400 && player.GetDistanceTo(Field.EnemyGoal.Position) < 800)
                        {
                            var vector = (player.Position - clostestEnemyPlayer.Position);
                            vector.Normalize();
                            player.ActionGo(vector);
                        }

                        else
                        {
                            player.ActionGo(new Vector(Field.EnemyGoal.Center.X - 500, Field.EnemyGoal.Center.Y + 100));
                        }
                    }
                }
            }
        }
    }
}
    
