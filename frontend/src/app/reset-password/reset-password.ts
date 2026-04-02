import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import Swal from 'sweetalert2';
import { AuthService } from '../services/auth-service';

@Component({
  selector: 'app-reset-password',
  imports: [FormsModule, RouterModule],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss',
})
export class ResetPassword {
  private auth = inject(AuthService)
  private router = inject(Router)

  resetData = {
    email: '',
    token: '',
    newPassword: ''
  };

  isSending = false

  onObtainCode() {
    if (!this.resetData.email) {
      this.toast('error', 'SYSTEM ERROR', 'Please enter a valid email address.')
      return;
    }

    this.isSending = true

    this.auth.forgotPassword(this.resetData.email).subscribe({
      next: () => {
        this.isSending = false
        this.toast('success', 'ACCESS GRANTED', 'A 6-digit code has been sent to your inbox.')
      },
      error: (err) => {
        this.isSending = false
        console.error(err)
        this.toast('error', 'AUTH FAILED', 'Could not verify user email.')
      }
    })
  }

  onSubmit() {
    if (this.resetData.newPassword.length < 8) {
      this.toast('error', 'SECURITY PROTOCOL', 'Password must be at least 8 characters long.')
      return
    }

    if (!this.resetData.token || this.resetData.token.length !== 6) {
      this.toast('error', 'SECURITY PROTOCOL', 'Please enter a valid 6-digit code.')
      return
    }

    this.auth.resetPassword(this.resetData).subscribe({
      next: () => {
        this.toast('success', 'CREDENTIALS UPDATED', 'Password updated successfully.');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Server Response:', err);

        if (err.status === 400 && err.error?.errors) {
          const messages = Object.values(err.error.errors).flat().join('\n')
          this.toast('error', 'VALIDATION FAILED', messages);
        }
        else if (err.status === 400 && err.error?.message) {
          this.toast('error', 'AUTH FAILED', err.error.message)
        }
        else {
          this.toast('error', 'INVALID CODE', 'The code is invalid or has expired.')
        }
      }
    })
  }

  private toast(icon: 'success' | 'error', title: string, text: string) {
    Swal.fire({
      icon: icon,
      title: title,
      text: text,
      background: '#0d0d12',
      color: '#EEE6E6',
      confirmButtonColor: '#00ff88',
      iconColor: icon === 'success' ? '#00ff88' : '#ff4d4d'
    });
  }
}
