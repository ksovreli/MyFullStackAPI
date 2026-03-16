import { Component, computed, inject, } from '@angular/core';
import { CartService } from '../services/cart-service';
import { RouterModule } from '@angular/router';
import Swal from 'sweetalert2';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './cart.html',
  styleUrl: './cart.scss',
})
export class Cart {

  public cartService = inject(CartService)
  
  items = this.cartService.items

  total = computed(() => {
    return this.items().reduce((acc, item) => {
      const activePrice = item.salePrice ?? item.price ?? 0
      return acc + (activePrice * (item.quantity ?? 1))
    }, 0)
  })

  ngOnInit() {
    this.cartService.refreshCart()
  }

  changeQuantity(item: any, delta: number) {
    const newQty = (item.quantity ?? 1) + delta
    if (newQty > 0) {
      this.cartService.updateQuantity(item.id, newQty)
    }
  }

  removeItem(id: number) {
    Swal.fire({
      title: 'Are you sure?',
      text: "This item will be removed from your gear.",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#121212',
      cancelButtonColor: '#ff4d4d',
      confirmButtonText: 'Yes, remove it',
      background: '#121212',
      color: '#fff'
    }).then((result) => {
      if (result.isConfirmed) {
        this.cartService.removeItem(id)
      }
    })
  }
}