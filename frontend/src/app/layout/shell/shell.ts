import { Component, inject } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { UserRole } from '../../core/models';

/** Shell + role-aware nav (skeleton-plan.md §9). Wraps the dispatcher/technician route trees. */
@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {
  private readonly authService = inject(AuthService);

  readonly currentUser = this.authService.currentUser;
  readonly UserRole = UserRole;

  logout(): void {
    this.authService.logout();
  }
}
