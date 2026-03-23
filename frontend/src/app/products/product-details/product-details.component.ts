import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product-service';
import { Product } from '../../models/product';
import { CartService } from '../../services/cart-service';
import { AuthService } from '../../services/auth-service';
import Swal from 'sweetalert2';
import { WishlistService } from '../../services/wishlist-service';

@Component({
  selector: 'app-product-details',
  imports: [],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent {

  private productService = inject(ProductService)
  private cartService = inject(CartService)
  private route = inject(ActivatedRoute)
  private router = inject(Router)
  private authService = inject(AuthService)
  public wishlistService = inject(WishlistService)

  productId: number = 0
  product?: Product

  ngOnInit() {
  this.route.params.subscribe(params => {
    const id = Number(params['id'])
    this.productId = id

    this.productService.getProductsById(id).subscribe({
      next: (foundProduct) => {
        if (!foundProduct) {
          this.router.navigate(['/404'], { skipLocationChange: true })
        }
        
        else {
          this.product = foundProduct
        }
      },
      error: (err) => {
        console.error('API Error:', err)
        this.router.navigate(['/404'], { skipLocationChange: true })
      }
    })
  })
}

  add(product: Product) {
    if (this.authService.isLoggedIn()) {
      this.cartService.addToCart(product)

      this.showToast('Added to cart!', 'success')
    }

    else {
      this.showToast("Please login first", "info")

      this.router.navigateByUrl("/login")
    }
  }

  goBack(){
    this.router.navigateByUrl("/products")
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
