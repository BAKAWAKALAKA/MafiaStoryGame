using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using MafiaStoryGame;
using MafiaStoryGame.Models;

namespace TelegramBotService.Bot
{
    public class IdleCommand : CommandHandler
    {
        public bool CanRespond(Messege message)
        {
            ///todo пока заглужка 
            ///а так надо по entites.type в меседже определять была ли команда
            if (message.text.StartsWith("/")) return false;

            var room = GameManager.GetUserRoom(message.from.id);
            var actor = room.Game.Actors.FirstOrDefault(q => q.User.Id == message.from.id);
            return actor != null;
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            /// todo проверить а может ли и юзер писать сообжения
            /// t e он не мертв?
            /// сейчас не ночь?
            /// если ночь то он не обротень?(или хантер и приест) но пока только для волков
            /// если он волк то только волкам(или хантер и приест) отправлять сообщения
            var room = GameManager.GetUserRoom(message.from.id);
            var actor = room.Game.Actors.FirstOrDefault(q => q.User.Id == message.from.id);
            var messeges = new List<Messege>();

            if (actor.Status != ActorStatus.Dead)
            {
                if (room.Game.GameState == GameState.Night)
                {
                    if (actor.Role == Roles.Werewolves)
                    {
                        // ночью волка услышать могут только волки
                        var wolfs = room.Game.Actors
                            .Where(q => q.Role == Roles.Werewolves).Except(new Actor[] { actor });
                        foreach (var wolf in wolfs)
                        {
                            var _chat = new Chat() { id = actor.User.ChatId };
                            messeges.Add(new Messege()
                            {
                                chat = _chat,
                                text = $"{actor.Alias}: {message.text}"
                            });
                        }
                    }
                    else if (false)
                    {
                        // если кого то тоже надо логику типа волков сделать

                    }
                    else
                    {
                        //написать только отправителю что он не может сейчас отправлять сообщения
                        messeges.Add(new Messege()
                        {
                            chat = message.chat,
                            text = $"ваше бормотание во сне никто не услышал."
                        });
                    }
                }
                else
                {
                    // т к в лубое время кроме ночи все живые могут говорить
                    var vilages = room.Game.Actors.Except(new Actor[] { actor });
                    foreach (var vilege in vilages)
                    {
                        var _chat = new Chat() { id = actor.User.ChatId };
                        messeges.Add(new Messege()
                        {
                            chat = _chat,
                            text = $"{actor.Alias}: {message.text}"
                        });
                    }
                }
            }
            else
            {
                // ты мерт что ты еще писать собрался!!
                //todo добавить возможность последнего возгласа!(еще один стейт в актере типо preDead)
                messeges.Add(new Messege()
                {
                    chat = message.chat,
                    text = $"мертвые не разговаривают"
                });
            }
            return messeges;
        }
    }
}
