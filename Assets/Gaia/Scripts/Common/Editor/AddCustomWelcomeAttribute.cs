// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using System;

namespace GaiaCommon1
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AddCustomWelcomeAttribute : Attribute
    {
        /// <summary>
        /// Mark the class as a Welcome class to implement custom welcome. Needs to inherit the IPWWelcome interface
        /// </summary>
        public AddCustomWelcomeAttribute() {}

        /// <summary>
        /// Optional parameter that affects the tab orders on the Welcome screen
        /// </summary>
        public int Priority;
    }
}
