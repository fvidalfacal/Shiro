// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from Shiro INC. team.
//  
// Copyrights (c) 2014 Shiro INC. All rights reserved.

using Shiro.Class;

namespace Shiro.ComboBox
{
    internal sealed class ComboboxItemCustomer
    {
        public string Text { private get; set; }
        public Customer Value { get; set; }

        public override string ToString()
        {
            return Value.Name;
        }
    }
}