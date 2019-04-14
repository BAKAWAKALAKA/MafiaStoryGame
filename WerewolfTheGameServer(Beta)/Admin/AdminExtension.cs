using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaStoryGame;
using NLog;

namespace WerewolfTheGameServer.Admin
{
    public class AdminExtension
    {
        public AdminPanel curentState { get; set;}
        public Logger _log = LogManager.GetCurrentClassLogger();

        public void Execute(string raw)
        {
            raw = raw.ToLower();
            if(raw.Contains("to main"))
            {
                curentState = AdminPanel.Main;
                _log.Trace("to main");
            }else
            if(curentState == AdminPanel.Main)
            {
                if (raw.Contains("all rooms"))
                {
                    _log.Trace(GameManager.SeeAllRoomInfo());
                }
                if (raw.Contains("room "))
                {
                    var sub = raw.Split(' ').Last();
                    var id = int.Parse(sub);
                    var room = GameManager.Rooms.FirstOrDefault(q => q.RoomId == id);
                    //todo добавить логику для просмотра subscrible
                    if (room != null)
                    {
                        curentState = AdminPanel.Room;
                        _log.Trace($"Room id {room.RoomId} - {room.Status} {room.Users.Count}/{room.MaxUsers}");
                        _log.Trace($"game {room.Game != null}");
                        if (room.Game != null)
                        {
                            var actors = room.Game.Actors;
                            var days = room.Game.DaysGone;
                            
                            var str = $"days gone {days} actors = {actors.Count}{Environment.NewLine}user id - actor name - role - status{Environment.NewLine}";
                            foreach (var actor in actors)
                            {
                                str += $"{actor.User.Id} - {actor.Alias} - {actor.Role} - {actor.Status}{Environment.NewLine}";
                            }
                            _log.Trace(str);
                        }
                    }
                    else
                    {
                        _log.Trace("nu such room");
                    }
                }
                if (raw.Contains("all users"))
                {
                    var users = GameManager.Rooms.SelectMany(q => q.Users);
                    var str = $"User id - user name - chat id{Environment.NewLine}";
                    foreach (var user in users)
                    {
                        str += $"{user.Id} - {user.UserName} - {user.ChatId}{Environment.NewLine}";
                    }
                    _log.Trace(str);
                }
                if (raw.Contains("user "))
                {
                    var sub = raw.Split(' ').Last();
                    var id = int.Parse(sub);
                    var user = GameManager.FindUser(id);
                    if (user != null)
                    {
                        curentState = AdminPanel.User;
                        var room = GameManager.GetUserRoom(id);
                        _log.Trace($"{user.Id} - {user.UserName} - {user.ChatId}{Environment.NewLine}");
    

                        _log.Trace($"Room id {room.RoomId} - {room.Status} {room.Users.Count}/{room.MaxUsers}");
                        _log.Trace($"game {room.Game != null}");
                    }
                    else
                    {
                        _log.Trace("no such user");
                    }
                }
            }
            else
            {

            }
        }

    }

    public enum AdminPanel
    {
        Main,
        Room,
        Game,
        User
    }


}
