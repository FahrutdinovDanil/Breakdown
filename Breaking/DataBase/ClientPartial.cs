using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaking.DataBase
{
    public partial class Client
    {
        public override string ToString()
        {
            return $"{LastName} {FirstName} {Patronymic}";
        }
    }
}
