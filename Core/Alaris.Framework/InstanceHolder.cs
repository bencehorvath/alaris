namespace Alaris.Framework
{
    /// <summary>
    /// Replacement of singleton.
    /// </summary>
    public static class InstanceHolder<T>
        where T: class
    {
        private static bool _instanceAssigned;
        private static T _instance;
        private readonly static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if(_instance == null)
                    Log.Warn("WARNING! Null instance returned by Instance Holder.");

                return _instance;
            }

            set
            {
                if (_instanceAssigned)
                    return;

                _instance = value;
                _instanceAssigned = true;
            }
        }

        /// <summary>
        /// Sets the instance to the specified object.
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static bool Set(T inst)
        {
            if (_instanceAssigned)
                return false;

            Instance = inst;
            return true;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static T Get()
        {
            return Instance;
        }
    }

    /// <summary>
    /// Providing extensions for instances.
    /// </summary>
    public static class InstanceExtensions
    {
        /// <summary>
        /// Gets the class' instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <returns></returns>
        public static T GetInstance<T>(this T ob)
            where T: class
        {
            return InstanceHolder<T>.Instance;
        }

        /// <summary>
        /// Sets the current object as the main instance of this type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static bool SetAsInstance<T>(this T inst)
            where T: class
        {
            return InstanceHolder<T>.Set(inst);
        }
    }
}
