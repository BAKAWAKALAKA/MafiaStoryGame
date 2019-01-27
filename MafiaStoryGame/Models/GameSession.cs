using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Dictionary<Actor, string> PeapleChoice { get; set; }
        public Dictionary<Actor, string> WerewolfsChoice { get; set; }
        public Dictionary<Actor, string> PriestsChoice { get; set; } // впринципе и списком можно обойтись
        public Dictionary<Actor, string> HunterChoice { get; set; } // впринципе можно и списком обойтись
        //

        public int DaysGone { get; set; } // сколько дней прошло со старта

        public GameStatus GameStatus { get; set; } // глобально статус игры начата или закончена
        public GameState GameState { get; set; } // состояние игры день ночь перегрузить set или в методе следующие состояния присавивать
        public GameType Type { get; } // тип (приватный или публичный) игры по ходу игры менять нельзя 


        public GameSession()
        {
            Initiliaze();
        }

        private void Initiliaze()
        {

        }


        //нудно ли мне создавать методы здесь или обойтись екстеншен методами чтобы отвязать логику от данных....
        public void SetNextState()
        {
            if (this.GameState==GameState.Start)
            {
                // первый день
                this.GameState = GameState.StartDay;
                this.DaysGone++;

                return;
            }
            else
            if (isGameOver())
            {
                // игра окончена
                this.GameState = GameState.End;



                return;
            }
            else
            {
                // игра окончена
                this.DaysGone++;


                switch (this.GameState)
                {
                    case GameState.StartDay:
                        {
                            this.GameState = GameState.EndDay;


                        }
                        break;
                    case GameState.EndDay:
                        {
                            this.GameState = GameState.Night;
                            // начать голосование

                        }
                        break;
                    case GameState.Night:
                        {
                            this.GameState = GameState.StartDay;
                            //проверить результаты голосования
                            //удалить актера за которого проголосовали(елси нет нечьи)
                            //вывести соответствующее сообщение о начале ночи

                        }
                        break;
                }
                return;
            }
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
