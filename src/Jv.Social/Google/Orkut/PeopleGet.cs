using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Google.Orkut
{
    public class PeopleGet : Get<Page<Person>>
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

        public int Count
        {
            get { return int.Parse(Parameters.GetField("count"), CultureInfo.InvariantCulture); }
            set { Parameters.Set("count", value.ToString(CultureInfo.InvariantCulture)); }
        }

        public int StartIndex
        {
            get { return int.Parse(Parameters.GetField("startIndex"), CultureInfo.InvariantCulture); }
            set { Parameters.Set("startIndex", value.ToString(CultureInfo.InvariantCulture)); }
        }

        public PeopleGet(string userId, string groupId, int count, int startIndex) : base("people.get")
        {
            if (groupId == GroupIds.Self)
                throw new ArgumentException("groupId can't be " + GroupIds.Self + ". Use PersonGet instead", "groupId");

            UserId = userId;
            GroupId = groupId;
            Count = count;
            StartIndex = startIndex;
        }
    }
}
