import { HttpInterceptorFn } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const platformId = inject(PLATFORM_ID)

  if (!isPlatformBrowser(platformId)) {
    return next(req)
  }

  const userData = localStorage.getItem('currentUser')
  if (!userData) return next(req)

  try {
    const user = JSON.parse(userData)
    if (user?.token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${user.token}`
        }
      })
    }
  } catch (e) {
    console.error("AUTH_INTERCEPTOR_ERROR: Parse failed", e)
  }

  return next(req)
};