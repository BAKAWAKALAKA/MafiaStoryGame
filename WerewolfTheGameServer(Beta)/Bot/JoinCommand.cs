using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using MafiaStoryGame;

namespace WerewolfTheGameServer.Bot
{
    public class JoinCommand : CommandHandler
    {
        public bool CanRespond(Messege message)
        {
            return message.text.ToLower().StartsWith("/join");
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            var msg= new Messege()
            {
                chat = message.chat,
                text = "reject"
            };
            int roomId;
            var strId = message.text.Replace("/join","").Replace(" ","");
            if(int.TryParse(strId,out roomId))
            {
                var _user = Map(message.from);
                _user.ChatId = message.chat.id; // todo так как нужно отправлять конкретно в комнату юзеру
                var result = GameManager.JoinRoom(roomId, _user);
                if (result)
                {
                    msg.text = $"you're joined to {roomId} room.";

                    var list = new List<Messege>();
                    list.Add(msg);
                    //todo fall 
                    foreach (var user in GameManager.RoomUsers(roomId))
                    {
                        if (user.ChatId != user.ChatId)
                        {
                            list.Add(new Messege() { chat = new Chat() { id = user.ChatId }, text = $"{message.from.first_name} joined to {roomId} room." });
                        }
                    }
                    return list;
                }
            }
            else
            {
                msg.text = "не указан номер";
            }
            return new Messege[] { msg };
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
