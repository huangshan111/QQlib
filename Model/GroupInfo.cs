using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQLib.Model
{
    public class GroupInfo
    {
        public List<Group> create { get; set; }
        public List<Group> join { get; set; }
        public int ec { get; set; }
    }

    public class Group
    { 
        public string gc{get;set;}
        public string gn {get;set;}
        public string owner{get;set;}
    }
}
