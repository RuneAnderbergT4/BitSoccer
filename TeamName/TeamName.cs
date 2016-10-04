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
        public void Action(Team myTeam, Team enemyTeam, Ball ball, MatchInfo matchInfo)
        {
            Random random = new Random(0);

            foreach (Player player in myTeam.Players)
            {
                Player closestEnemy = player.GetClosest(enemyTeam);

                if (ball.Owner == player) // Always shoots for the enemy goal.
                {
                    if (player.PlayerType == PlayerType.Keeper)
                    {
                        // Find player of type LD, RD or CF and pass to the one that has an enemy player the furthest away
                        var target = myTeam.Players.Find();
                        
                        player.ActionShoot(target);
                    }
                    else if (player.PlayerType == PlayerType.LeftDefender || player.PlayerType == PlayerType.CenterForward || player.PlayerType == PlayerType.RightDefender)
                    {
                        // Find player of type LF or RF and pass to the one that has an enemy player the furthest away
                        player.ActionShoot(player.GetClosest(myTeam));
                    }
                    else
                    {
                        // Finds enemy GK
                        var enemyGK = enemyTeam.Players.Find((player1 => player1.PlayerType == PlayerType.Keeper));

                        // Checks GK distance to top & bottom of goal
                        var disTop = enemyGK.GetDistanceTo(Field.EnemyGoal.Top);
                        var disBot = enemyGK.GetDistanceTo(Field.EnemyGoal.Bottom);

                        // Shoot at the side of the goal the enemy GK is furthest away from
                        if (disTop > disBot)
                        {
                            player.ActionShoot(Field.EnemyGoal.Top);
                        }
                        else if (disBot > disTop)
                        {
                            player.ActionShoot(Field.EnemyGoal.Bottom);
                        }
                        else
                        {
                            player.ActionShoot(Field.EnemyGoal);
                        }
                        
                    }
                    
                }

                else if (player.CanPickUpBall(ball)) // Picks up the ball if posible.
                {
                    player.ActionPickUpBall();
                }

                else if (player.CanTackle(closestEnemy)) // Tackles any enemy that is close.
                {
                    player.ActionTackle(closestEnemy);
                }

                else // If the player cannot do anything urgently usefull, move to a good position.
                {
                    if (player == ball.GetClosest(myTeam)) // If the player is closest to the ball, go for it.
                    {
                        player.ActionGo(ball);
                    }

                    // Goal Keeper
                    else if (player.PlayerType == PlayerType.Keeper)
                    {
                        player.ActionGo(new Vector(50,
                            Math.Max(Math.Min(ball.Position.Y, Field.MyGoal.Bottom.Y), Field.MyGoal.Top.Y)));
                    }

                    // Left Defender
                    else if (player.PlayerType == PlayerType.LeftDefender)
                    {
                        player.ActionGo(new Vector(Field.Borders.Width * 0.2, ball.Position.Y - 150));
                    }

                    // Center Defender
                    else if (player.PlayerType == PlayerType.CenterForward)
                    {
                        player.ActionGo(new Vector(Field.Borders.Width*0.2, ball.Position.Y));
                    }

                    // Right Defender
                    else if (player.PlayerType == PlayerType.RightDefender)
                    {
                        player.ActionGo(new Vector(Field.Borders.Width*0.2, ball.Position.Y + 150));
                    }

                    // Left Forward
                    else if (player.PlayerType == PlayerType.LeftForward)
                    {
                        player.ActionGo((Field.Borders.Center + Field.Borders.Top + ball.Position)/3);
                    }
                    
                    // Right Foward
                    else if (player.PlayerType == PlayerType.RightForward)
                    {
                        player.ActionGo((Field.Borders.Center + Field.Borders.Bottom + ball.Position)/3);
                    }
                }
            }
        }
    }
}
