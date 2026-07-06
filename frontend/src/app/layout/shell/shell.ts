import { Component, computed, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { UserRole } from '../../core/models';
import { Avatar } from '../../shared/ui';

interface NavItem {
  label: string;
  icon: string;
  link: string;
}

/** Shell + role-aware sidebar nav (skeleton-plan.md §9). Wraps the dispatcher/technician route trees. */
@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, Avatar],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly currentUser = this.authService.currentUser;

  readonly navItems = computed<NavItem[]>(() =>
    this.currentUser()?.role === UserRole.Dispatcher
      ? [{ label: 'Events', icon: 'grid_view', link: '/dispatcher/events' }]
      : [
          { label: 'My Events', icon: 'assignment', link: '/technician' },
          { label: 'Available', icon: 'notifications_active', link: '/technician/available' },
        ],
  );

  logout(): void {
    this.authService.logout();
    void this.router.navigateByUrl('/login');
  }
}
