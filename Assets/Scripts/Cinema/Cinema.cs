using System;
using Assets.Scripts.Client;
using Assets.Scripts.Space;

namespace Assets.Scripts.Cinema
{
    public class Cinema
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public LocalSpace Space { get; set; }

        public UserInfo[] Administrators { get; set; }
    }
}