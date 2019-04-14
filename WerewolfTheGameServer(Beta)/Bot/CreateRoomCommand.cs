using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using MafiaStoryGame;

namespace WerewolfTheGameServer.Bot
{
    public class CreateRoomCommand : CommandHandler
    {
        public bool CanRespond(Messege message)
        {
            return message.text.ToLower().StartsWith("/new");
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            var user = Map(message.from);
            var maxUsers = 5;
            user.ChatId = message.chat.id; // todo так как нужно отправлять конкретно в комнату юзеру
            var parametrs = message.text.Split(' ');
            if (parametrs.Any() && parametrs.Count()>1)
            {
                int.TryParse(parametrs[1],out maxUsers);
            }
            var roomId = GameManager.CreateNewRoom(user,"GAME",maxUsers);

            return new Messege[] { new Messege() { chat = message.chat, text = $"room created! id = {roomId}"} };
        }

        private MafiaStoryGame.Models.User Map(User user)
        {
            var _user = new MafiaStoryGame.Models.User();
            _user.Id = user.id;
            _user.UserName = (string.IsNullOrEmpty(user.username))? $"{user.first_name} {user.last_name}" : user.username;
            return _user;
        }

    }
}
