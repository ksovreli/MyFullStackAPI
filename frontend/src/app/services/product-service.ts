import { inject, Injectable } from '@angular/core';
import { Product } from '../models/product';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'https://localhost:7119/api/Backpacks'
  private http = inject(HttpClient)
  
  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl)
  }

  getProductsById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`)
  }

  getSaleProducts(): Observable<Product[]> {
    return this.getProducts().pipe(
      map(products => products.filter(p => p.salePrice !== undefined))
    )
  }

  getFilteredProducts(category: string, sortBy: string): Observable<Product[]> {
    let params = new HttpParams()
      .set('category', category)
      .set('sortBy', sortBy)

      return this.http.get<Product[]>(`${this.apiUrl}/filter`, { params })
  }
}