import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth-service';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../services/cart-service';
import { CommonModule } from '@angular/common'; // Needed for @for and @if

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterModule, CommonModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  private authService = inject(AuthService)
  private cartService = inject(CartService)
  private router = inject(Router)

  regData = {
    username: '',
    email: '',
    password: ''
  }

  serverErrors: string[] = []

  onRegister() {
    this.serverErrors = []

    this.authService.register(this.regData).subscribe({
      next: () => {
        this.cartService.refreshCart();
        this.router.navigateByUrl('/home')
      },
      error: (err) => {
        console.error('Registration failed', err)

        if (err.error && err.error.errors) {
          this.serverErrors = err.error.errors
        }
        
        else {
          this.serverErrors = ['Registration failed. Please check your connection or try a different email.']
        }
      }
    })
  }

  hasNumber(str: string): boolean {
    return /[0-9]/.test(str)
  }

  hasUpper(str: string): boolean {
    return /[A-Z]/.test(str)
  }

  hasSymbol(str: string): boolean {
    return /[!@#$%^&*(),.?":{}|<>]/.test(str)
  } 
}