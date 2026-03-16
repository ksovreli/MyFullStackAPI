import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/user';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient)
  private apiUrl = 'https://localhost:7119/api/Auth'

  isLoggedIn = signal<boolean>(!!localStorage.getItem('currentUser'))
  currentUser = signal<User | null>(this.getStoredUser())

  constructor() {
    
    const user = this.getStoredUser()
    if (user) {
      this.currentUser.set(user)
      this.isLoggedIn.set(true)
    }
  }

  private getStoredUser(): User | null {
    const user = localStorage.getItem('currentUser')
    try {
      return user ? JSON.parse(user) : null
    } catch {
      return null
    }
  }

  login(credentials: any): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/login`, credentials).pipe(
      tap(user => {
        this.saveSession(user)
      })
    )
  }

  register(credentials: any): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/register`, credentials).pipe(
      tap(user => {
        this.saveSession(user)
      })
    )
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