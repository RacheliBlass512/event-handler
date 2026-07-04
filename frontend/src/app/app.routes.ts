import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';
import { UserRole } from './core/models';

// assign/transfer/status-update/notes are embeddable action components used from within
// event-detail, not standalone routes (skeleton-plan.md §9).
export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/auth/login/login').then((m) => m.Login) },
  {
    path: 'dispatcher',
    canActivate: [authGuard, roleGuard],
    data: { role: UserRole.Dispatcher },
    loadComponent: () => import('./layout/shell/shell').then((m) => m.Shell),
    children: [
      {
        path: '',
        loadComponent: () => import('./features/dispatcher/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'events',
        loadComponent: () => import('./features/dispatcher/event-list/event-list').then((m) => m.EventList),
      },
      {
        path: 'events/:id',
        loadComponent: () =>
          import('./features/dispatcher/event-detail/event-detail').then((m) => m.EventDetail),
      },
      {
        path: 'technicians',
        loadComponent: () =>
          import('./features/dispatcher/technician-status/technician-status').then((m) => m.TechnicianStatus),
      },
    ],
  },
  {
    path: 'technician',
    canActivate: [authGuard, roleGuard],
    data: { role: UserRole.Technician },
    loadComponent: () => import('./layout/shell/shell').then((m) => m.Shell),
    children: [
      {
        path: '',
        loadComponent: () => import('./features/technician/my-events/my-events').then((m) => m.MyEvents),
      },
      {
        path: 'events/:id',
        loadComponent: () =>
          import('./features/technician/event-detail/event-detail').then((m) => m.EventDetail),
      },
      {
        path: 'available',
        loadComponent: () =>
          import('./features/technician/request-available/request-available').then((m) => m.RequestAvailable),
      },
    ],
  },
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: '**', redirectTo: 'login' },
];
