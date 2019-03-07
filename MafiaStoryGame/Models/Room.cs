using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace MafiaStoryGame.Models
{
    public class Room
    {
        public int RoomId { get;  set; }
        public string Name { get;  set; }
        public List<User> Users {get;  set; }
        public GameSession Game { get;  set; }
        public RoomStatus Status { get;  set; }
        public int MaxUsers { get; set; } = 5;
        public event Action<Dictionary<User,string>> Subscrible;
        private Logger _log = LogManager.GetCurrentClassLogger();
        
        public Room(User user, string name, int maxUsers, int counter)
        {
            Users = new List<User>() { user };
            Name = name;
            MaxUsers = maxUsers;
            RoomId = counter;
            _log.Info($"created room:{RoomId}");
        }

        public bool AddUser(User user)
        {
            _log.Info($"room {RoomId} add user {user.Id}");
            if (Users.FirstOrDefault(q => q.Id == user.Id)!=null) return false;
            if (Users.Count < MaxUsers)
            {
                Users.Add(user);
                if (Users.Count == MaxUsers)
                {
                    Status = RoomStatus.Game;
                    Game = new GameSession(this.Users);
                    Game.Subscrible += Action;
                    Game.Start();
                    _log.Info($"room {RoomId} change status to {Status}");
                    //че то чтобы оповестить о начале
                }
                return true;
            }
            return false;
        }

        public void Action(Dictionary<User, string> raw)
        {
            Subscrible?.Invoke(raw);
        }

        public void KillYourself()
        {
            // на всякий случай диспойзнуть все и отписаться
            Game = null;
            Users = null;
            Subscrible = null;
            _log.Info($"room {RoomId} was deleted");
        }
    }


    public enum RoomStatus
    {
        Wait,
        Game,
        End
    }
}
