﻿using System;
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
            foreach (Player player in myTeam.Players)
            {
                Player closestEnemy = player.GetClosest(enemyTeam);

                if (ball.Owner == player)  //Allways shoots for the enemy goal.
                    player.ActionShootGoal();
                else if (player.CanPickUpBall(ball))  //Picks up the ball if posible.
                    player.ActionPickUpBall();
                else if (player.CanTackle(closestEnemy))  //Tackles any enemy that is close.
                    player.ActionTackle(closestEnemy);
                else  //If the player cannot do anything urgently usefull, move to a good position.
                {
                    if (player == ball.GetClosest(myTeam))  //If the player is closest to the ball, go for it.
                        player.ActionGo(ball);
                    else if (player.PlayerType == PlayerType.Keeper)  //The keeper protects the goal.
                        player.ActionGo(new Vector(50, Math.Max(Math.Min(ball.Position.Y, Field.MyGoal.Bottom.Y), Field.MyGoal.Top.Y))); //The keeper positions himself 50 units out from the goal                                                                                                            //at the same height as the ball, although never leaving the goal
                    else if (player.PlayerType == PlayerType.LeftDefender)
                        player.ActionGo(new Vector(Field.Borders.Width * 0.2, ball.Position.Y));  //The left defender helps protect the goal
                    else if (player.PlayerType == PlayerType.RightDefender)
                        player.ActionGo(Field.MyGoal.GetClosest(enemyTeam));   //The right defender chases the enemy closest to myGoal
                    else if (player.PlayerType == PlayerType.RightForward)
                        player.ActionGo((Field.Borders.Center + Field.Borders.Bottom + ball.Position) / 3);  //Right forward stays in position on the midline, untill the ball comes close.
                    else if (player.PlayerType == PlayerType.LeftForward)
                        player.ActionGo((Field.Borders.Center + Field.Borders.Top + ball.Position) / 3);   //Left forward stays in position on the midline, untill the ball comes close.
                    else if (player.PlayerType == PlayerType.CenterForward)
                        player.ActionGo((Field.Borders.Center + Field.EnemyGoal.Center + ball.Position) / 3);    //Center forward stays in position on the enemy side of the field.
                }
            }
        }
    }
}
