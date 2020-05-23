﻿using System;

namespace Gaia.FullSerializer
{
    /// <summary>
    /// Explicitly mark a property to be serialized. This can also be used to give the name that the
    /// property should use during serialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class fsPropertyAttribute : Attribute {
        /// <summary>
        /// The name of that the property will use in JSON serialization.
        /// </summary>
        public string Name;

        public fsPropertyAttribute()
            : this(string.Empty) {
        }

        public fsPropertyAttribute(string name) {
            Name = name;
        }
    }
}