using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MafiaStoryGame.Models
{
    public class SessionInfo
    {
        // класс для момента когда игроки еще ищут 
        public void ts()
        {
            var t = new Timer(async (x) => this.GetTask());
            t.Change(1,1);
        }

        public async Task GetTask()
        {

        }

    }
}
