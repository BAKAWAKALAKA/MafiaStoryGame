using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NLog;

namespace MafiaStoryGame.Models
{
    public class GameSession
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Actor CurrentActor { get; set; }
        public List<Actor> Actors { get; set; }
        public List<User> Users { get; set; }//все игроки в игре(не удаляются в течении всей игры)
       
        public int RoundGone { get; set; } // сколько дней прошло со старта

        public GameStatus GameStatus { get; set; } // глобально статус игры начата или закончена
        public GameState GameState { get; set; } // состояние игры день ночь перегрузить set или в методе следующие состояния присавивать
      //  public GameType Type { get; } // тип (приватный или публичный) игры по ходу игры менять нельзя 
        public Timer _timer { get; set; } // пока что таймер с константным лимитом на все события

        public List<int> UsedWhiteCards { get; set; }
        public List<int> UsedBlackCards { get; set; }

        public event Action<Dictionary<User, string>> Subscrible;
        private Logger _log = LogManager.GetCurrentClassLogger();

        public GameSession(IEnumerable<User> users)
        {
            Initiliaze();
        }
        

        public void Start()
        {
            _timer = new Timer((x) => Change());
            _timer.Change(new TimeSpan(0, 0, 0), new TimeSpan(0, 1, 0));
            _log.Info($"Session start");
        }

        public GameSession(int time)
        {
            Initiliaze();
        }

        private void Initiliaze()
        {
          
        }

        private void Change()
        {
            var tt = SetNextState();
            Subscrible?.Invoke(tt);
        }

        //нудно ли мне создавать методы здесь или обойтись екстеншен методами чтобы отвязать логику от данных....
        public Dictionary<User, string> SetNextState()
        {
            //todo избавиться от дисктионару! это так дерьмово что просто фу
            //todo неправильная логика передеачи состояния
            _log.Info($"Session setnext session start.curent state {GameState} users:{Actors.Count()}");
            var notification = new Dictionary<User, string>();
            var specialReplices = new Dictionary<User, string>();

            return notification;
        }

        private bool isGameOver()
        {
            return true;
        }
    }


    public enum GameState
    {
        Prepare=0,
        Run=1,
        Complete = 2
    }

    public enum GameStatus
    {
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
