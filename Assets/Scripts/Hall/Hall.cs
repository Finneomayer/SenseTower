using System;
using Assets.Scripts.Space;

namespace Assets.Scripts.Hall
{
    public sealed class Hall
    {
        public Guid Id { get; set; } = default;

        public string Name { get; set; } = null!;

        public LocalSpace[] Spaces { get; set; } = Array.Empty<LocalSpace>();

        public LocalSpace Space { get; set; }
    }
}
