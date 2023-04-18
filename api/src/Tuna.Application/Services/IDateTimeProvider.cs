﻿using System;

namespace Tuna.Application.Services;

public interface IDateTimeProvider
{
    /// <summary>
    ///     The current time based on the date time provider.
    /// </summary>
    DateTime Now { get; }
}