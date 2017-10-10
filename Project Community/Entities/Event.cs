using System;

namespace Entities
{
    /// <summary>
    /// An in game event 
    /// to be passed around.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class Event
    {
        /// <summary>
        /// Type of event
        /// </summary>
        public String type
        {
            get;
            set;
        }

        /// <summary>
        /// any data that is relevant to the event
        /// </summary>
        public Object _value
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_type">Type of event</param>
        /// <param name="__value">any data that is relevant to the event</param>
        public Event(String _type, Object __value)
        {
            type = _type;
            _value = __value;
        }


    }
}
