import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login.component')
                                .then(c => c.LoginComponent)
    },
    {
         path: 'register',
        loadComponent: () => import('./features/auth/register/register.component')
                                .then(c => c.RegisterComponent)
    },
    {
        path: 'perfil',
        canActivate: [authGuard],
        loadComponent: () => import('./features/profile/profile.component')
                                 .then(c => c.ProfileComponent)
    },
    {path: '', redirectTo: 'perfil', pathMatch: 'full'},
    {path: '**', redirectTo: "perfil"}
];
