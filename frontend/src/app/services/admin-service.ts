import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environment';
import { Order } from '../models/order';

@Injectable({ 
  providedIn: 'root',
})
export class AdminService {
  private apiUrl = `${environment.apiUrl}/orders/admin`;
  
  allOrders = signal<Order[]>([])

  constructor(private http: HttpClient) {}

  loadAllOrders() {
    this.http.get<Order[]>(`${this.apiUrl}/all`).subscribe(orders => {
      this.allOrders.set(orders)
    });
  }

  updateStatus(orderId: number, newStatus: string) {
    return this.http.patch(`${this.apiUrl}/${orderId}/status`, `"${newStatus}"`, {
      headers: { 'Content-Type': 'application/json' }
    })
  }
}