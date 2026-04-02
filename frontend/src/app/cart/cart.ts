import { Component, computed, inject } from '@angular/core';
import { CartService } from '../services/cart-service';
import { RouterModule } from '@angular/router';
import Swal from 'sweetalert2';
import { CommonModule } from '@angular/common';
import { BasketItem } from '../models/basketitem';

@Component({
  selector: 'app-cart',
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


  changeQuantity(item: BasketItem, delta: number) {
    const newQty = item.quantity + delta
    if (newQty > 0) {
      this.cartService.updateQuantity(item.backpackId, newQty)
    }
  }

  removeItem(backpackId: number) {
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
        this.cartService.removeItem(backpackId)
      }
    })
  }
}