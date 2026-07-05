import { HttpErrorResponse } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { vi } from 'vitest';
import { AuthService } from '../../../core/auth/auth.service';
import { UserRole } from '../../../core/models';
import { Login } from './login';

describe('Login', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;
  let login: ReturnType<typeof vi.fn>;
  let navigateByUrl: ReturnType<typeof vi.fn>;

  beforeEach(async () => {
    login = vi.fn();
    navigateByUrl = vi.fn();

    await TestBed.configureTestingModule({
      imports: [Login],
      providers: [
        { provide: AuthService, useValue: { login } },
        { provide: Router, useValue: { navigateByUrl } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('does not call authService.login when the form is invalid', () => {
    component.form.setValue({ username: '', password: '' });

    component.submit();

    expect(login).not.toHaveBeenCalled();
  });

  it('navigates to /dispatcher on successful dispatcher login', () => {
    component.form.setValue({ username: 'dispatcher', password: 'pw' });
    login.mockReturnValue(of({ token: 't', role: UserRole.Dispatcher, displayName: 'Dana', expiresAt: '' }));

    component.submit();

    expect(navigateByUrl).toHaveBeenCalledWith('/dispatcher');
    expect(component.submitting()).toBe(false);
  });

  it('navigates to /technician on successful technician login', () => {
    component.form.setValue({ username: 'tech1', password: 'pw' });
    login.mockReturnValue(of({ token: 't', role: UserRole.Technician, displayName: 'Tom', expiresAt: '' }));

    component.submit();

    expect(navigateByUrl).toHaveBeenCalledWith('/technician');
  });

  it('shows an invalid-credentials message and stops submitting on a 401', () => {
    component.form.setValue({ username: 'dispatcher', password: 'wrong' });
    login.mockReturnValue(throwError(() => new HttpErrorResponse({ status: 401 })));

    component.submit();

    expect(component.submitting()).toBe(false);
    expect(component.errorMessage()).toBe('Invalid username or password.');
    expect(navigateByUrl).not.toHaveBeenCalled();
  });

  it('shows a generic error message and stops submitting on other failures', () => {
    component.form.setValue({ username: 'dispatcher', password: 'pw' });
    login.mockReturnValue(throwError(() => new HttpErrorResponse({ status: 500 })));

    component.submit();

    expect(component.submitting()).toBe(false);
    expect(component.errorMessage()).toContain('500');
    expect(navigateByUrl).not.toHaveBeenCalled();
  });
});
