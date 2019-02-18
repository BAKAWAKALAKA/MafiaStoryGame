using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MafiaStoryGame.Models
{
    public class GameSession
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Host { get; set; }

        public List<Actor> Actors { get; set; } // возможно надо переделать
        public List<Actor> Werewolfs { get; set; }
        public List<Actor> Habitants { get; set; }
        public List<Actor> Hunters { get; set; }
        public List<Actor> Priest { get; set; }
        public List<User> Users { get; set; }//все игроки в игре(не удаляются в течении всей игры)

        // очень не очень
        public Dictionary<Actor, Actor> PeapleChoice { get; set; }
        public Dictionary<Actor, Actor> WerewolfsChoice { get; set; }
        public Dictionary<Actor, Actor> PriestsChoice { get; set; } // впринципе и списком можно обойтись
        public Dictionary<Actor, Actor> HunterChoice { get; set; } // впринципе можно и списком обойтись

        public int DaysGone { get; set; } // сколько дней прошло со старта

        public GameStatus GameStatus { get; set; } // глобально статус игры начата или закончена
        public GameState GameState { get; set; } // состояние игры день ночь перегрузить set или в методе следующие состояния присавивать
        public GameType Type { get; } // тип (приватный или публичный) игры по ходу игры менять нельзя 
        public Timer _timer { get; set; } // пока что таймер с константным лимитом на все события

        public event Action<Dictionary<User, string>> Subscrible;

        public GameSession()
        {
            Initiliaze();
        }

        public GameSession(int time)
        {
            Initiliaze();
        }

        private void Initiliaze()
        {
            Actors = new List<Actor>();
            Werewolfs = new List<Actor>();
            Habitants = new List<Actor>();
            Hunters = new List<Actor>();
            Priest = new List<Actor>();



            _timer = new Timer((x)=>Change());
            _timer.Change(new TimeSpan(0,0,0),new TimeSpan(0,5,0));
        }

        private void Change()
        {
            var tt = SetNextState();
            if (Subscrible != null)
            {
                Subscrible(tt);
            }
        }


        //нудно ли мне создавать методы здесь или обойтись екстеншен методами чтобы отвязать логику от данных....
        public Dictionary<User, string> SetNextState()
        {
            var notification = new Dictionary<User, string>();
            if (this.GameState==GameState.Start)
            {
                // первый день
                this.GameState = GameState.StartDay;
                this.DaysGone++;

                foreach (var actor in Actors)
                {
                    notification.Add(actor.User, $"Day: {DaysGone}{Environment.NewLine}Good morning suspicious ceiling!");
                }
            }
            else
            if (isGameOver())
            {
                // игра окончена
                this.GameState = GameState.End;

                foreach(var actor in Actors)
                notification.Add(actor.User, $"Day: {DaysGone}{Environment.NewLine} After all everybody died");

            }
            else
            {
                // обычные условия
                this.DaysGone++;

                switch (this.GameState)
                {
                    case GameState.StartDay:
                        {
                            this.GameState = GameState.EndDay;

                            foreach (var actor in Actors)
                            {
                                notification.Add(actor.User, $"Day: {DaysGone}{Environment.NewLine}Nothing Happen!");
                            }
                        }
                        break;
                    case GameState.EndDay:
                        {
                            this.GameState = GameState.Night;
                            // начать голосование
                            foreach (var actor in Actors)
                            {
                                notification.Add(actor.User, $"Day: {DaysGone}{Environment.NewLine}Nothing Happen!");
                            }
                        }
                        break;
                    case GameState.Night:
                        {
                            this.GameState = GameState.StartDay;
                            //проверить результаты голосования
                            //удалить актера за которого проголосовали(елси нет нечьи)
                            //вывести соответствующее сообщение о начале ночи
                            foreach (var actor in Actors)
                            {
                                notification.Add(actor.User, $"Day: {DaysGone}{Environment.NewLine}Nothing Happen!");
                            }
                        }
                        break;
                }

            }
            return notification;
        }

        private bool isGameOver()
        {
            if (Werewolfs.Count() < 1) return true;
           return Actors.Except(Werewolfs).Count() < 1;
        }

    }


    public enum GameState
    {
        Start=0,
        StartDay=1,
        EndDay=2,
        Night=3,
        End=4
    }

    public enum GameStatus
    {
        Preparing,
        Start,
        Running,
        End
    }

    public enum GameType
    {
        Private,
        Public
    }

}
