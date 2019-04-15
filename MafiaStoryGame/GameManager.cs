using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MafiaStoryGame.Models;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using NLog;

namespace MafiaStoryGame
{
    public static class GameManager
    {
        public static int counter;
        public static List<Room> Rooms { get; private set; }
        public static event Action<IEnumerable<Messege>> Subscrible;
        public static List<string> Names { get; set; }
        private static Random r = new Random();

        static GameManager()
        {
            Rooms = new List<Room>();
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Names = new List<string>();
            var json = File.ReadAllText(path+ @"\Dialogs\names_female.json");
            Names.AddRange(JsonConvert.DeserializeObject<List<string>>(json));
            json = File.ReadAllText(path + @"\Dialogs\names_male.json");
            Names.AddRange(JsonConvert.DeserializeObject<List<string>>(json));

            //Rooms = new List<Room>();
            //var testRoom = new Room(new User { Id = 1433345, ChatId = 14334, UserName = "bot" }, "test", 4, 1234);
            //testRoom.Users.Add(new User { Id = 14933345, ChatId = 1439034, UserName = "bot" });
            //testRoom.Users.Add(new User { Id = 143300345, ChatId = 1439734, UserName = "bot" });
            //Rooms.Add(testRoom);
            //Rooms.First().Subscrible += Action;
            counter = 1235;
        }


        public static List<int> Rand(int lenght)
        {
            int n = lenght;
            int[] perm = Enumerable.Range(0, n).ToArray(); // 0 1 2 ... (n - 1)
                                                           // если это НЕ учебное задание, не создавайте новый Random здесь, а заведите
                                                           // один глобальный, а то значения будут всегда одни и те же
            for (int i = n - 1; i >= 1; i--)
            {
                int j = r.Next(i + 1);
                // exchange perm[j] and perm[i]
                int temp = perm[j];
                perm[j] = perm[i];
                perm[i] = temp;
            }
            return perm.ToList();
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

        public static int CreateNewRoom(User user,string name,int maxUsers = 0)
        {
            var max = (maxUsers > 2) ? maxUsers : 3;
            var room = new Room(user, name, maxUsers, counter++);
            room.Subscrible += Action;
            Rooms.Add(room);

            return room.RoomId;
        }

        public static void Action(IEnumerable<Messege> raw)
        {
            Subscrible?.Invoke(raw);
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
            var freeRooms = Rooms.Where(q => q.Status == RoomStatus.Wait).ToList();

            if (!freeRooms.Any()) return "**no one**";

            var res = $"next rooms is free now:{Environment.NewLine}";
            res += $"id - name - users/max_users{Environment.NewLine}";
            foreach (var room in freeRooms)
            {
                res += $"{room.RoomId} - {room.Name} - {room.Users.Count} /{room.MaxUsers}{Environment.NewLine}";
            }
            return res;
        }

        public static string SeeAllRoomInfo()
        {
            var freeRooms = Rooms.ToList();

            if (!freeRooms.Any()) return "no one";

            var res = $"Rooms:";
            res += $"id - name - status - users/max_users{Environment.NewLine}";
            foreach (var room in freeRooms)
            {
                res += $"{room.RoomId} - {room.Name} - {room.Status} - {room.Users.Count} /{room.MaxUsers}{Environment.NewLine}";
            }
            return res;
        }

        public static List<User> RoomUsers(int roomId)
        {
            var room = Rooms.FirstOrDefault(q => q.RoomId == roomId);
            if (room != null)
            {
               return room.Users;
            }
            return null;
        }

        public static void KillMe(GameSession session)
        {
            var room = Rooms.FirstOrDefault(q=>q.Game.Equals(session));
            var actors = room.Game.Actors;
            var res = new List<Messege>();
            foreach (var actor in actors)
            {
                if(actor.User!=null)
                res.Add(new Messege() { From = actor.User.Id, Text = $"комната {room.RoomId} удалена!" });
            }
            Action(res);
            room.KillYourself();
            Rooms.Remove(room);
        }

    }

    public class Messege
    {
        public int From;
        public string Text;
        public DateTime? Time;
        public int Image;
        public IEnumerable<Command> Commands;
    }

    public class Command
    {
        public CommandType CommandType;
        public string ReturnText;
        public string Text;
    }

    public enum CommandType
    {
        InText,
        InConsole
    }
}
