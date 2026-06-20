import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  // შენი ძირითადი ფერები
  private readonly gold = '#d4af37';
  private readonly red = '#ff003c';
  private readonly bg = '#0a0a0a';

  private darkTheme = {
    background: this.bg,
    color: '#fff',
    confirmButtonColor: this.gold,
    cancelButtonColor: '#222',
    buttonsStyling: true,
    customClass: {
      popup: 'cyber-alert-popup',
      title: 'cyber-alert-title',
      confirmButton: 'cyber-confirm-btn',
      cancelButton: 'cyber-cancel-btn',
      input: 'cyber-alert-input'
    }
  };

  success(message: string) {
    this.toast(message, 'success', this.gold);
  }

  error(message: string) {
    this.toast(message, 'error', this.red);
  }

  info(message: string) {
    this.toast(message, 'info', '#00f3ff'); // Cyber Blue აქცენტისთვის
  }

 async selectStatus(orderId: number, currentStatus: string) {
  return Swal.fire({
    ...this.darkTheme,
    title: 'UPDATE_ORDER_STATUS',
    text: `MODIFYING_DEPLOYMENT: #PX-${orderId}`,
    icon: 'info',
    input: 'select',
    // SweetAlert-ში მარცხნივ იწერება მნიშვნელობა, მარჯვნივ კი რა გამოჩნდეს
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
    didOpen: (popup) => {
      const select = popup.querySelector('.swal2-select') as HTMLElement;
      if (select) {
        select.style.background = '#111';
        select.style.color = '#d4af37';
        select.style.border = '1px solid #333';
        select.style.borderRadius = '0';
      }
    }
  });
}
  private toast(title: string, icon: 'success' | 'error' | 'info' | 'warning', accent: string) {
    Swal.fire({
      title: `> ${title}`,
      icon,
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 2000,
      timerProgressBar: true,
      background: this.bg,
      color: '#fff',
      didOpen: (toast) => {
        toast.style.borderLeft = `3px solid ${accent}`;
        toast.style.textShadow = `0 0 5px ${accent}66`;
      }
    });
  }

  async confirm(title: string, text: string) {
    return Swal.fire({
      ...this.darkTheme,
      title: `> ${title}`,
      text: text.toUpperCase(),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'CONFIRM_EXECUTION',
      cancelButtonText: 'ABORT'
    });
  }

  async confirmDelete(message: string) {
    return Swal.fire({
      ...this.darkTheme,
      title: 'TERMINATION_PROTOCOL',
      text: message,
      icon: 'warning',
      confirmButtonColor: this.red,
      confirmButtonText: 'CONFIRM_ERASURE',
      cancelButtonText: 'ABORT',
      didOpen: (popup) => {
        popup.style.border = `1px solid ${this.red}`;
        popup.style.boxShadow = `0 0 20px ${this.red}33`;
      }
    });
  }
}