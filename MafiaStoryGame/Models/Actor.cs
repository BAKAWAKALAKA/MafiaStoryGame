using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaStoryGame.Models
{
    public class Actor
    {
        public User User { get; set; } // как оказалось в телеге у юзеров ид в инте
        public string Name { get; set; }
        public string Alias { get; set; }
        public Roles Role { get; set; }
        public ActorStatus Status { get; set; }
    }

    public enum Roles
    {
        Habitant,
        Werewolves,
        Priest,
        Hunter
    }

    public enum ActorStatus
    {
        Live=0,
        Dead=1
    }
}
