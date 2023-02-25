using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaking.DataBase
{
    public partial class BreakingEntities
    {
        private static BreakingEntities context;
        public static BreakingEntities GetContext()
        {
            if (context == null)
                context = new BreakingEntities();

            return context;
        }
    }
}
