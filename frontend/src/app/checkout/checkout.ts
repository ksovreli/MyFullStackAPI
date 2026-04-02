import { Component, computed, inject } from '@angular/core';
import { CartService } from '../services/cart-service';
import { CheckoutService } from '../services/checkout-service';
import { AuthService } from '../services/auth-service';
import { AlertService } from '../services/alert-service';
import { Router, RouterLink } from '@angular/router';
import { Location } from '@angular/common';
import { OrderRequest } from '../models/order';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss',
})
export class Checkout {

  private cartService = inject(CartService)
  private checkoutService = inject(CheckoutService)
  private authService = inject(AuthService)
  private alertService = inject(AlertService)
  private router = inject(Router)
  private location = inject(Location)

  cartItems = this.cartService.items
  user = this.authService.currentUser

  shippingInfo = {
    address: '',
    city: '',
    phone: ''
  }

  total = computed(() => {
    return this.cartItems().reduce((acc, item) => acc + (item.price * item.quantity), 0)
  })

  async processCheckout() {
    const currentUser = this.user()
    
    if (!currentUser?.id) {
      this.alertService.error('Session expired. Re-authenticating...')
      this.router.navigateByUrl('/login')
      return
    }

    const orderPayload: OrderRequest = {
      userId: currentUser.id,
      shippingAddress: `${this.shippingInfo.address}, ${this.shippingInfo.city}`,
      phoneNumber: this.shippingInfo.phone,
      totalAmount: this.total(),
      items: this.cartItems().map(item => ({
        backpackId: item.backpackId,
        quantity: item.quantity,
        price: item.price
      }))
    }

    const result = await this.alertService.confirm(
      'Confirm Your Order',
      `You're about to place an order totaling $${orderPayload.totalAmount}. Do you want to proceed?`
    )

    if (result.isConfirmed) {
      this.checkoutService.placeOrder(orderPayload).subscribe({
        next: () => {
          this.alertService.success('Order Successful. Gear is on the way.')
          this.cartService.refreshCart() 
          this.router.navigateByUrl('/order-history')
        },
        error: (err) => {
          console.error(err)
          this.alertService.error('System Error: Transaction aborted.')
        }
      })
    }
  }

  goBack(){
    this.location.back()
  }
}