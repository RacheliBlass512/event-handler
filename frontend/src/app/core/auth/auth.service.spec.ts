import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { API_BASE_URL } from '../config/app-config';
import { LoginResponseDto, UserRole } from '../models';
import { AuthService } from './auth.service';

const STORAGE_KEY = 'eventhandler.auth';

function loginResponse(overrides: Partial<LoginResponseDto> = {}): LoginResponseDto {
  return {
    token: 'fake-token',
    role: UserRole.Dispatcher,
    displayName: 'Dana Dispatcher',
    expiresAt: new Date(Date.now() + 3600_000).toISOString(),
    ...overrides,
  };
}

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('login() posts credentials and stores the auth state', () => {
    service.login({ username: 'dispatcher', password: 'pw' }).subscribe();

    const req = httpMock.expectOne(`${API_BASE_URL}/api/auth/login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ username: 'dispatcher', password: 'pw' });
    req.flush(loginResponse());

    expect(service.getToken()).toBe('fake-token');
    expect(service.currentUser()?.displayName).toBe('Dana Dispatcher');
    expect(JSON.parse(localStorage.getItem(STORAGE_KEY)!).token).toBe('fake-token');
  });

  it('logout() clears the signal and localStorage', () => {
    service.login({ username: 'dispatcher', password: 'pw' }).subscribe();
    httpMock.expectOne(`${API_BASE_URL}/api/auth/login`).flush(loginResponse());

    service.logout();

    expect(service.getToken()).toBeNull();
    expect(localStorage.getItem(STORAGE_KEY)).toBeNull();
  });

  it('isAuthenticated() is false before login, true after, false once the token expires', () => {
    expect(service.isAuthenticated()).toBe(false);

    service.login({ username: 'dispatcher', password: 'pw' }).subscribe();
    httpMock.expectOne(`${API_BASE_URL}/api/auth/login`).flush(loginResponse());
    expect(service.isAuthenticated()).toBe(true);

    service.login({ username: 'dispatcher', password: 'pw' }).subscribe();
    httpMock
      .expectOne(`${API_BASE_URL}/api/auth/login`)
      .flush(loginResponse({ expiresAt: new Date(Date.now() - 1000).toISOString() }));
    expect(service.isAuthenticated()).toBe(false);
  });

  it('hasRole() matches the stored role only', () => {
    service.login({ username: 'tech1', password: 'pw' }).subscribe();
    httpMock
      .expectOne(`${API_BASE_URL}/api/auth/login`)
      .flush(loginResponse({ role: UserRole.Technician, displayName: 'Tom Technician' }));

    expect(service.hasRole(UserRole.Technician)).toBe(true);
    expect(service.hasRole(UserRole.Dispatcher)).toBe(false);
  });
});
