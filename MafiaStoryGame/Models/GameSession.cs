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

        public List<Actor> Actors { get; set; } // возможно надо переделать
        public List<Actor> Werewolfs { get; set; }
        public List<Actor> Habitants { get; set; }
        public List<Actor> Hunters { get; set; }
        public List<Actor> Priest { get; set; }
       // public List<User> Users { get; set; }//все игроки в игре(не удаляются в течении всей игры)

        // очень не очень
        public ActorChoiceDictionry PeapleChoice { get; set; }
        public ActorChoiceDictionry WerewolfsChoice { get; set; }
        public ActorChoiceDictionry PriestsChoice { get; set; } // впринципе и списком можно обойтись
        public ActorChoiceDictionry HunterChoice { get; set; } // впринципе можно и списком обойтись

        public int DaysGone { get; set; } // сколько дней прошло со старта

        public GameStatus GameStatus { get; set; } // глобально статус игры начата или закончена
        public GameState GameState { get; set; } // состояние игры день ночь перегрузить set или в методе следующие состояния присавивать
      //  public GameType Type { get; } // тип (приватный или публичный) игры по ходу игры менять нельзя 
        public Timer _timer { get; set; } // пока что таймер с константным лимитом на все события

        public event Action<Dictionary<User, string>> Subscrible;
        private Logger _log = LogManager.GetCurrentClassLogger();

        public GameSession(IEnumerable<User> users)
        {
            Initiliaze();

            foreach (var user in users)
            {
                var actor = new Actor() { User = user, Alias = "testalies", Role = Roles.Habitant, Status = ActorStatus.Live };
                Actors.Add(actor);
                Habitants.Add(actor);
            }

            RolesMaker();
            AliasMaker();
            var werewolfs = Actors.Where(q=>q.Role == Roles.Werewolves);
            var hunters = Actors.Where(q => q.Role == Roles.Hunter);
            var priest = Actors.Where(q => q.Role == Roles.Priest);
            var habitants = Actors.Where(q=>q.Role == Roles.Habitant);
            if (werewolfs.Any())
            {
                Werewolfs.AddRange(werewolfs);
            }
            if (hunters.Any())
            {
                Hunters.AddRange(hunters);
            }
            if (priest.Any())
            {
                Priest.AddRange(priest);
            }
            if (habitants.Any())
            {
                Habitants.AddRange(habitants);
            }
        }
        private void RolesMaker()
        {
            /// 3 - 1 vfabz 2 ;bntkz
            /// 4 - 1 мафия 3 жителя
            /// 5 - 1 мафия 1 комисар 3 жителя 
            /// 6 - 2 мафии 1 комисар 3 жителя
            /// 7 - 2 мафия 1 комисар 1 доктор 3 жителя
            /// 8 - 2 мафия 1 комисар 1 доктор 4 жителей
            /// 9 - 3 мафия 1 комисар 1 доктор 4 жителя
            ///
            var list = new List<Roles>();


            Actors[2].Role = Roles.Werewolves;

            if (Actors.Count > 4)
            {
                Actors[4].Role = Roles.Hunter;
            }

            if (Actors.Count > 5)
            {
                Actors[5].Role = Roles.Werewolves; 
            }

            if (Actors.Count > 6)
            {
                Actors[6].Role = Roles.Priest;
            }
        }

        private void AliasMaker()
        {
            var indexes = GameManager.Rand(GameManager.Names.Count);
            for (int i = 0; i < Actors.Count; i++)
            {
                Actors[i].Alias = GameManager.Names[indexes[i]];
            }
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
            Actors = new List<Actor>();
            Werewolfs = new List<Actor>();
            Habitants = new List<Actor>();
            Hunters = new List<Actor>();
            Priest = new List<Actor>();
            PeapleChoice = new ActorChoiceDictionry();
            HunterChoice = new ActorChoiceDictionry();
            PriestsChoice = new ActorChoiceDictionry();
            WerewolfsChoice = new ActorChoiceDictionry();
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
            var special = new Dictionary<User, string>();
            if (this.GameState == GameState.End)
            {
                //todo добаить оповещение для тех кто сам еще не вышел из игры что их автоматом выкинуло
                _timer.Change(Timeout.Infinite,Timeout.Infinite);
                _timer = null;
                GameManager.KillMe(this);
            }
            else {
                if (this.GameState == GameState.Start)
                {
                    // первый день
                    this.GameState = GameState.StartDay;
                    this.DaysGone++;


                    foreach (var actor in Actors)
                    {
                        string names = $"В игре следующие игроки:{Environment.NewLine}";
                        foreach (var act in Actors)
                        {
                            if(actor!=act) 
                            names += $"{act.Alias}{Environment.NewLine}";
                        }
                        notification.Add(actor.User, $"Day: FirstDay{DaysGone}{Environment.NewLine}Good morning suspicious ceiling!{Environment.NewLine}{names}{Environment.NewLine}{actor.Alias}(это вы) you are {actor.Role}.Let's survive and win!");
                    }
                }
                else
                {
                    if (isGameOver())
                    {
                        // игра окончена
                        this.GameState = GameState.End;
                        StringBuilder stringBuilder = new StringBuilder();
                        var winners = (Werewolfs.Count < 1) ? "Villagers" : "Werewolfs";
                        stringBuilder.AppendLine($"Day: {DaysGone}{Environment.NewLine}Жители держались {DaysGone} дней.{Environment.NewLine}{winners} is win!");
                        // формируем список участников
                        foreach (var actor in Actors)
                        {
                            stringBuilder.AppendLine($"{actor.Status} {actor.Alias} was {actor.Role}");
                        }
                        var text = stringBuilder.ToString();
                        foreach (var actor in Actors)
                            notification.Add(actor.User, text);

                    }
                    else
                    {
                        // обычные условия
                        switch (this.GameState)
                        {
                            case GameState.StartDay:
                                {
                                    this.GameState = GameState.EndDay;

                                    foreach (var actor in Actors)
                                    {
                                        notification.Add(actor.User, $"Day: {DaysGone}Evening{Environment.NewLine}Now all vilagers should chouice who die!");
                                    }
                                }
                                break;
                            case GameState.EndDay:
                                {
                                    var str = $"Day: {DaysGone} Night! Live users:{Actors.Where(q => q.Status == ActorStatus.Live).Count()} / {Actors.Count()}{Environment.NewLine}Nothing Happen!{Environment.NewLine}";
                                    //проверить результаты голосования
                                    if (PeapleChoice.Any())
                                    {
                                        var killed = PeapleChoice.GroupBy(q => q.Value)
                                            .Select(q => new { Key = q.Key, Val = q.Count() })
                                            .OrderBy(q => q.Val)
                                            .FirstOrDefault().Key;
                                        killed.Status = ActorStatus.Dead;
                                        str += $"{killed.Alias} burn in inquisition fire!";
                                        switch (killed.Role)
                                        {
                                            case Roles.Habitant:
                                                str += "он полностью сгорел в огне. видимо жители ошиблись и он был обычным селянином";
                                                break;
                                            case Roles.Hunter:
                                                str += "он полностью сгорел в огне. уже после в пепле обнаружили значок охотника";
                                                break;
                                            case Roles.Priest:
                                                str += "он полностью сгорел в огне. уже после в пепле обнаружили крест свещеника";
                                                break;
                                            case Roles.Werewolves:
                                                str += "он полностью сгорел в огне. уже после в пелпе вы обнаружили что его останки больше напоминают волка нежили человека!";
                                                break;
                                        }
                                        PeapleChoice.Clear();
                                    }
                                    else
                                    {
                                        str += "Сегодня никто не отправится на костер...";
                                    }
                                    //удалить актера за которого проголосовали(елси нет нечьи)
                                    //вывести соответствующее сообщение о начале ночи
                                    this.GameState = GameState.Night;
                                    foreach (var actor in Actors)
                                    {
                                        notification.Add(actor.User,str);
                                    }
                                }
                                break;
                            case GameState.Night:
                                {
                                    this.GameState = GameState.StartDay;
                                    this.DaysGone++;// т к после ночи всегда наступает новый день!
                                    var str = $"Day: {DaysGone} Morning. Live users:{Actors.Where(q=>q.Status==ActorStatus.Live).Count()}/{Actors.Count()}{Environment.NewLine}";
                                    if (!WerewolfsChoice.Any())
                                    {
                                        str += "видимо волки не были сегодня голодны и ничего не произошло";
                                    }
                                    var luckers = new List<Actor>();

                                    foreach (var choice in HunterChoice)
                                    {
                                        var hunter = choice.Key;
                                        var killed = choice.Value;
                                        str += $"какой то охотник сегодня посетил {killed.Alias}";
                                        if (WerewolfsChoice.ContainsValue(killed))
                                        {
                                            str += "и спугнул оборотня";
                                        }
                                        special.Add(hunter.User, $"{Environment.NewLine}Посещенный вами {killed.Alias} оказался {killed.Role}");
                                    }
                                    HunterChoice.Clear();
                                    foreach (var choice in PriestsChoice)
                                    {
                                        var priest = choice.Key;
                                        var killed = choice.Value;
                                        str += $"какой то свещеник сегодня посетил {killed.Alias}";
                                        if (WerewolfsChoice.ContainsValue(killed) && !luckers.Contains(killed))
                                        {
                                            str += "и спугнул оборотня";
                                        }
                                    }
                                    PriestsChoice.Clear();
                                    //foreach (var killed in WerewolfsChoice.Select(q=>q.Value).Except(luckers))
                                    //{
                                    //    killed.Status = ActorStatus.Dead;
                                    //    str += $"В одном из переулков обноружен {killed.Alias}. Его труп был зверски истерзан.";
                                    //    switch (killed.Role)
                                    //    {
                                    //        case Roles.Hunter:
                                    //            str += "в руке он сжимал значек охотника";
                                    //            break;
                                    //        case Roles.Priest:
                                    //            str += "в руке он сжимал крест";
                                    //            break;
                                    //        case Roles.Werewolves:
                                    //            str += "видимо оборотни занялись конибализмом. иначе как объяснить что его лицо слишком волчье...";
                                    //            break;
                                    //    }
                                    //}

                                    var kill = WerewolfsChoice.Where(q=>!luckers.Contains(q.Value))
                                          .GroupBy(q => q.Value)
                                          .Select(q => new { Key = q.Key, Val = q.Count() })
                                          .OrderBy(q => q.Val)
                                          .FirstOrDefault().Key;
                                    kill.Status = ActorStatus.Dead;
                                    str += $"В одном из переулков обноружен {kill.Alias}. Его труп был зверски истерзан.";
                                    switch (kill.Role)
                                    {
                                        case Roles.Hunter:
                                            str += "в руке он сжимал значек охотника";
                                            break;
                                        case Roles.Priest:
                                            str += "в руке он сжимал крест";
                                            break;
                                        case Roles.Werewolves:
                                            str += "видимо оборотни занялись конибализмом. иначе как объяснить что его лицо слишком волчье...";
                                            break;
                                    }
                                    WerewolfsChoice.Clear();

                                    foreach (var actor in Actors)
                                    {
                                        if (special.ContainsKey(actor.User))
                                        {
                                            notification.Add(actor.User, str+special[actor.User]);
                                        }
                                        else
                                        {
                                            notification.Add(actor.User, str);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            foreach (var not in notification)
                _log.Info($"notify {not.Key.Id} text: {not.Value}");

            var result = new Dictionary<User, string>();
            foreach (var nn in notification)
            {
                var atr = Actors.First(q=>q.User.Id==nn.Key.Id);
                if(atr.Status!= ActorStatus.Leave)
                {
                    result.Add(nn.Key,nn.Value);
                }
            }
            return result;
        }

        private bool isGameOver()
        {
            if (Werewolfs.Where(q=>q.Status==ActorStatus.Live).Count() < 1) return true;
           return Actors.Except(Werewolfs).Where(q => q.Status == ActorStatus.Live).Count() <= 1;
        }

    }


    public enum GameState
    {
        Start=0,
        StartDay=1,
        Evening = 2,
        EndDay =3,//midnight
        Night=4,
        EndNight = 5,
        End=6
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


    public class ActorChoiceDictionry: Dictionary<Actor, Actor>
    {
        public new void Add(Actor key, Actor value)
        {
            if (this.ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                base.Add(key,value);
            }
        }
    }

}
