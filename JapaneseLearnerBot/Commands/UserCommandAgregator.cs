using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;

namespace JapaneseLearnerBot.Commands
{
    public class UserCommandAgregator: CommandHandler
    {
        private Dictionary<int, CommandHandler> _curentUsers;

        public UserCommandAgregator()
        {

        }

        public bool CanRespond(Messege message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            throw new NotImplementedException();
        }
    }
}
