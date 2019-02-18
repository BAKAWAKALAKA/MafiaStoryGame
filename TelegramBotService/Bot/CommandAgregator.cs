using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using MafiaStoryGame;

namespace TelegramBotService.Bot
{
    /// todo я тут подумала а ведь все "распределение а какие команды сейчас пользователь может делать"
    /// можно делать в одной команде которая запрашивает GameManager!!!!!!
    /// скорее всего даже если юзер еще не присоединился к игре все равно нажо создавать его объетк чтобыследить с самого начала(ну в бд записать)
    public class CommandAgregator : CommandHandler
    {
        private Dictionary<User, CommandHandler> _curentUsers;
        private List<CommandHandler> _notInGame;
        private List<CommandHandler> _inGame;
        private List<CommandHandler> _inRoom;
        private List<CommandHandler> _End;

        public CommandAgregator()
        {
            _notInGame = new List<CommandHandler>
            {
                new CreateRoomCommand(),
                new JoinCommand(),
                new FreeRoomsCommand(),
            };
            _inGame = new List<CommandHandler>()
            {
                new IdleCommand()
            };

            _inRoom = new List<CommandHandler>()
            {

            };

            _End = new List<CommandHandler>()
            {

            };
        }

        public bool CanRespond(Messege message)
        {
            ///здесь как обычно проверяем если в списке команд(всех возможных) введеных
            ///хотя с другой стороны блин а если человек просто пишет в чат что то
            ///тогда блин придется лишний раз обращаться в GameManager и смотреть статус игрока и игры в которой он сейчас
            var room = GameManager.GetUserRoom(message.from.id); 
            if (room != null)
            {
                // юзер уже в игре
                var user = room.Users.First(q=>q.Id==message.from.id);

                if (room.Status == MafiaStoryGame.Models.RoomStatus.Wait)
                {
                    // ожидание подключения всех игроков
                    foreach (var cmd in _inRoom)
                    {
                        if (cmd.CanRespond(message))
                        {
                            _curentUsers.Add(message.from, cmd);
                        }
                    }
                }
                if (room.Status == MafiaStoryGame.Models.RoomStatus.Game)
                {
                    // уже идет игра
                    foreach (var cmd in _inGame)
                    {
                        if (cmd.CanRespond(message))
                        {
                            _curentUsers.Add(message.from, cmd);
                        }
                    }
                }
                if (room.Status == MafiaStoryGame.Models.RoomStatus.End)
                {
                    // закончилась
                    foreach (var cmd in _End)
                    {
                        if (cmd.CanRespond(message))
                        {
                            _curentUsers.Add(message.from, cmd);
                        }
                    }
                }
            }
            else
            {
                // пока еще не вступил в игру
                foreach (var cmd in _notInGame)
                {
                    if (cmd.CanRespond(message))
                    {
                        _curentUsers.Add(message.from,cmd);
                    }
                }
            }
            return _curentUsers.Any();
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            // выдавливаем эту сранную команду которую может позволить себе этот сраный юзер
            // хотя может хранить словарь юзера и команды которую задавать будем в CanRespond а вызывать и убирать тут
            throw new NotImplementedException();
        }
    }
}
