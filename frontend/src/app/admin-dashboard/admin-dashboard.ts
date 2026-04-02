import { Component, computed, inject, signal } from '@angular/core';
import { AdminService } from '../services/admin-service';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { AlertService } from '../services/alert-service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, DatePipe, DecimalPipe],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss'
})
export class AdminDashboard {
  private adminService = inject(AdminService)
  private alertService = inject(AlertService)

  allOrders = this.adminService.allOrders
  searchTerm = signal<string>('')

  totalRevenue = computed(() =>
    this.allOrders().reduce((sum, order) => sum + (order.totalPrice || 0), 0)
  )

  filteredOrders = computed(() => {
    const term = this.searchTerm().toLowerCase()
    if (!term) return this.allOrders()

    return this.allOrders().filter(order =>
      order.id.toString().includes(term) ||
      order.status.toLowerCase().includes(term) ||
      order.userId.toString().includes(term)
    )
  })

  ngOnInit() {
    this.adminService.loadAllOrders()
  }

  onSearch(event: Event) {
    const value = (event.target as HTMLInputElement).value
    this.searchTerm.set(value)
  }

  updateStatus(id: number, currentStatus: string) {
    this.alertService.selectStatus(id, currentStatus).then((result) => {
      if (result.isConfirmed && result.value) {
        this.adminService.updateStatus(id, result.value).subscribe({
          next: () => {
            this.adminService.loadAllOrders()
            this.alertService.success('DATABASE_UPDATED_SUCCESSFULLY')
          },
          error: () => {
            this.alertService.error('PROTOCOL_FAILURE: ACCESS_DENIED')
          }
        })
      }
    })
  }
}