using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using MafiaStoryGame;
using MafiaStoryGame.Models;

namespace WerewolfTheGameServer.Bot
{
    public class KillCommand : CommandHandler
    {
        //todo команда еще не реализована в игровой сессии
        public bool CanRespond(Messege message)
        {
            if (message.text.ToLower().StartsWith("/kill"))
            {
                var room = GameManager.GetUserRoom(message.from.id);
                return (room.Game.GameState == MafiaStoryGame.Models.GameState.EndDay ||
                        room.Game.GameState == MafiaStoryGame.Models.GameState.Night);
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            var room = GameManager.GetUserRoom(message.from.id);
            var actor = room.Game.Actors.FirstOrDefault(q=>q.User.Id == message.from.id);
            var result = new List<Messege>();

            if (room.Game.GameState == MafiaStoryGame.Models.GameState.EndDay)
            {
                var name = message.text.Split(' ').Last().ToLower();
                var victim = room.Game.Actors.FirstOrDefault(q=>q.Alias.ToLower()==name);
                if (victim==null)
                {
                    result.Add(new Messege() { chat= message.chat, text = "такого юзера здесь нет!" });
                }
                else
                {
                    if (victim.Status == ActorStatus.Dead)
                    {
                        result.Add(new Messege() { chat = message.chat, text = "он итак мертв!" });
                    }
                    room.Game.PeapleChoice.Add(actor, victim);
                    result.Add(new Messege() { chat = message.chat, text = "голос принят" });
                    foreach (var actr in room.Game.Actors)
                    {
                        result.Add(new Messege() { chat = new Chat() { id= actr.User.ChatId}, text = $"{actor.Alias} проголосовал за {victim.Alias}" });
                    }
                }
            }
            else
            {
                //night
                if(actor.Role!= MafiaStoryGame.Models.Roles.Habitant)
                {
                    var name = message.text.Split(' ').Last().ToLower();
                    var victim = room.Game.Actors.FirstOrDefault(q => q.Alias.ToLower() == name);
                    if (victim == null)
                    {
                        result.Add(new Messege() { chat = message.chat, text = "такого юзера здесь нет!" });
                    }
                    else
                    {
                        if (victim.Status != ActorStatus.Dead)
                        {
                            if (actor.Role == MafiaStoryGame.Models.Roles.Werewolves)
                            {
                                var werewolfs = room.Game.Actors.Where(q => q.Role == MafiaStoryGame.Models.Roles.Werewolves);
                                room.Game.WerewolfsChoice.Add(actor, victim);
                                foreach (var werewolf in werewolfs)
                                {
                                    result.Add(new Messege() { chat = new Chat() { id = werewolf.User.ChatId }, text = $"{actor.Alias} проголосовал за {victim.Alias}" });
                                }
                            }
                            if (actor.Role == MafiaStoryGame.Models.Roles.Hunter)
                            {
                                //todo добавить сообщение кто выбранный актер охотнику!
                                var hunters = room.Game.Actors.Where(q => q.Role == MafiaStoryGame.Models.Roles.Hunter);
                                room.Game.HunterChoice.Add(actor, victim);
                                foreach (var hunter in hunters)
                                {
                                    result.Add(new Messege() { chat = new Chat() { id = hunter.User.ChatId }, text = $"{actor.Alias} решил пойти за {victim.Alias}" });
                                }
                            }
                            if (actor.Role == MafiaStoryGame.Models.Roles.Priest)
                            {
                                var priests = room.Game.Actors.Where(q => q.Role == MafiaStoryGame.Models.Roles.Priest);
                                room.Game.PriestsChoice.Add(actor, victim);
                                foreach (var priest in priests)
                                {
                                    result.Add(new Messege() { chat = new Chat() { id = priest.User.ChatId }, text = $"{actor.Alias} решил пойти за {victim.Alias}" });
                                }
                            }
                        }
                        else
                        {
                            result.Add(new Messege() { chat = message.chat, text = "за чем?!? он итак мертв!" });
                        }
                    }
                }
                else
                {
                    result.Add(new Messege() { chat = message.chat, text = "вы не можите голосовать ночью" });
                }
            }
            return result;
        }
    }
}
