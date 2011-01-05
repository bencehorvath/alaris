using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alaris.Framework
{
    ///<summary>
    /// Base abstract class for Alaris-based bots.
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public abstract class AlarisBase<T>
        where T: class, new()
    {

    }
}
