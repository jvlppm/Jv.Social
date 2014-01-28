using Jv.Web.OAuth;

namespace Jv.Social.Google.Orkut
{
    public class Object : DynamicWrapper
    {
        internal Object(SafeObject obj)
            : base(obj)
        {
        }

        public string Id { get { return Object.id; } }
    }
}
