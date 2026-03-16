import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Product } from '../models/product';
import { AuthService } from './auth-service';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  
  private http = inject(HttpClient)
  private auth = inject(AuthService)
  private apiUrl = 'https://localhost:7119/api/Basket'

  items = signal<Product[]>([])
  private isRefreshing = false

  constructor() {
    this.refreshCart()
  }

  refreshCart() {
    const userId = this.auth.currentUser()?.id
    
    if (!userId || this.isRefreshing) return

    this.isRefreshing = true 

    this.http.get<Product[]>(`${this.apiUrl}/${userId}`)
      .subscribe({
        next: (data) => {
          this.items.set(data)
          this.isRefreshing = false
        },
        error: (err) => {
          this.isRefreshing = false
          console.error('Cart refresh failed', err)
        }
      })
  }

  addToCart(product: Product) {
    const user = this.auth.currentUser()
    if (!user) {
      this.showToast('Please login first', 'info')
      return
    }

    const dto = {
      userId: user.id?.toString(),
      backpackId: product.id,
      quantity: 1
    }

    this.http.post(`${this.apiUrl}/AddToBasket`, dto)
      .subscribe({
        next: () => {
          this.refreshCart()
          this.showToast('Added to cart!', 'success')
        }
      })
  }

  updateQuantity(basketItemId: number, newQuantity: number) {
    if (newQuantity < 1) return 
    this.http.put(`${this.apiUrl}/${basketItemId}?quantity=${newQuantity}`, {})
      .subscribe(() => this.refreshCart())
  }

  removeItem(basketItemId: number) {
    this.http.delete(`${this.apiUrl}/${basketItemId}`)
      .subscribe(() => {
        this.refreshCart()
        this.showToast('Removed from cart', 'info')
      })
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