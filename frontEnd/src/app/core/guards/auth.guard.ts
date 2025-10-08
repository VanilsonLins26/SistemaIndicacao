import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { map, filter, take } from 'rxjs/operators';
import { toObservable } from '@angular/core/rxjs-interop';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return toObservable(authService.currentUser).pipe(
    
    filter(user => user !== undefined), 
    
    take(1), 
    
    map(user => {
      if (user) {
        return true;
      }
      
      return router.createUrlTree(['/login']);
    })
  );
};