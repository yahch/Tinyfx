using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinyfx.Common
{
    public class TStorage
    {
        private static readonly TStorage instance = new TStorage();

        private static Dictionary<string, object> dictionary;

        public object this[string key]
        {
            get
            {
                try
                {
                    return dictionary[key];
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    if (dictionary == null) dictionary = new Dictionary<string, object>();
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, value);
                    }
                    else
                    {
                        dictionary[key] = value;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private TStorage()
        {

        }

        public static TStorage GetInstance()
        {
            return instance;
        }
    }
}
