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
        private static List<Room> Rooms { get; set; }

        static GameManager()
        {
            Rooms = new List<Room>();
            counter = 1234;
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
                    room.AddUser(user);
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

            var res = "next rooms is free now:";
            res += "id - name - users/max_users";
            foreach (var room in freeRooms)
            {
                res += $"{room.RoomId} - {room.Name} - {room.Users.Count} /{room.MaxUsers}";
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
