using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace QuickGraph //.Alt
{
    /// <summary>
    /// The struct implementation. It doesn't implement <see cref="IEdge&lt;TVertex&gt;"/> 
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
    [DebuggerDisplay("{Source}->{Target}")]
    public struct Edge<TVertex>
    {
        public readonly TVertex Source;
        public readonly TVertex Target;

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge&lt;TVertex&gt;"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public Edge(TVertex source, TVertex target)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (target == null) throw new ArgumentNullException("target");

            Source = source;
            Target = target;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return Source + "->" + Target;
        }

        public override bool Equals(object obj)
        {
            var otherStr = obj?.ToString();
            return !string.IsNullOrWhiteSpace(otherStr) &&
                (string.Equals($"{Source}->{Target}", otherStr)
                || string.Equals($"{Target}->{Source}", otherStr));
        }

        public override int GetHashCode()
        {
            return Source.GetHashCode() + Target.GetHashCode();
        }
    }
}
