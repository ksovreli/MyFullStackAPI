import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product-service';
import { Product } from '../../models/product';
import { CartService } from '../../services/cart-service';
import { AuthService } from '../../services/auth-service';
import { WishlistService } from '../../services/wishlist-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Review } from '../../models/review';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AlertService } from '../../services/alert-service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-product-details',
  imports: [CommonModule, FormsModule],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent {
  
  private productService = inject(ProductService)
  private cartService = inject(CartService)
  private alertService = inject(AlertService)
  private route = inject(ActivatedRoute)
  public router = inject(Router)
  public authService = inject(AuthService)
  public wishlistService = inject(WishlistService)
  private http = inject(HttpClient)
  public location = inject(Location)

  productId: number = 0
  product?: Product
  reviews: Review[] = []
  newReview: any = { rating: 0, comment: '' }
  hasReviewed: boolean = false
  currentUsername: string = ''
  isEditing = false
  editingReviewId: number | null = null

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = Number(params['id'])
      this.productId = id

      this.productService.getProductsById(id).subscribe({
        next: (foundProduct) => {
          if (!foundProduct) {
            this.router.navigate(['/404'], { skipLocationChange: true })
          } else {
            this.product = foundProduct
            this.loadReviews(id)
          }
        },
        error: () => this.router.navigate(['/404'], { skipLocationChange: true })
      })
    })
  }

  loadReviews(id: number) {
    this.currentUsername = this.authService.getUsername()
    this.http.get<Review[]>(`https://localhost:7119/api/Review/backpack/${id}`)
      .subscribe({
        next: (data) => {
          this.reviews = data
          if (!this.isEditing) {
            this.hasReviewed = this.reviews.some(r => r.username === this.currentUsername)
          }
        },
        error: (err) => console.error(err)
      })
  }

  prepareEdit(review: any) {
    this.isEditing = true
    this.editingReviewId = review.id
    this.hasReviewed = false

    this.newReview = {
      rating: review.rating,
      comment: review.comment,
      backpackId: this.productId
    }

    setTimeout(() => {
      document.querySelector('.add-review-container')?.scrollIntoView({ behavior: 'smooth' })
    }, 100)
  }

  submitReview() {
    if (!this.authService.isLoggedIn()) {
      this.alertService.info("Please login to post a review")
      this.router.navigateByUrl("/login")
      return
    }

    const token = this.authService.getToken()
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`)
    const reviewPayload = {
      rating: Number(this.newReview.rating),
      comment: this.newReview.comment,
      backpackId: Number(this.productId)
    }

    if (this.isEditing && this.editingReviewId) {
      this.http.put(`https://localhost:7119/api/Review/${this.editingReviewId}`, reviewPayload, { headers })
        .subscribe({
          next: () => {
            this.alertService.success('Review updated successfully!')
            this.resetReviewForm()
            this.loadReviews(this.productId)
          },
          error: (err) => console.error(err)
        })
    } else {
      this.http.post<Review>('https://localhost:7119/api/Review', reviewPayload, { headers })
        .subscribe({
          next: (savedReview) => {
            this.reviews.unshift(savedReview)
            this.hasReviewed = true
            this.newReview = { rating: 0, comment: '' }
            this.alertService.success('Review posted successfully!')
            this.loadReviews(this.productId)
          },
          error: (err) => {
            if (err.status === 409) {
              this.alertService.info('You have already reviewed this product.')
              this.hasReviewed = true
            } else {
              this.alertService.error('Could not post review.')
            }
          }
        })
    }
  }

  async deleteReview(id: number) {
    const result = await this.alertService.confirm('Are you sure?', "You won't be able to revert this!")
    
    if (result.isConfirmed) {
      const token = this.authService.getToken()
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`)

      this.http.delete(`https://localhost:7119/api/Review/${id}`, { headers })
        .subscribe({
          next: () => {
            this.alertService.success('Review deleted.')
            this.loadReviews(this.productId)
            this.hasReviewed = false
          },
          error: (err) => console.error(err)
        })
    }
  }

  resetReviewForm() {
    this.isEditing = false
    this.editingReviewId = null
    this.newReview = { rating: 0, comment: '' }
    this.hasReviewed = this.reviews.some(r => r.username === this.currentUsername)
  }

  add(product: Product) {
    if (this.authService.isLoggedIn()) {
      this.cartService.addToCart(product)
      this.alertService.success('Added to cart!')
    } else {
      this.alertService.info("Please login first")
      this.router.navigateByUrl("/login")
    }
  }

  goBack() {
    this.location.back()
  }
}
