using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.Service
{
    public static class ServiceLocator
    {
        public static IServiceProvider Services { get; set; } = default!;
    }
}
