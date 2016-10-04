using Common; //Common contains all the classes you need to play CloudBall.

namespace TeamOne
{
    /// <summary>
    /// This is the example team TeamOne.
    /// 
    /// It is a very basic team, all the players act the same, and they allways go for the ball.
    /// </summary>
    //All teams submitted has to inherit from ITeam
    public class TeamOne : ITeam    
    {                                                
        public void Action(Team myTeam, Team enemyTeam, Ball ball, MatchInfo matchInfo)
        {
            //Loop over all players in my team.
            foreach (Player player in myTeam.Players)                                      
            {
                //If this player has the ball.
                if (ball.Owner == player)                                                   
                {
                    //Tell this player to shoot towards the goal, at maximum strength!
                    player.ActionShootGoal();                                               
                }
                else if (player.CanPickUpBall(ball))
                {
                    player.ActionPickUpBall();
                }
                else
                {
                    //Worst case just go for the ball
                    player.ActionGo(ball.Position);
                }                                                  
            }                                                          
        }
    }
}