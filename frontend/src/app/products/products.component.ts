import { Component, inject, OnInit } from '@angular/core';
import { Product } from '../models/product';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent {

  private http = inject(HttpClient)
  private route = inject(ActivatedRoute)
  private router = inject(Router)

  private apiUrl = 'https://localhost:7119/api/Backpacks'

  filteredProducts: Product[] = []
  selectedCategory: string = 'All Collections'
  selectedSortLabel: string = 'Recommended'
  isDropdownOpen: boolean = false

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.selectedCategory = params['category'] || 'All Collections'
      this.selectedSortLabel = params['sort'] || 'Recommended'

      this.loadFilteredProducts(this.selectedCategory, this.selectedSortLabel)
    })
  }

  loadFilteredProducts(category: string, sortBy: string) {
    let params = new HttpParams()
      .set('category', category)
      .set('sortBy', sortBy)

    this.http.get<Product[]>(`${this.apiUrl}/filter`, { params }).subscribe({
      next: (data) => {
        this.filteredProducts = data
      },
      error: (err) => {
        console.error('Failed to load products:', err)
      }
    })
  }

  changeCategory(category: string) {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { category: category },
      queryParamsHandling: 'merge'
    })
  }

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen
  }

  onOptionClick(event: Event, label: string) {
    event.stopPropagation()
    this.isDropdownOpen = false

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { sort: label },
      queryParamsHandling: 'merge'
    })
  }
}