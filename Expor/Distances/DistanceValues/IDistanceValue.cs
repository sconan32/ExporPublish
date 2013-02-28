using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Socona.Expor.Distances.DistanceValues
{

    /// <summary>
    /// The interface Distance defines the requirements of any instance class.
    /// </summary>
    ///<see cref="Socona.Expor.Distances.DistanceUtil"/>
    public interface IDistanceValue : IComparable, IComparable<IDistanceValue>
    {

        /// <summary>
        /// Provides a measurement suitable to this measurement function based on the given pattern.
        /// </summary>
        /// <param name="pattern"> pattern a pattern defining a similarity suitable to this measurement function</param>
        /// <returns>a measurement suitable to this measurement function based on the  given pattern</returns>
        IDistanceValue ParseString(String pattern);

        /// <summary>
        /// Get string as description of the required input format.
        /// </summary>
        String RequiredInputPattern { get; }


        /// <summary>
        /// Get the infinite distance.
        /// </summary>
        IDistanceValue Infinity { get; }

        /// <summary>
        /// Get the null distance
        /// </summary>
        IDistanceValue Empty { get; }

        /// <summary>
        /// Get the undefined distance.
        /// </summary>
        IDistanceValue Undefined { get; }

        /// <summary>
        /// Returns true, if the distance is an infinite distance, false otherwise.
        /// </summary>
        bool IsInfinity { get; }


        /// <summary>
        /// Returns true, if the distance is a null distance, false otherwise.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Returns true, if the distance is an undefined distance, false otherwise.
        /// </summary>
        bool IsUndefined { get; }

        /// <summary>
        /// Get the double form of a distance 
        /// <para>a valid double value if the convertion succeeded, or throws exception.</para>
        /// </summary>
        /// <returns>the double form of a distance </returns>
        double ToDouble();
    }
}
