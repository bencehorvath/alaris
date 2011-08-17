using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Alaris.Framework.Config
{
    /// <remarks />

    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = "http://github.com/Twl/alaris")]
    [XmlRootAttribute(Namespace = "http://github.com/Twl/alaris", IsNullable = false, ElementName = "Alaris")]
    public class AlarisConfig
    {
        /// <remarks />
        public Config Config { get; set; }
    }

    /// <remarks />
    [Serializable]
    [DebuggerStepThrough]
    [XmlTypeAttribute(Namespace = "http://github.com/Twl/alaris")]
    public class Config
    {
        /// <remarks />
        public Irc Irc { get; set; }

        /// <remarks />
        public string Database { get; set; }

        /// <remarks />
        public Scripts Scripts { get; set; }

        /// <remarks />
        public Addons Addons { get; set; }

        /// <remarks />
        public Localization Localization { get; set; }

        /// <remarks />
        public Remote Remote { get; set; }

        /// <remarks />
        public CLI CLI { get; set; }
    }

    /// <remarks />
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = "http://github.com/Twl/alaris")]
    public class Irc
    {
        /// <remarks />
        public string Server { get; set; }

        /// <remarks />
        public string Nickname { get; set; }

        /// <remarks />
        public NickServ NickServ { get; set; }

        /// <remarks />
        public string Channels { get; set; }

        /// <remarks />
        public Admin Admin { get; set; }
    }

    /// <remarks />
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = "http://github.com/Twl/alaris")]
    public class NickServ
    {
        /// <remarks />
        public bool Enabled { get; set; }

        /// <remarks />
        public string Password { get; set; }
    }

    /// <remarks />
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = "http://github.com/Twl/alaris")]
    public class CLI
    {
        /// <remarks />
        public bool Enabled { get; set; }
    }

    /// <remarks />
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = "http://github.com/Twl/alaris")]
    public class Remote
    {
        /// <remarks />
        public int Port { get; set; }

        /// <remarks />
        public string Name { get; set; }

        /// <remarks />
        public string Password { get; set; }
    }

    /// <remarks />
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = "http://github.com/Twl/alaris")]
    public class Localization
    {
        /// <remarks />
        public string Locale { get; set; }
    }

    /// <remarks />
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = "http://github.com/Twl/alaris")]
    public class Addons
    {
        /// <remarks />
        public bool Enabled { get; set; }

        /// <remarks />
        public string Directory { get; set; }
    }

    /// <remarks />
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = "http://github.com/Twl/alaris")]
    public class Scripts
    {
        /// <remarks />
        public bool Lua { get; set; }

        /// <remarks />
        public string Directory { get; set; }

        /// <remarks />
        public string AdditionalReferences { get; set; }
    }

    /// <remarks />
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = "http://github.com/Twl/alaris")]
    public class Admin
    {
        /// <remarks />
        public string Nick { get; set; }

        /// <remarks />
        public string User { get; set; }

        /// <remarks />
        public string Host { get; set; }
    }
}