﻿namespace Audiochan.Core.Common.Interfaces
{
    public interface IHasPage
    {
        public int Page { get; init; }
        public int Size { get; init; }
    }
}