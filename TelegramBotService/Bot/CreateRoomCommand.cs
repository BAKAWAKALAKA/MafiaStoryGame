using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using MafiaStoryGame;

namespace TelegramBotService.Bot
{
    public class CreateRoomCommand : CommandHandler
    {
        public bool CanRespond(Messege message)
        {
            return message.text.Contains("/new");
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            var user = Map(message.from);
            user.ChatId = message.chat.id; // todo так как нужно отправлять конкретно в комнату юзеру
            var roomId = GameManager.CreateNewRoom(user,"GAME",2);

            return new Messege[] { new Messege() { chat = message.chat, text = $"room created! id = {roomId}"} };
        }

        private MafiaStoryGame.Models.User Map(User user)
        {
            var _user = new MafiaStoryGame.Models.User();
            _user.Id = user.id;
            _user.UserName = user.username;
            return _user;
        }

    }
}
