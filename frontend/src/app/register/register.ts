import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth-service';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../services/cart-service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterModule],
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

  onRegister() {
    this.authService.register(this.regData).subscribe({
      next: () => {
        this.cartService.refreshCart()
        this.router.navigateByUrl('/home')
      },
      error: (err) => {
        console.error('Registration failed', err)
        alert(err.error || 'Registration failed. Try a different email.')
      }
    })
  }
}