using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesAzureADTest.Graph
{
    public class GraphConstants
    {
        // Defines the permission scopes used by the app
        public readonly static string[] Scopes =
        {
            "User.Read",
            "GroupMember.Read.All",
            "email",
            "profile"
        };
    }
}
