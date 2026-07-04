import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { API_BASE_URL } from '../config/app-config';
import { LoginRequestDto, LoginResponseDto, UserRole } from '../models';
import { AuthState } from './auth.models';

const TOKEN_STORAGE_KEY = 'eventhandler.auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly state = signal<AuthState | null>(readStoredState());

  readonly currentUser = this.state.asReadonly();

  constructor(private readonly http: HttpClient) {}

  login(request: LoginRequestDto): Observable<LoginResponseDto> {
    return this.http
      .post<LoginResponseDto>(`${API_BASE_URL}/api/auth/login`, request)
      .pipe(tap((response) => this.setState(response)));
  }

  logout(): void {
    this.state.set(null);
    localStorage.removeItem(TOKEN_STORAGE_KEY);
  }

  getToken(): string | null {
    return this.state()?.token ?? null;
  }

  isAuthenticated(): boolean {
    const state = this.state();
    return state !== null && Date.parse(state.expiresAt) > Date.now();
  }

  hasRole(role: UserRole): boolean {
    return this.state()?.role === role;
  }

  private setState(response: LoginResponseDto): void {
    const state: AuthState = {
      token: response.token,
      role: response.role,
      displayName: response.displayName,
      expiresAt: response.expiresAt,
    };
    this.state.set(state);
    localStorage.setItem(TOKEN_STORAGE_KEY, JSON.stringify(state));
  }
}

function readStoredState(): AuthState | null {
  const raw = localStorage.getItem(TOKEN_STORAGE_KEY);
  return raw ? (JSON.parse(raw) as AuthState) : null;
}
