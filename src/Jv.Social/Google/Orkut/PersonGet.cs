using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Google.Orkut
{
    public class PersonGet : Get<Person>
    {
        public string UserId
        {
            get { return Parameters.GetField("userId"); }
            set { Parameters.Set("userId", value); }
        }

        public string GroupId
        {
            get { return Parameters.GetField("groupId"); }
            set { Parameters.Set("groupId", value); }
        }

        public PersonGet(string userId)
            : base("people.get")
        {
            UserId = userId;
            GroupId = GroupIds.Self;
        }
    }
}
