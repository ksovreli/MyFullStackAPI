import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Product } from '../models/product';
import { AuthService } from './auth-service';
import { AlertService } from './alert-service';

@Injectable({
  providedIn: 'root',
})
export class WishlistService {

  private http = inject(HttpClient)
  private auth = inject(AuthService)
  private apiUrl = 'https://localhost:7119/api/Wishlist'
  private alertService = inject(AlertService)

  items = signal<Product[]>([])

  refreshWishlist() {
    const user = this.auth.currentUser()
    if (!user) {
      this.items.set([])
      return
    }

    this.http.get<Product[]>(`${this.apiUrl}/${user.id}`)
      .subscribe({
        next: (data) => {
          this.items.set(data)
        },
        error: (err) => console.error('Wishlist refresh failed', err)
      })
  }

  toggleWishlist(product: Product) {
    const user = this.auth.currentUser()

    if (!user) {
      this.alertService.success('Please log in to manage your wishlist.')
      return
    }

    const dto = {
      userId: user.id,
      backpackId: product.id
    }

    this.http.post(`${this.apiUrl}/toggle`, dto)
      .subscribe({
        next: (response: any) => {
          this.refreshWishlist()

          if (response.status === 'added') {
            this.alertService.success(`${product.name} added to favorites`)
          }
          else {
            this.alertService.success(`${product.name} removed from favorites`)
          }
        },
        error: (err) => {
          console.error('Toggle failed', err)
          this.alertService.success('Connection error. Try again later.')
        }
      })
  }

  isInWishlist(productId: number): boolean {
    return this.items().some(item => item.id === productId)
  }
}