﻿// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from Shiro INC. team.
//  
// Copyrights (c) 2014 Shiro INC. All rights reserved.

namespace Shiro.Class
{
    internal static class Date
    {
        /// <summary>
        /// Convertis la date du format français à US
        /// </summary>
        /// <param name="date"></param>
        /// <returns>date au format US</returns>
        internal static string Fr2Us(string date)
        {
            var dateUs = date.Split(new[] {'/'});
            return string.Format("{0}-{1}-{2}", dateUs[2], dateUs[1], dateUs[0]);
        }

        /// <summary>
        /// Convertis la date du format US à français
        /// </summary>
        /// <param name="date"></param>
        /// <returns>date au format français</returns>
        internal static string Us2Fr(string date)
        {
            var dateUs = date.Split(new[] {'-'});
            return string.Format("{0}/{1}/{2}", dateUs[2], dateUs[1], dateUs[0]);
        }
    }
}