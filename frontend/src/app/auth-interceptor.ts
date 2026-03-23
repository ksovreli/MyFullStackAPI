import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const userData = localStorage.getItem('currentUser')
  
  if (userData) {
    const user = JSON.parse(userData)
    const token = user.token

    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      })
    }
  }

  return next(req)
}