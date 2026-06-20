import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth-service';

export const loginGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // თუ მომხმარებელი შესულია (მაგ: ტოკენი არსებობს)
  if (authService.isLoggedIn()) {
    router.navigate(['/home']); 
    return false; // არ შეუშვა /login-ზე
  }

  return true; // თუ არ არის შესული, შეუშვი
};