import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth-service';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../services/cart-service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterModule, CommonModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  private authService = inject(AuthService)

// register.ts
regData = {
  Username: '', // დიდი U
  Email: '',    // დიდი E
  Password: ''  // დიდი P
}
  serverErrors = signal<string[]>([])
  registrationComplete = signal(false)

  onRegister() {
    this.serverErrors.set([])

    this.authService.register(this.regData).subscribe({
      next: () => {
        this.registrationComplete.set(true)
      },
      error: (err) => {
        console.error('Registration failed', err)
        
        if (err.error && err.error.errors) {
          const extractedErrors = Object.values(err.error.errors).flat() as string[]
          this.serverErrors.set(extractedErrors)
        }
        else {
          this.serverErrors.set(['PROTOCOL_ERROR: Connection to Apex Terminal lost.'])
        }
      }
    })
  }

  hasNumber = (str: string) => /[0-9]/.test(str);
  hasUpper = (str: string) => /[A-Z]/.test(str);
  hasSymbol = (str: string) => /[!@#$%^&*(),.?":{}|<>]/.test(str);
}