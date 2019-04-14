using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaStoryGame.System
{
    public interface Command
    {
        int To { get; set; }
    }

    public class TextCommand : Command
    {
        public int To { get; set; }
        public string Text { get; set; }
    }

    public class ImageCommand : Command
    {
        public int To { get; set; }
        public int Image { get; set; }
    }
    //переделать
    public class StikerCommand: Command
    {
        public int To { get; set; }
        public int Stiker { get; set; }
    }
    //переделать
    public class ButtonsCommand : Command
    {
        public int To { get; set; }
        public int Image { get; set; }
        public string Text { get; set; }
        public Dictionary<string, string> Entities { get; set; }
    }
    //переделать для команд на клавиатуре
    public class KeysCommand : Command
    {
        public int To { get; set; }
        public Dictionary<string, string> Entities { get; set; }
    }
}
