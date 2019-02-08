using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaStoryGame.Models;

namespace MafiaStoryGame
{
    public static class GameManager
    {
        public static int counter;
        public static List<Room> Rooms { get; private set; }

        static GameManager()
        {
            Rooms = new List<Room>();
            counter = 1234;
        }


        public static User FindUser(int id)
        {
            foreach (var room in Rooms)
            {
                var user = room.Users.FirstOrDefault(u=>u.Id==id);
                if (user != null) return user;
            }
            return null;
        }

        public static Room GetUserRoom(int userId)
        {
            foreach (var room in Rooms)
            {
                var user = room.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null) return room;
            }
            return null;
        }

        public static int CreateNewRoom(User user,string name,int maxUsers)
        {
            var room = new Room()
            {
                Users = new List<User>() { user },
                Name = name,
                MaxUsers = maxUsers,
                RoomId = counter++
            };
            Rooms.Add(room);

            return room.RoomId;
        }

        public static bool JoinRoom(int roomId, User user)
        {
            var room = Rooms.FirstOrDefault(q => q.RoomId == roomId);
            if (room != null)
            {
                if (room.Status==RoomStatus.Wait)
                {
                    return room.AddUser(user);
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public static string SeeFreeRoomsInfo()
        {
            // todo переделать т к наверно лучше предовать структуру чем стрингу
            var freeRooms = Rooms.Where(q => q.Status == RoomStatus.Wait).ToList();

            if (!freeRooms.Any()) return "no one";

            var res = $"next rooms is free now:{Environment.NewLine}";
            res += $"id - name - users/max_users{Environment.NewLine}";
            foreach (var room in freeRooms)
            {
                res += $"{room.RoomId} - {room.Name} - {room.Users.Count} /{room.MaxUsers}{Environment.NewLine}";
            }
            return res;
        }

        public static List<User> RoomUsers(int roomId)
        {
            var room = Rooms.FirstOrDefault(q => q.RoomId == roomId);
            if (room != null)
            {
                if (room.Status == RoomStatus.Wait)
                {
                    return room.Users;
                }
            }
            return null;
        }
    }


}
