// Mirrors backend/src/EventHandler.Server/Api/Dtos/AuthDtos.cs.

import { UserRole } from './enums';

export interface LoginRequestDto {
  username: string;
  password: string;
}

export interface LoginResponseDto {
  token: string;
  role: UserRole;
  displayName: string;
  expiresAt: string;
}
