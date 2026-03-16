import { Component, inject } from '@angular/core';
import { Product } from '../models/product';
import { ProductService } from '../services/product-service';

import { ActivatedRoute, RouterModule } from '@angular/router';

@Component({
  selector: 'app-products',
  imports: [RouterModule],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent {

  private productService = inject(ProductService)
  private route = inject(ActivatedRoute)

  products: Product[] = []
  filteredProducts: Product[] = []
  selectedCategory: string = "All Collections"

  isDropdownOpen: boolean = false
  selectedSortLabel: string = 'Price: Low to High'

  ngOnInit() {
    this.productService.getProducts().subscribe(products => {
      this.products = products
      this.selectedSortLabel = 'Recommended'
      this.filterByCategory("All Collections")
    })

    this.route.queryParams.subscribe(params => {
      let categoryFromUrl = params['category']

      if (categoryFromUrl) {
        this.filterByCategory(categoryFromUrl)
      }

      else {
        this.filterByCategory("All Collections")
      }
    })
  }

 filterByCategory(category: string) {
  this.selectedCategory = category

  if (category === "All Collections") {
    this.filteredProducts = [...this.products]
  } 

  else if (category === "New Arrivals") {
    this.filteredProducts = this.products.filter(p => p.isNew)
  }

  else {
    this.filteredProducts = this.products.filter(p => 
      p.categoryName === category
    )
  }
  this.applySort()
}

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen
  }

  onOptionClick(event: Event, label: string) {
    event.stopPropagation()
    this.selectedSortLabel = label
    this.isDropdownOpen = false
    this.applySort()
  }

  applySort() {
    switch (this.selectedSortLabel) {
      case 'Recommended':
        this.filteredProducts.sort((a, b) => {
          let getScore = (p: Product) => {
            let score = 0

            if (p.isNew) score += 100

            score += (p.rating ?? 0) * 10

            if (p.salePrice) score += 30

            return score
          }

          return getScore(b) - getScore(a)
        })
        break

      case 'Price: Low to High':
        this.filteredProducts.sort((a, b) =>
          (a.salePrice ?? a.price ?? 0) - (b.salePrice ?? b.price ?? 0)
        )
        break

      case 'Price: High to Low':
        this.filteredProducts.sort((a, b) =>
          (b.salePrice ?? b.price ?? 0) - (a.salePrice ?? a.price ?? 0)
        )
        break

      case 'Top Rated':
        this.filteredProducts.sort((a, b) => (b.rating ?? 0) - (a.rating ?? 0))
        break


      case 'Newest':
        this.filteredProducts.sort((a, b) => Number(b.isNew ?? 0) - Number(a.isNew ?? 0))
        break
    }
  }
}