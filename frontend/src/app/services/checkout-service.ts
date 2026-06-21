import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Order } from '../models/order';
import { Observable } from 'rxjs';
import { environment } from '../../environment';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  private http = inject(HttpClient)

  private apiUrl = `${environment.apiUrl}/Orders`;

  placeOrder(request: { shippingAddress: string }): Observable<Order> {
    return this.http.post<Order>(`${this.apiUrl}/checkout`, request)
  }

  getUserOrderHistory(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}/my-history`)
  }
}