using System;
using System.Collections.Generic;
using System.Text;

namespace InversedIndex
{
    /// <summary>
    /// Abstract class defining identifyable interface.
    /// </summary>
    [Serializable]
    public abstract class Identifyable
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Identifyable()
        {
            mId = msId;
            msId++;
        }

        /// <summary>
        /// Constructor.
        /// 
        /// WARNING
        /// Identifyable will not care for unique id
        /// if this constructor used.
        /// </summary>
        /// <param name="id">Id of an object.</param>
        public Identifyable(uint id)
        {
            mId = id;
        }

        /// <summary>
        /// Gets id of an object.
        /// </summary>
        public uint Id
        {
            get
            {
                return mId;
            }
        }

        /// <summary>
        /// Identifier of an object.
        /// </summary>
        private uint mId;

        /// <summary>
        /// Next identifier to assign.
        /// </summary>
        static uint msId;
    }
}
