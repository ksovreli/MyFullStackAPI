import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environment';
import { Observable } from 'rxjs';
import { Product } from '../models/product';
import { Order } from '../models/order';

@Injectable({ providedIn: 'root' })
export class AdminService {

  private ordersApi = `${environment.apiUrl}/orders/admin`;
  private productsApi = `${environment.apiUrl}/backpacks`;
  private categoriesApi = `${environment.apiUrl}/categories`;
  private uploadApi = `${environment.apiUrl}/upload`;

  allOrders = signal<Order[]>([]);
  products = signal<Product[]>([]);

  constructor(private http: HttpClient) {}

  loadAllOrders() {
    this.http.get<Order[]>(`${this.ordersApi}/all`).subscribe({
      next: (res) => this.allOrders.set(res || []),
      error: (err) => console.error(err)
    });
  }

  loadProducts() {
    this.http.get<Product[]>(this.productsApi).subscribe({
      next: (res) => this.products.set(res || []),
      error: (err) => console.error(err)
    });
  }

  getCategories() {
    return this.http.get<any[]>(this.categoriesApi);
  }

  uploadImage(formData: FormData) {
  return this.http.post(`${this.uploadApi}`, formData);
}

  addBackpack(payload: any): Observable<Product> {
    return this.http.post<Product>(this.productsApi, payload);
  }

  updateBackpack(id: number, payload: any): Observable<Product> {
    return this.http.put<Product>(`${this.productsApi}/${id}`, payload);
  }

  deleteBackpack(id: number) {
    return this.http.delete(`${this.productsApi}/${id}`);
  }

  updateStatus(orderId: number, status: string) {
    return this.http.patch(`${this.ordersApi}/${orderId}/status`, `"${status}"`, {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}