using System;
using System.Runtime.Serialization;

namespace Alaris.Framework
{
    /// <summary>
    /// Class used to store information about IRC admins.
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class Admin
    {
        /// <summary>
        /// Id of the admin.
        /// </summary>
        [DataMember]
        public string Id;
        /// <summary>
        /// IRC nick of the admin.
        /// </summary>
        [DataMember]
        public string Nick;
        /// <summary>
        /// IRC username of the admin.
        /// </summary>
        [DataMember]
        public string User;
        /// <summary>
        /// IRC host of the admin.
        /// </summary>
        [DataMember]
        public string Host;
    }
}
