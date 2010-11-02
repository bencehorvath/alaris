using System;
using System.Runtime.Serialization;

namespace Alaris.API
{
    [Serializable]
    [DataContract]
    public sealed class Admin
    {
        [DataMember]
        public string Id;
        [DataMember]
        public string Nick;
        [DataMember]
        public string User;
        [DataMember]
        public string Host;
    }
}
