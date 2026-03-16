import { Component, inject } from '@angular/core';
import { WishlistService } from '../services/wishlist-service';
import { Product } from '../models/product';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../services/cart-service';
import { AuthService } from '../services/auth-service';

@Component({
  selector: 'app-wishlist',
  imports: [RouterModule],
  templateUrl: './wishlist.html',
  styleUrl: './wishlist.scss',
})
export class Wishlist {
  public wishlistService = inject(WishlistService)
  public cartService = inject(CartService)
  private authService = inject(AuthService)
  public router = inject(Router)

  items = this.wishlistService.items

  ngOnInit() {
    this.wishlistService.refreshWishlist()
  }

  moveToCart(product: Product) {
    if (!this.authService.isLoggedIn()) {
      this.router.navigateByUrl('/login')
      return
    }

    this.cartService.addToCart(product)
    
    this.wishlistService.toggleWishlist(product)
  }

  removeItem(product: Product) {
    this.wishlistService.toggleWishlist(product)
  }
}
