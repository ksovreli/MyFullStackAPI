import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/user';
import { Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient)
  private platformId = inject(PLATFORM_ID)
  private apiUrl = `${environment.apiUrl}/auth`;

  currentUser = signal<User | null>(this.getStoredUser())
  isLoggedIn = signal<boolean>(!!this.currentUser())

  constructor() {

  }

  getUsername(): string {
    return this.currentUser()?.username || ''
  }

  private getStoredUser(): User | null {
    if (isPlatformBrowser(this.platformId)) {
      const userJson = localStorage.getItem('currentUser')
      if (!userJson) return null

      try {
        return JSON.parse(userJson) as User
      }
      catch {
        localStorage.removeItem('currentUser')
        return null
      }
    }

    return null
  }
  getToken(): string | null {
    return this.currentUser()?.token || null
  }

  login(credentials: any): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/login`, credentials).pipe(
      tap(user => this.saveSession(user))
    )
    
  }

  // auth-service.ts
register(credentials: any): Observable<User> {
  const headers = { 'Content-Type': 'application/json' };
  return this.http.post<User>(`${this.apiUrl}/register`, credentials, { headers });
}

  verifyEmail(email: string, token: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/confirm-email`, {
      params: { email, token }
    })
  }

  forgotPassword(email: string) {
    return this.http.post(`${this.apiUrl}/forgot-password`, { Email: email });
  }

  resetPassword(resetData: any) {
    const payload = {
      Email: resetData.email,
      Token: resetData.token,
      NewPassword: resetData.newPassword
    };
    return this.http.post(`${this.apiUrl}/reset-password`, payload);
  }
  deleteAccount(): Observable<any> {
    return this.http.delete(`${this.apiUrl}/terminate-account`).pipe(
      tap(() => {
        this.logout();
      })
    )
  }

  getUserRole(): string | null {
    const token = this.getToken()
    if (!token) return null

    try {
      const decoded: any = jwtDecode(token);
      const roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

      return decoded[roleClaim] || decoded['role'] || null
    }
    catch (error) {
      return null
    }
  }

  private saveSession(user: User) {
    localStorage.setItem('currentUser', JSON.stringify(user))
    this.currentUser.set(user)
    this.isLoggedIn.set(true)
  }

  logout() {
    localStorage.removeItem('currentUser')
    this.currentUser.set(null)
    this.isLoggedIn.set(false)
  }
}