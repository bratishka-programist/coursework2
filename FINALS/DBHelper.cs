using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FINALS
{
    public static class DB
    {
            private static readonly THEFINALSDBEntities context = new THEFINALSDBEntities();
            public static THEFINALSDBEntities Context => context;
    }
}
