﻿namespace JWT.Api.Endpoints.Dtos;

internal record RefreshTokenRequest(string Token, string RefreshToken);