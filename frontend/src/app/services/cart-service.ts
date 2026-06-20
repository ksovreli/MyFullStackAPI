import { inject, Injectable, signal, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Product } from '../models/product';
import { AuthService } from './auth-service';
import Swal from 'sweetalert2';
import { BasketItem } from '../models/basketitem';
import { AlertService } from './alert-service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class CartService {

  private http = inject(HttpClient)
  private auth = inject(AuthService)
  private apiUrl = 'https://apex-store-api-aj1b.onrender.com/api/Basket';
  private alertService = inject(AlertService)
  public router = inject(Router)

  items = signal<BasketItem[]>([])
  private isRefreshing = false

  constructor() {
    effect(() => {
      const user = this.auth.currentUser()
      if (user) {
        this.refreshCart()
      }
      else {
        this.items.set([])
      }
    })
  }

  refreshCart() {
    const user = this.auth.currentUser()
    if (!user) {
      this.items.set([])
      return
    }

    if (this.isRefreshing) return

    this.isRefreshing = true

    this.http.get<BasketItem[]>(`${this.apiUrl}/${user.id}`)
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

    if (!user?.id) {
      this.alertService.confirm('Login Required', 'You must be logged in to add items to your cart. Do you want to login now?')
      .then(result => {
        if (result.isConfirmed) this.router.navigateByUrl('/login')
      })
      return;
    }
    const dto = {
      userId: user.id,
      backpackId: product.id,
      quantity: 1
    }

    this.http.post(`${this.apiUrl}/AddToBasket`, dto).subscribe({
      next: () => {
        this.refreshCart()
        this.alertService.success(`${product.name?.toUpperCase()} added to inventory.`)
      },
      error: (err) => {
        console.error('Add to cart failed', err)
        this.alertService.error('DATABASE_SYNC_FAILED: Item not added.')
      }
    })
  }

  updateQuantity(backpackId: number, newQuantity: number) {
    const user = this.auth.currentUser()
    if (!user) {
      return
    }

    const url = `${this.apiUrl}/${backpackId}/${user.id}?quantity=${newQuantity}`

    this.http.put(url, {}).subscribe({
      next: () => this.refreshCart(),
      error: (err) => console.error(err)
    })
  }

  removeItem(backpackId: number) {
    const user = this.auth.currentUser()
    if (!user) {
      return
    }

    const url = `${this.apiUrl}/${backpackId}/${user.id}`
    this.http.delete(url).subscribe({
      next: () => this.refreshCart(),
      error: (err) => console.error(err)
    })
  }
}