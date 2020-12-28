using Netherite.Texts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Entities
{
    public interface ICommandSender
    {
        void SendMessage(Text text);

        void SendMessage(string message);
    }
}
