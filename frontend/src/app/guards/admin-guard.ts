import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth-service';

export const adminGuard = () => {
  const authService = inject(AuthService)
  const router = inject(Router)
  const role = authService.getUserRole()

  if (authService.isLoggedIn() && role === 'Admin') {
    return true
  }

  console.warn("UNAUTHORIZED_ACCESS_ATTEMPT: Redirecting to 404")
  router.navigate(['/404'])
  return false
};