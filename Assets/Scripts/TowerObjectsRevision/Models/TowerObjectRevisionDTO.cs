using System;
using Assets.Scripts.TowerObjectsClass.Models;

namespace Assets.Scripts.TowerObjectsRevision.Models
{
    public class TowerObjectRevisionDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TowerObjectClass ObjectClass { get; set; }
        public Owner Owner { get; set; }
    
    }
}