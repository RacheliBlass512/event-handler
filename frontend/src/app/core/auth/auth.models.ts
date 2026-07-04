import { UserRole } from '../models';

/** Local session state — distinct from LoginResponseDto, the wire shape. */
export interface AuthState {
  token: string;
  role: UserRole;
  displayName: string;
  expiresAt: string;
}
