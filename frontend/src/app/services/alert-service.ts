import { Injectable } from '@angular/core'
import Swal from 'sweetalert2'

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private darkTheme = {
    background: '#121212',
    color: '#fff',
    confirmButtonColor: '#c9a84c', // Gold/Cyberpunk accent
    cancelButtonColor: '#333'
  }

  success(message: string) {
    this.toast(message, 'success')
  }

  error(message: string) {
    this.toast(message, 'error')
  }

  info(message: string) {
    this.toast(message, 'info')
  }

  async selectStatus(orderId: number, currentStatus: string) {
  return Swal.fire({
    title: 'UPDATE_ORDER_STATUS',
    text: `Modifying Deployment #${orderId}`,
    icon: 'info',
    input: 'select',
    inputOptions: {
      'Pending': 'PENDING',
      'Processing': 'PROCESSING',
      'Shipped': 'SHIPPED',
      'Delivered': 'DELIVERED',
      'Cancelled': 'CANCELLED'
    },
    inputValue: currentStatus,
    inputPlaceholder: 'SELECT_NEW_STATUS',
    showCancelButton: true,
    confirmButtonText: 'APPLY_CHANGES',
    cancelButtonText: 'ABORT',
    ...this.darkTheme,
    customClass: {
      popup: 'terminal-alert-popup',
      input: 'terminal-alert-input'
    }
  })
}

  private toast(title: string, icon: 'success' | 'error' | 'info' | 'warning') {
    Swal.fire({
      title,
      icon,
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 2000,
      timerProgressBar: true,
      background: this.darkTheme.background,
      color: this.darkTheme.color
    })
  }

  async confirm(title: string, text: string) {
    return Swal.fire({
      title,
      text,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Confirm Execution',
      cancelButtonText: 'Abort',
      ...this.darkTheme
    })
  }
}