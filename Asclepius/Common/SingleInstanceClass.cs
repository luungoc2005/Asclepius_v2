using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Asclepius.Common
{
    public class SingleInstanceClass
    {        
        private static volatile SingleInstanceClass _singletonInstance;
        private static Object _syncRoot = new Object();

        [XmlIgnoreAttribute]
        public static SingleInstanceClass Instance
        {
            get
            {
                if (_singletonInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singletonInstance == null)
                        {
                            _singletonInstance = new SingleInstanceClass();
                        }
                    }
                }
                return _singletonInstance;
            }
        }
    }
}
