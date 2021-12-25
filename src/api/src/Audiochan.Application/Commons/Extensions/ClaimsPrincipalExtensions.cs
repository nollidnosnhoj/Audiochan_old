﻿using System;
using System.Security.Claims;

namespace Audiochan.Application.Commons.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool TryGetUserId(this ClaimsPrincipal? principal, out long userId)
        {
            var claim = principal?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is not null)
            {
                return long.TryParse(claim.Value, out userId);
            }
            
            userId = 0;
            return false;
        }
        
        public static bool TryGetUserName(this ClaimsPrincipal? principal, out string userName)
        {
            var claim = principal?.FindFirst(ClaimTypes.Name);

            if (claim is not null && !string.IsNullOrWhiteSpace(claim.Value))
            {
                userName = claim.Value;
                return true;
            }

            userName = "";
            return false;
        }

        public static long GetUserId(this ClaimsPrincipal? principal)
        {
            if (principal is null) throw new ArgumentNullException(nameof(principal));
            
            if (!principal.TryGetUserId(out var userId))
            {
                throw new ArgumentException("ClaimsPrincipal does not contain userId", nameof(principal));
            }

            return userId;
        }

        public static string GetUserName(this ClaimsPrincipal? principal)
        {
            if (principal is null) throw new ArgumentNullException(nameof(principal));

            if (!principal.TryGetUserName(out var userName))
            {
                throw new ArgumentException("ClaimsPrincipal does not contain username", nameof(principal));
            }

            return userName;
        }
    }
}