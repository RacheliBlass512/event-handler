import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserRole } from '../models';
import { AuthService } from './auth.service';

/** Screen-per-role check via route `data: { role: UserRole.X }`. UX convenience only — real
 * enforcement is server-side (skeleton-plan.md §9). */
export const roleGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const requiredRole = route.data['role'] as UserRole | undefined;

  return !requiredRole || authService.hasRole(requiredRole) ? true : router.parseUrl('/login');
};
