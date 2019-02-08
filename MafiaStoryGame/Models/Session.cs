using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaStoryGame.Models
{
    public class Room
    {
        public int RoomId { get;  set; }
        public string Name { get;  set; }
        public List<User> Users {get;  set; }
        public GameSession Game { get;  set; }
        public RoomStatus Status { get;  set; }
        public int MaxUsers { get;  set;}

        public bool AddUser(User user)
        {
            if (Users.FirstOrDefault(q => q.Id == user.Id)!=null) return false;
            if (Users.Count < MaxUsers)
            {
                Users.Add(user);
                if (Users.Count == MaxUsers)
                {
                    Status = RoomStatus.Game;
                    Game = new GameSession();
                    //че то чтобы оповестить о начале
                }
                return true;
            }
            return false;
        }

    }


    public enum RoomStatus
    {
        Wait,
        Game,
        End
    }
}
