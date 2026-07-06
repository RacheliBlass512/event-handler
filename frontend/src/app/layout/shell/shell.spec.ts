import { signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AuthState } from '../../core/auth/auth.models';
import { AuthService } from '../../core/auth/auth.service';
import { UserRole } from '../../core/models';
import { Shell } from './shell';

describe('Shell', () => {
  let component: Shell;
  let fixture: ComponentFixture<Shell>;
  const currentUser = signal<AuthState | null>({
    token: 't',
    role: UserRole.Dispatcher,
    displayName: 'Dana Dispatcher',
    expiresAt: '',
  });

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Shell],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: { currentUser, logout: () => {} } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Shell);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('shows a single Events nav item for a dispatcher', () => {
    expect(component.navItems().map((i) => i.label)).toEqual(['Events']);
  });
});
