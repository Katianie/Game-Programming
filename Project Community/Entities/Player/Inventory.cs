using System;
using System.Collections.Generic;

namespace Entities.Player
{
    /// <summary>
    /// Its basically a dictionary of items.  Names as keys and amounts held as values.  
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class Inventory : Dictionary<String, int>
    {
        public List<String> lastThree;
        /// <summary>
        /// Constructor
        /// </summary>
        public Inventory()
        {
            lastThree = new List<string>();
        }

        /// <summary>
        /// Adds items to inventory.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        public void AddItems(String name, int count)
        {
            if ( ContainsKey(name))
                this[name] += count;
            else
                 Add(name,count);

            if(!lastThree.Contains(name))
            {
                lastThree.Add(name);
                if(lastThree.Count > 3)
                    lastThree.RemoveAt(0);
            }
        }

        /// <summary>
        /// Tries to remove items
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        /// <returns>False if there are not enough</returns>
        public bool takeItems(String name, int count)
        {
            if (!lastThree.Contains(name))
            {
                lastThree.Add(name);
                if (lastThree.Count > 3)
                    lastThree.RemoveAt(0);
            }

            if (! ContainsKey(name))
                return false;
            if (this[name] >= count)
                this[name] -= count;
            else
                return false;
            return true;
        }

        /// <summary>
        /// Tries to remove all items from a category
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        /// <returns>False if nothing is removed</returns>
        public bool removeAllItems(String name)
        {
            if (ContainsKey(name))
            {
                if (this[name] > 0)
                {
                    this[name] = 0;
                    return true;
                }
            }
            return false;
        }
    }
}
