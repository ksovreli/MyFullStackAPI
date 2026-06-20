import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const verifyEmailGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  
  // ვიღებთ Query პარამეტრებს: ?token=...&email=...
  const token = route.queryParamMap.get('token');
  const email = route.queryParamMap.get('email');

  if (token && email) {
    return true; // თუ ორივე გვაქვს, შევუშვათ
  }

  // თუ პარამეტრები აკლია, დავაბრუნოთ Login-ზე ან Home-ზე
  console.warn("SYSTEM_ACCESS_DENIED: Missing verification parameters.");
  router.navigate(['/login']);
  return false;
};