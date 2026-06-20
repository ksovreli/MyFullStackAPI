import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../services/cart-service';
import { AuthService } from '../services/auth-service';
import { WishlistService } from '../services/wishlist-service';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../services/product-service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {

  menuOpen = false
  private productService = inject(ProductService)
  public router = inject(Router)
  public cartService = inject(CartService)
  public authService = inject(AuthService)
  public wishListService = inject(WishlistService)


  

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn()
  }

  isSearchOpen = false;
    searchQuery = '';
    searchResults: any[] = []; // აქ შეინახება ნაპოვნი ჩანთები

toggleSearch() {
        this.isSearchOpen = !this.isSearchOpen;
        if (!this.isSearchOpen) {
            this.searchQuery = '';
            this.searchResults = [];
        }
    }

    onSearchInput() {
        if (this.searchQuery.length > 1) {
            // ვფილტრავთ ჩანთებს სახელით
            this.productService.getProducts().subscribe(products => {
                this.searchResults = products
                    .filter(product => product?.name && product.name.toLowerCase().includes(this.searchQuery.toLowerCase()))
                    .slice(0, 5); // ვაჩვენებთ მხოლოდ პირველ 5 შედეგს
            });
        } else {
            this.searchResults = [];
        }
    }

    goToProduct(id: number) {
        this.router.navigate(['/product', id]);
        this.toggleSearch(); // ვხურავთ ძებნას გადასვლისას
    }

    viewAllResults() {
        this.router.navigate(['/products'], { queryParams: { q: this.searchQuery } });
        this.toggleSearch();
    }

  wishlistCount(): number {
    return this.wishListService.items().length
  }

    cartCount(): number {
    return this.cartService.items().length
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
    }
    else {
      this.router.navigate(['/home'], { fragment: sectionId })
    }
  }

  isAdmin(): boolean {
    return this.authService.getUserRole() === 'Admin'
  }

  navigateToHome() {
    this.router.navigateByUrl('/home')
  }
}
