﻿// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from Shiro INC. team.
//  
// Copyrights (c) 2014 Shiro INC. All rights reserved.

using System.Windows.Controls;

namespace Shiro.Class
{
    internal sealed class SalesMan
    {
        public SalesMan(int id, string telephone, string name, string firstname, string mail)
        {
            IdSalesman = id;
            Telephone = telephone;
            Name = name;
            Firstname = firstname;
            Mail = mail;
        }

        public int IdSalesman { get; private set; }
        public string Firstname { get; private set; }
        public string Name { get; private set; }
        public string Mail { get; set; }
        public string Telephone { get; set; }
        public Border Border { get; set; }
    }
}