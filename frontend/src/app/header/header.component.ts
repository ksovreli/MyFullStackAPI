import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../services/cart-service';
import { AuthService } from '../services/auth-service';
import { WishlistService } from '../services/wishlist-service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {

  menuOpen = false

  public router = inject(Router)
  public cartService = inject(CartService)
  public authService = inject(AuthService)
  public wishListService = inject(WishlistService)

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn()
  }

  cartCount(): number {
    return this.cartService.items().length
  }

  wishlistCount(): number {
    return this.wishListService.items().length
  }

  logout() {
    this.authService.logout()
    this.menuOpen = false
    this.router.navigateByUrl('/home')
  }

  scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  scrollTo(sectionId: string) {
    if (this.router.url === '/home' || this.router.url === '/') {
      const element = document.getElementById(sectionId)
      if (element) {
        element.scrollIntoView({ behavior: 'smooth' })
      }
    } else {
      this.router.navigate(['/home'], { fragment: sectionId })
    }
  }

  navigateToHome() {
    this.router.navigateByUrl('/home')
  }
}
