using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using MafiaStoryGame;

namespace TelegramBotService.Bot
{
    public class LeaveCommand : CommandHandler
    {
        public bool CanRespond(Messege message)
        {
            var user = GameManager.FindUser(message.from.id);
            return (user != null) ? message.text.StartsWith("/exit") : false;
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            var responces = new List<Messege>();
            var room = GameManager.GetUserRoom(message.from.id);
            var user = GameManager.FindUser(message.from.id);

            if (room.Status == MafiaStoryGame.Models.RoomStatus.Game)
            {
                var actor = room.Game.Actors.FirstOrDefault(q=>q.User.Id==user.Id);
                actor.Status = MafiaStoryGame.Models.ActorStatus.Leave;
                actor.User = null;
                responces.Add(new Messege() { chat = new Chat() { id = user.ChatId }, text = $"you quit from {room.RoomId} room." });
                foreach (var _user in room.Users)
                {
                    responces.Add(new Messege() { chat = new Chat() { id = _user.ChatId }, text = $"User {actor.Name} quit from room. Now users: {room.Users}/{room.MaxUsers}" });
                }
            }
            else
            {
                room.Users.Remove(user);
                responces.Add(new Messege() { chat = new Chat() { id = user.ChatId }, text = $"you quit from {room.RoomId} room." });
                foreach (var _user in room.Users)
                {
                    responces.Add(new Messege() { chat = new Chat() { id = _user.ChatId }, text = $"Some user quit from room. Now users: {room.Users}/{room.MaxUsers}" });
                }
                if (!room.Users.Any())
                {
                    GameManager.Rooms.Remove(room);
                }
            }

            return responces;
        }
    }
}
