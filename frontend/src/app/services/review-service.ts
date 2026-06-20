import { inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Review } from '../models/review';
import { AuthService } from './auth-service';
import { first, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ReviewService {
  private http = inject(HttpClient)
  private auth = inject(AuthService)
  private apiUrl = 'https://localhost:7119/api/Review'

  reviews = signal<Review[]>([])

  private getHeaders() {
    const token = this.auth.getToken()
    return new HttpHeaders().set('Authorization', `Bearer ${token}`)
  }
  loadReviews(backpackId: number) {
    return this.http.get<Review[]>(`${this.apiUrl}/backpack/${backpackId}`)
      .pipe(
        tap(data => this.reviews.set(data)),
        first()
      )
  }

  postReview(payload: any) {
    return this.http.post<Review>(this.apiUrl, payload, { headers: this.getHeaders() })
  }

  updateReview(id: number, payload: any) {
    return this.http.put(`${this.apiUrl}/${id}`, payload, { headers: this.getHeaders() })
  }

  deleteReview(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() })
  }
}