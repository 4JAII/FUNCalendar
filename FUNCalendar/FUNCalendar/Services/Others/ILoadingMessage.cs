using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNCalendar.Services
{
    public interface ILoadingMessage
    {
        
        void Show(string message);

        
        void Hide();

        
        bool IsShow { get; }
    }
}
