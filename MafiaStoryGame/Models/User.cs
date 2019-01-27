using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaStoryGame.Models
{
    public class User
    {
        public int Id;
        public string UserName;
        public string Firstname;
        public string SecondName;
        public DateTime LastTime; //когда в последний раз юзер проявлял активность
        public bool inGame;
    }
}
