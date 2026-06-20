import { Component, inject, PLATFORM_ID, OnInit } from '@angular/core';
import { isPlatformBrowser } from '@angular/common'; // Import this helper
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import AOS from 'aos';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Apex Store'
  isLoading = true

  private platformId = inject(PLATFORM_ID)

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      AOS.init()

      setTimeout(() => {
        this.isLoading = false
      }, 1000);
    }
    else {
      this.isLoading = false
    }
  }
}