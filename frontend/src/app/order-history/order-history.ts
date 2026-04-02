import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CheckoutService } from '../services/checkout-service';
import { AlertService } from '../services/alert-service';
import { Order } from '../models/order';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './order-history.html',
  styleUrl: './order-history.scss',
})
export class OrderHistory implements OnInit {
  private orderService = inject(CheckoutService)
  private alertService = inject(AlertService)

  orders = signal<Order[]>([])
  isLoading = signal<boolean>(true)

  ngOnInit() {
    this.loadHistory()
  }

  loadHistory() {
    this.isLoading.set(true)
    
    this.orderService.getUserOrderHistory().subscribe({
      next: (data) => {
        this.orders.set(data)
        this.isLoading.set(false)
      },
      error: (err) => {
        console.error(err)
        this.alertService.error('System Error: Access Denied to Database.')
        this.isLoading.set(false)
      }
    })
  }

 // Inside your OrderHistory class
selectedOrder = signal<Order | null>(null)

viewDetails(orderId: number) {
  const match = this.orders().find(o => o.id === orderId)
  if (match) {
    this.selectedOrder.set(match)
  }
}

closeLogs() {
  this.selectedOrder.set(null)
}
}