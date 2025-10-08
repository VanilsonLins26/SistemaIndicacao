import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private authService = inject(AuthService); // INJETE O SERVIÇO

  title = 'Desafio VORTEX';
  currentYear = new Date().getFullYear();
  
  // A inicialização agora acontece aqui!
  ngOnInit(): void {
    const initAuth = this.authService.initAuthentication();
    if (initAuth) {
      initAuth.subscribe();
    }
  }
}