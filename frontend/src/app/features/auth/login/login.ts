import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { UserRole } from '../../../core/models';

/** Front door of the frontend's E2E path (skeleton-plan.md §13.4): a real form backed by the
 * real AuthService.login() call against the Server's /api/auth/login endpoint. */
@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly form = this.fb.nonNullable.group({
    username: ['', Validators.required],
    password: ['', Validators.required],
  });

  readonly features = [
    'Real-time incident tracking across all sources',
    'Multi-source ingestion: IoT, external systems & manual',
    'Dispatcher & Technician role-based workflows',
    'Complete audit trail and activity history',
  ];

  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly showPassword = signal(false);

  togglePassword(): void {
    this.showPassword.update((v) => !v);
  }

  submit(): void {
    if (this.form.invalid || this.submitting()) {
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);

    this.authService.login(this.form.getRawValue()).subscribe({
      next: (response) => {
        this.submitting.set(false);
        void this.router.navigateByUrl(response.role === UserRole.Dispatcher ? '/dispatcher' : '/technician');
      },
      error: (error: HttpErrorResponse) => {
        this.submitting.set(false);
        this.errorMessage.set(
          error.status === 401 ? 'Invalid username or password.' : `Login failed (${error.status}).`,
        );
      },
    });
  }
}
