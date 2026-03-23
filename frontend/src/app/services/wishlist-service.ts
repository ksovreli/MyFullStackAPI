import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Product } from '../models/product';
import { AuthService } from './auth-service';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root',
})
export class WishlistService {

  private http = inject(HttpClient)
  private auth = inject(AuthService)
  private apiUrl = 'https://localhost:7119/api/Wishlist'

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
      this.showToast('Please login to save favorites', 'info')
      return
    }

    const dto = { 
      userId: user.id?.toString(), 
      backpackId: product.id 
    }

    this.http.post(`${this.apiUrl}/toggle`, dto)
      .subscribe({
        next: (response: any) => {
          this.refreshWishlist()
          const msg = response.status === 'added' ? 'Added to wishlist!' : 'Removed from wishlist'
          const icon = response.status === 'added' ? 'success' : 'info'
          this.showToast(msg, icon)
        },
        error: (err) => console.error('Toggle failed', err)
      })
  }

  isInWishlist(productId: number): boolean {
    return this.items().some(item => item.id === productId)
  }

  private showToast(title: string, icon: 'success' | 'info') {
    Swal.fire({
      title: title,
      icon: icon,
      background: '#121212',
      color: '#fff',
      timer: 1500,
      showConfirmButton: false,
      toast: true,
      position: 'top-end'
    })
  }
}